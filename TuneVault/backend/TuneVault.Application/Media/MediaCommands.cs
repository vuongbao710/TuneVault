using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TuneVault.Application.Common;
using TuneVault.Application.Media.Dtos;
using TuneVault.Domain.Entities;
using TuneVault.Domain.Enums;

namespace TuneVault.Application.Media;

public record GetMediaListQuery : IRequest<IReadOnlyList<MediaItemDto>>;

public class GetMediaListQueryHandler : IRequestHandler<GetMediaListQuery, IReadOnlyList<MediaItemDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IIdentityService _identityService;

    public GetMediaListQueryHandler(IApplicationDbContext db, IIdentityService identityService)
    {
        _db = db;
        _identityService = identityService;
    }

    public async Task<IReadOnlyList<MediaItemDto>> Handle(GetMediaListQuery request, CancellationToken cancellationToken)
    {
        var items = await _db.MediaItems.OrderByDescending(m => m.CreatedAt).ToListAsync(cancellationToken);
        var result = new List<MediaItemDto>();
        foreach (var item in items)
        {
            var ownerName = await _identityService.GetDisplayNameAsync(item.OwnerId, cancellationToken) ?? "Unknown";
            result.Add(Map(item, ownerName));
        }
        return result;
    }

    internal static MediaItemDto Map(MediaItem item, string ownerName) =>
        new(item.Id, item.Title, item.Artist, item.Genre, item.Type, item.DurationSeconds, item.Description, item.OwnerId, ownerName, item.CreatedAt);
}

public record GetMediaByIdQuery(Guid Id) : IRequest<MediaItemDto?>;

public class GetMediaByIdQueryHandler : IRequestHandler<GetMediaByIdQuery, MediaItemDto?>
{
    private readonly IApplicationDbContext _db;
    private readonly IIdentityService _identityService;

    public GetMediaByIdQueryHandler(IApplicationDbContext db, IIdentityService identityService)
    {
        _db = db;
        _identityService = identityService;
    }

    public async Task<MediaItemDto?> Handle(GetMediaByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _db.MediaItems.FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);
        if (item == null) return null;
        var ownerName = await _identityService.GetDisplayNameAsync(item.OwnerId, cancellationToken) ?? "Unknown";
        return GetMediaListQueryHandler.Map(item, ownerName);
    }
}

public record UploadMediaCommand(
    string Title,
    string Artist,
    string Genre,
    MediaType Type,
    int DurationSeconds,
    string? Description,
    IFileUpload File) : IRequest<MediaItemDto>;

public class UploadMediaCommandValidator : AbstractValidator<UploadMediaCommand>
{
    public UploadMediaCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Artist).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Genre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.File).NotNull();
    }
}

public class UploadMediaCommandHandler : IRequestHandler<UploadMediaCommand, MediaItemDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorage;
    private readonly IIdentityService _identityService;

    public UploadMediaCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        IFileStorageService fileStorage,
        IIdentityService identityService)
    {
        _db = db;
        _currentUser = currentUser;
        _fileStorage = fileStorage;
        _identityService = identityService;
    }

    public async Task<MediaItemDto> Handle(UploadMediaCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId))
        {
            throw new UnauthorizedAccessException();
        }

        var ext = Path.GetExtension(request.File.FileName).ToLowerInvariant();
        if (!_fileStorage.IsAllowedExtension(ext))
        {
            throw new InvalidOperationException("File type not allowed.");
        }

        var (path, contentType) = await _fileStorage.SaveMediaAsync(request.File, cancellationToken);
        var item = new MediaItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Artist = request.Artist,
            Genre = request.Genre,
            Type = request.Type,
            DurationSeconds = request.DurationSeconds,
            Description = request.Description,
            FilePath = path,
            ContentType = contentType,
            OwnerId = _currentUser.UserId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Add(item);
        await _db.SaveChangesAsync(cancellationToken);

        var ownerName = await _identityService.GetDisplayNameAsync(item.OwnerId, cancellationToken) ?? "Unknown";
        return GetMediaListQueryHandler.Map(item, ownerName);
    }
}

public record DeleteMediaCommand(Guid Id) : IRequest<bool>;

public class DeleteMediaCommandHandler : IRequestHandler<DeleteMediaCommand, bool>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteMediaCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(DeleteMediaCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId))
        {
            throw new UnauthorizedAccessException();
        }

        var item = await _db.MediaItems.FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);
        if (item == null) return false;
        if (item.OwnerId != _currentUser.UserId)
        {
            throw new UnauthorizedAccessException("You can only delete your own media.");
        }

        _db.Remove(item);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public record GetMediaStreamQuery(Guid Id) : IRequest<MediaStreamResult?>;

public record MediaStreamResult(Stream Stream, string ContentType, long Length, string FileName);

public class GetMediaStreamQueryHandler : IRequestHandler<GetMediaStreamQuery, MediaStreamResult?>
{
    private readonly IApplicationDbContext _db;
    private readonly IFileStorageService _fileStorage;

    public GetMediaStreamQueryHandler(IApplicationDbContext db, IFileStorageService fileStorage)
    {
        _db = db;
        _fileStorage = fileStorage;
    }

    public async Task<MediaStreamResult?> Handle(GetMediaStreamQuery request, CancellationToken cancellationToken)
    {
        var item = await _db.MediaItems.FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);
        if (item == null) return null;

        var (stream, contentType, length) = await _fileStorage.OpenReadAsync(item.FilePath, cancellationToken);
        return new MediaStreamResult(stream, contentType, length, Path.GetFileName(item.FilePath));
    }
}
