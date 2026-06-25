using MediatR;
using Microsoft.EntityFrameworkCore;
using TuneVault.Application.Common;
using TuneVault.Application.Media;
using TuneVault.Application.Media.Dtos;

namespace TuneVault.Application.Search;

public record SearchMediaQuery(string Query, int Page = 1, int PageSize = 20) : IRequest<SearchMediaResult>;

public record SearchMediaResult(IReadOnlyList<MediaItemDto> Items, int TotalCount, int Page, int PageSize);

public class SearchMediaQueryHandler : IRequestHandler<SearchMediaQuery, SearchMediaResult>
{
    private readonly IApplicationDbContext _db;
    private readonly IIdentityService _identityService;

    public SearchMediaQueryHandler(IApplicationDbContext db, IIdentityService identityService)
    {
        _db = db;
        _identityService = identityService;
    }

    public async Task<SearchMediaResult> Handle(SearchMediaQuery request, CancellationToken cancellationToken)
    {
        var q = request.Query.Trim().ToLower();
        var query = _db.MediaItems.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(m =>
                m.Title.ToLower().Contains(q) ||
                m.Artist.ToLower().Contains(q) ||
                m.Genre.ToLower().Contains(q));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var result = new List<MediaItemDto>();
        foreach (var item in items)
        {
            var ownerName = await _identityService.GetDisplayNameAsync(item.OwnerId, cancellationToken) ?? "Unknown";
            result.Add(GetMediaListQueryHandler.Map(item, ownerName));
        }

        return new SearchMediaResult(result, total, request.Page, request.PageSize);
    }
}
