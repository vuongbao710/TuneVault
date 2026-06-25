using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TuneVault.Application.Common;
using TuneVault.Application.Media;
using TuneVault.Application.Media.Dtos;
using TuneVault.Application.Playlists.Dtos;
using TuneVault.Domain.Entities;

namespace TuneVault.Application.Playlists;

public record GetPlaylistsQuery : IRequest<IReadOnlyList<PlaylistDto>>;

public class GetPlaylistsQueryHandler : IRequestHandler<GetPlaylistsQuery, IReadOnlyList<PlaylistDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;

    public GetPlaylistsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser, IIdentityService identityService)
    {
        _db = db;
        _currentUser = currentUser;
        _identityService = identityService;
    }

    public async Task<IReadOnlyList<PlaylistDto>> Handle(GetPlaylistsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var query = _db.Playlists.AsQueryable();
        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where(p => p.IsPublic || p.OwnerId == userId);
        }
        else
        {
            query = query.Where(p => p.IsPublic);
        }

        var playlists = await query.OrderByDescending(p => p.CreatedAt).ToListAsync(cancellationToken);
        var result = new List<PlaylistDto>();
        foreach (var p in playlists)
        {
            var ownerName = await _identityService.GetDisplayNameAsync(p.OwnerId, cancellationToken) ?? "Unknown";
            var trackCount = await _db.PlaylistTracks.CountAsync(t => t.PlaylistId == p.Id, cancellationToken);
            result.Add(new PlaylistDto(p.Id, p.Name, p.Description, p.IsPublic, p.OwnerId, ownerName, trackCount, p.CreatedAt));
        }
        return result;
    }
}

public record GetPlaylistByIdQuery(Guid Id) : IRequest<PlaylistDetailDto?>;

public class GetPlaylistByIdQueryHandler : IRequestHandler<GetPlaylistByIdQuery, PlaylistDetailDto?>
{
    private readonly IApplicationDbContext _db;
    private readonly IIdentityService _identityService;

    public GetPlaylistByIdQueryHandler(IApplicationDbContext db, IIdentityService identityService)
    {
        _db = db;
        _identityService = identityService;
    }

    public async Task<PlaylistDetailDto?> Handle(GetPlaylistByIdQuery request, CancellationToken cancellationToken)
    {
        var playlist = await _db.Playlists.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (playlist == null) return null;

        var tracks = await _db.PlaylistTracks
            .Where(t => t.PlaylistId == request.Id)
            .Join(_db.MediaItems, t => t.MediaItemId, m => m.Id, (t, m) => new { t.Order, Media = m })
            .OrderBy(x => x.Order)
            .ToListAsync(cancellationToken);

        var ownerName = await _identityService.GetDisplayNameAsync(playlist.OwnerId, cancellationToken) ?? "Unknown";
        var mediaDtos = new List<MediaItemDto>();
        foreach (var track in tracks)
        {
            var mediaOwner = await _identityService.GetDisplayNameAsync(track.Media.OwnerId, cancellationToken) ?? "Unknown";
            mediaDtos.Add(GetMediaListQueryHandler.Map(track.Media, mediaOwner));
        }

        return new PlaylistDetailDto(playlist.Id, playlist.Name, playlist.Description, playlist.IsPublic, playlist.OwnerId, ownerName, mediaDtos, playlist.CreatedAt);
    }
}

public record CreatePlaylistCommand(string Name, string? Description, bool IsPublic) : IRequest<PlaylistDto>;

public class CreatePlaylistCommandValidator : AbstractValidator<CreatePlaylistCommand>
{
    public CreatePlaylistCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}

public class CreatePlaylistCommandHandler : IRequestHandler<CreatePlaylistCommand, PlaylistDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;

    public CreatePlaylistCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser, IIdentityService identityService)
    {
        _db = db;
        _currentUser = currentUser;
        _identityService = identityService;
    }

    public async Task<PlaylistDto> Handle(CreatePlaylistCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId)) throw new UnauthorizedAccessException();

        var playlist = new Playlist
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            IsPublic = request.IsPublic,
            OwnerId = _currentUser.UserId,
            CreatedAt = DateTime.UtcNow
        };
        _db.Add(playlist);
        await _db.SaveChangesAsync(cancellationToken);

        var ownerName = await _identityService.GetDisplayNameAsync(playlist.OwnerId, cancellationToken) ?? "Unknown";
        return new PlaylistDto(playlist.Id, playlist.Name, playlist.Description, playlist.IsPublic, playlist.OwnerId, ownerName, 0, playlist.CreatedAt);
    }
}

