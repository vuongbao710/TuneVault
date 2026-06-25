using MediatR;
using Microsoft.EntityFrameworkCore;
using TuneVault.Application.Common;
using TuneVault.Application.Media;
using TuneVault.Application.Media.Dtos;
using TuneVault.Domain.Entities;

namespace TuneVault.Application.History;

public record RecordPlayHistoryCommand(Guid MediaId) : IRequest<bool>;

public class RecordPlayHistoryCommandHandler : IRequestHandler<RecordPlayHistoryCommand, bool>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public RecordPlayHistoryCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(RecordPlayHistoryCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId)) throw new UnauthorizedAccessException();

        var mediaExists = await _db.MediaItems.AnyAsync(m => m.Id == request.MediaId, cancellationToken);
        if (!mediaExists) throw new KeyNotFoundException("Media not found.");

        _db.Add(new PlayHistory
        {
            Id = Guid.NewGuid(),
            UserId = _currentUser.UserId,
            MediaItemId = request.MediaId,
            PlayedAt = DateTime.UtcNow
        });

        var histories = await _db.PlayHistories
            .Where(h => h.UserId == _currentUser.UserId)
            .OrderByDescending(h => h.PlayedAt)
            .Skip(10)
            .ToListAsync(cancellationToken);

        foreach (var old in histories) _db.Remove(old);

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public record GetPlayHistoryQuery : IRequest<IReadOnlyList<MediaItemDto>>;

public class GetPlayHistoryQueryHandler : IRequestHandler<GetPlayHistoryQuery, IReadOnlyList<MediaItemDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;

    public GetPlayHistoryQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser, IIdentityService identityService)
    {
        _db = db;
        _currentUser = currentUser;
        _identityService = identityService;
    }

    public async Task<IReadOnlyList<MediaItemDto>> Handle(GetPlayHistoryQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId)) throw new UnauthorizedAccessException();

        var items = await _db.PlayHistories
            .Where(h => h.UserId == _currentUser.UserId)
            .OrderByDescending(h => h.PlayedAt)
            .Take(10)
            .Join(_db.MediaItems, h => h.MediaItemId, m => m.Id, (h, m) => m)
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
