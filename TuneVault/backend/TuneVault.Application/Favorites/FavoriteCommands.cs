using MediatR;
using Microsoft.EntityFrameworkCore;
using TuneVault.Application.Common;
using TuneVault.Application.Media;
using TuneVault.Application.Media.Dtos;
using TuneVault.Domain.Entities;

namespace TuneVault.Application.Favorites;

public record ToggleFavoriteCommand(Guid MediaId) : IRequest<bool>;

public class ToggleFavoriteCommandHandler : IRequestHandler<ToggleFavoriteCommand, bool>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public ToggleFavoriteCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(ToggleFavoriteCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId)) throw new UnauthorizedAccessException();

        var mediaExists = await _db.MediaItems.AnyAsync(m => m.Id == request.MediaId, cancellationToken);
        if (!mediaExists) throw new KeyNotFoundException("Media not found.");

        var favorite = await _db.Favorites
            .FirstOrDefaultAsync(f => f.UserId == _currentUser.UserId && f.MediaItemId == request.MediaId, cancellationToken);

        if (favorite != null)
        {
            _db.Remove(favorite);
            await _db.SaveChangesAsync(cancellationToken);
            return false;
        }

        _db.Add(new Favorite
        {
            Id = Guid.NewGuid(),
            UserId = _currentUser.UserId,
            MediaItemId = request.MediaId,
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public record GetFavoritesQuery : IRequest<IReadOnlyList<MediaItemDto>>;

public class GetFavoritesQueryHandler : IRequestHandler<GetFavoritesQuery, IReadOnlyList<MediaItemDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;

    public GetFavoritesQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser, IIdentityService identityService)
    {
        _db = db;
        _currentUser = currentUser;
        _identityService = identityService;
    }

    public async Task<IReadOnlyList<MediaItemDto>> Handle(GetFavoritesQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId)) throw new UnauthorizedAccessException();

        var items = await _db.Favorites
            .Where(f => f.UserId == _currentUser.UserId)
            .Join(_db.MediaItems, f => f.MediaItemId, m => m.Id, (f, m) => m)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);

        var result = new List<MediaItemDto>();
        foreach (var item in items)
        {
            var ownerName = await _identityService.GetDisplayNameAsync(item.OwnerId, cancellationToken) ?? "Unknown";
            result.Add(GetMediaListQueryHandler.Map(item, ownerName));
        }
        return result;
    }
}