public record UpdatePlaylistCommand(Guid Id, string Name, string? Description, bool IsPublic) : IRequest<PlaylistDto?>;

public class UpdatePlaylistCommandValidator : AbstractValidator<UpdatePlaylistCommand>
{
    public UpdatePlaylistCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}

public class UpdatePlaylistCommandHandler : IRequestHandler<UpdatePlaylistCommand, PlaylistDto?>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;

    public UpdatePlaylistCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser, IIdentityService identityService)
    {
        _db = db;
        _currentUser = currentUser;
        _identityService = identityService;
    }

    public async Task<PlaylistDto?> Handle(UpdatePlaylistCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId)) throw new UnauthorizedAccessException();

        var playlist = await _db.Playlists.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (playlist == null) return null;
        if (playlist.OwnerId != _currentUser.UserId) throw new UnauthorizedAccessException();

        playlist.Name = request.Name;
        playlist.Description = request.Description;
        playlist.IsPublic = request.IsPublic;
        await _db.SaveChangesAsync(cancellationToken);

        var ownerName = await _identityService.GetDisplayNameAsync(playlist.OwnerId, cancellationToken) ?? "Unknown";
        var trackCount = await _db.PlaylistTracks.CountAsync(t => t.PlaylistId == playlist.Id, cancellationToken);
        return new PlaylistDto(playlist.Id, playlist.Name, playlist.Description, playlist.IsPublic, playlist.OwnerId, ownerName, trackCount, playlist.CreatedAt);
    }
}

public record DeletePlaylistCommand(Guid Id) : IRequest<bool>;

public class DeletePlaylistCommandHandler : IRequestHandler<DeletePlaylistCommand, bool>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeletePlaylistCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(DeletePlaylistCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId)) throw new UnauthorizedAccessException();

        var playlist = await _db.Playlists.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (playlist == null) return false;
        if (playlist.OwnerId != _currentUser.UserId) throw new UnauthorizedAccessException();

        var tracks = await _db.PlaylistTracks.Where(t => t.PlaylistId == request.Id).ToListAsync(cancellationToken);
        foreach (var track in tracks) _db.Remove(track);
        _db.Remove(playlist);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public record AddTrackToPlaylistCommand(Guid PlaylistId, Guid MediaId) : IRequest<bool>;

public class AddTrackToPlaylistCommandHandler : IRequestHandler<AddTrackToPlaylistCommand, bool>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public AddTrackToPlaylistCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(AddTrackToPlaylistCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId)) throw new UnauthorizedAccessException();

        var playlist = await _db.Playlists.FirstOrDefaultAsync(p => p.Id == request.PlaylistId, cancellationToken);
        if (playlist == null) return false;
        if (playlist.OwnerId != _currentUser.UserId) throw new UnauthorizedAccessException();

        var mediaExists = await _db.MediaItems.AnyAsync(m => m.Id == request.MediaId, cancellationToken);
        if (!mediaExists) return false;

        var exists = await _db.PlaylistTracks.AnyAsync(t => t.PlaylistId == request.PlaylistId && t.MediaItemId == request.MediaId, cancellationToken);
        if (exists) return true;

        var order = await _db.PlaylistTracks.CountAsync(t => t.PlaylistId == request.PlaylistId, cancellationToken);
        _db.Add(new PlaylistTrack { PlaylistId = request.PlaylistId, MediaItemId = request.MediaId, Order = order });
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public record RemoveTrackFromPlaylistCommand(Guid PlaylistId, Guid MediaId) : IRequest<bool>;

public class RemoveTrackFromPlaylistCommandHandler : IRequestHandler<RemoveTrackFromPlaylistCommand, bool>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public RemoveTrackFromPlaylistCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(RemoveTrackFromPlaylistCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId)) throw new UnauthorizedAccessException();

        var playlist = await _db.Playlists.FirstOrDefaultAsync(p => p.Id == request.PlaylistId, cancellationToken);
        if (playlist == null) return false;
        if (playlist.OwnerId != _currentUser.UserId) throw new UnauthorizedAccessException();

        var track = await _db.PlaylistTracks.FirstOrDefaultAsync(t => t.PlaylistId == request.PlaylistId && t.MediaItemId == request.MediaId, cancellationToken);
        if (track == null) return false;

        _db.Remove(track);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
