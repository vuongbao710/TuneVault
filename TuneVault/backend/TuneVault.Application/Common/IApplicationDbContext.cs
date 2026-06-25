using TuneVault.Domain.Entities;

namespace TuneVault.Application.Common;

public interface IApplicationDbContext
{
    IQueryable<MediaItem> MediaItems { get; }
    IQueryable<Playlist> Playlists { get; }
    IQueryable<PlaylistTrack> PlaylistTracks { get; }
    IQueryable<MediaShare> MediaShares { get; }
    IQueryable<Notification> Notifications { get; }
    IQueryable<Favorite> Favorites { get; }
    IQueryable<PlayHistory> PlayHistories { get; }

    void Add<T>(T entity) where T : class;
    void Remove<T>(T entity) where T : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
