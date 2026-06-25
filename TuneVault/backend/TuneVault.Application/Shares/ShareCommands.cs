using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TuneVault.Application.Common;
using TuneVault.Application.Media;
using TuneVault.Application.Media.Dtos;
using TuneVault.Application.Notifications.Dtos;
using TuneVault.Application.Shares.Dtos;
using TuneVault.Domain.Entities;
using TuneVault.Domain.Enums;

namespace TuneVault.Application.Shares;

public record ShareMediaCommand(string ReceiverUserId, Guid? MediaItemId, Guid? PlaylistId) : IRequest<ShareDto>;

public class ShareMediaCommandValidator : AbstractValidator<ShareMediaCommand>
{
    public ShareMediaCommandValidator()
    {
        RuleFor(x => x.ReceiverUserId).NotEmpty();
        RuleFor(x => x).Must(x => x.MediaItemId.HasValue ^ x.PlaylistId.HasValue)
            .WithMessage("Provide either MediaItemId or PlaylistId, not both.");
    }
}

public class ShareMediaCommandHandler : IRequestHandler<ShareMediaCommand, ShareDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    private readonly TuneVault.Application.Common.INotificationPublisher _notificationPublisher;
    private readonly ShareMapper _mapper;

    public ShareMediaCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        IIdentityService identityService,
        TuneVault.Application.Common.INotificationPublisher notificationPublisher,
        ShareMapper mapper)
    {
        _db = db;
        _currentUser = currentUser;
        _identityService = identityService;
        _notificationPublisher = notificationPublisher;
        _mapper = mapper;
    }

    public async Task<ShareDto> Handle(ShareMediaCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId)) throw new UnauthorizedAccessException();
        if (request.ReceiverUserId == _currentUser.UserId)
        {
            throw new InvalidOperationException("Cannot share with yourself.");
        }

        if (!await _identityService.UserExistsAsync(request.ReceiverUserId, cancellationToken))
        {
            throw new KeyNotFoundException("Receiver not found.");
        }

        MediaItem? media = null;
        Playlist? playlist = null;

        if (request.MediaItemId.HasValue)
        {
            media = await _db.MediaItems.FirstOrDefaultAsync(m => m.Id == request.MediaItemId.Value, cancellationToken);
            if (media == null) throw new KeyNotFoundException("Media not found.");
        }
        else if (request.PlaylistId.HasValue)
        {
            playlist = await _db.Playlists.FirstOrDefaultAsync(p => p.Id == request.PlaylistId.Value, cancellationToken);
            if (playlist == null) throw new KeyNotFoundException("Playlist not found.");
        }

        var existing = await _db.MediaShares.FirstOrDefaultAsync(s =>
            s.SenderId == _currentUser.UserId &&
            s.ReceiverId == request.ReceiverUserId &&
            s.MediaItemId == request.MediaItemId &&
            s.PlaylistId == request.PlaylistId, cancellationToken);

        if (existing != null)
        {
            return await _mapper.MapAsync(existing, cancellationToken);
        }

        var share = new MediaShare
        {
            Id = Guid.NewGuid(),
            SenderId = _currentUser.UserId,
            ReceiverId = request.ReceiverUserId,
            MediaItemId = request.MediaItemId,
            PlaylistId = request.PlaylistId,
            SharedAt = DateTime.UtcNow
        };
        _db.Add(share);

        var payload = JsonSerializer.Serialize(new
        {
            shareId = share.Id,
            senderId = share.SenderId,
            mediaItemId = share.MediaItemId,
            playlistId = share.PlaylistId,
            mediaTitle = media?.Title,
            playlistName = playlist?.Name
        });

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = request.ReceiverUserId,
            Type = request.MediaItemId.HasValue ? NotificationType.MediaShared : NotificationType.PlaylistShared,
            PayloadJson = payload,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
        _db.Add(notification);
        await _db.SaveChangesAsync(cancellationToken);

        var notificationDto = new NotificationDto(notification.Id, notification.Type, notification.PayloadJson, notification.IsRead, notification.CreatedAt);
        await _notificationPublisher.PublishAsync(request.ReceiverUserId, notificationDto, cancellationToken);

        return await _mapper.MapAsync(share, cancellationToken);
    }
}

public record GetSharesWithMeQuery : IRequest<IReadOnlyList<ShareDto>>;

public class GetSharesWithMeQueryHandler : IRequestHandler<GetSharesWithMeQuery, IReadOnlyList<ShareDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly ShareMapper _mapper;

    public GetSharesWithMeQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser, ShareMapper mapper)
    {
        _db = db;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ShareDto>> Handle(GetSharesWithMeQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId)) throw new UnauthorizedAccessException();
        var shares = await _db.MediaShares.Where(s => s.ReceiverId == _currentUser.UserId).OrderByDescending(s => s.SharedAt).ToListAsync(cancellationToken);
        var result = new List<ShareDto>();
        foreach (var share in shares)
        {
            result.Add(await _mapper.MapAsync(share, cancellationToken));
        }
        return result;
    }
}

public record GetSharesByMeQuery : IRequest<IReadOnlyList<ShareDto>>;

public class GetSharesByMeQueryHandler : IRequestHandler<GetSharesByMeQuery, IReadOnlyList<ShareDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly ShareMapper _mapper;

    public GetSharesByMeQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser, ShareMapper mapper)
    {
        _db = db;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ShareDto>> Handle(GetSharesByMeQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId)) throw new UnauthorizedAccessException();
        var shares = await _db.MediaShares.Where(s => s.SenderId == _currentUser.UserId).OrderByDescending(s => s.SharedAt).ToListAsync(cancellationToken);
        var result = new List<ShareDto>();
        foreach (var share in shares)
        {
            result.Add(await _mapper.MapAsync(share, cancellationToken));
        }
        return result;
    }
}

public class ShareMapper
{
    private readonly IApplicationDbContext _db;
    private readonly IIdentityService _identityService;

    public ShareMapper(IApplicationDbContext db, IIdentityService identityService)
    {
        _db = db;
        _identityService = identityService;
    }

    public async Task<ShareDto> MapAsync(MediaShare share, CancellationToken cancellationToken)
    {
        var senderName = await _identityService.GetDisplayNameAsync(share.SenderId, cancellationToken) ?? "Unknown";
        var receiverName = await _identityService.GetDisplayNameAsync(share.ReceiverId, cancellationToken) ?? "Unknown";
        MediaItemDto? mediaDto = null;
        string? playlistName = null;

        if (share.MediaItemId.HasValue)
        {
            var media = await _db.MediaItems.FirstOrDefaultAsync(m => m.Id == share.MediaItemId, cancellationToken);
            if (media != null)
            {
                var ownerName = await _identityService.GetDisplayNameAsync(media.OwnerId, cancellationToken) ?? "Unknown";
                mediaDto = GetMediaListQueryHandler.Map(media, ownerName);
            }
        }

        if (share.PlaylistId.HasValue)
        {
            playlistName = await _db.Playlists.Where(p => p.Id == share.PlaylistId).Select(p => p.Name).FirstOrDefaultAsync(cancellationToken);
        }

        return new ShareDto(share.Id, share.SenderId, senderName, share.ReceiverId, receiverName, share.MediaItemId, share.PlaylistId, mediaDto, playlistName, share.SharedAt);
    }
}
