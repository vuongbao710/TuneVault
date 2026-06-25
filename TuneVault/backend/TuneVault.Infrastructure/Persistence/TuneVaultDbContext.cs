using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TuneVault.Application.Common;
using TuneVault.Domain.Entities;
using TuneVault.Infrastructure.Identity;

namespace TuneVault.Infrastructure.Persistence;

public class TuneVaultDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public TuneVaultDbContext(DbContextOptions<TuneVaultDbContext> options) : base(options)
    {
    }

    public DbSet<MediaItem> MediaItems => Set<MediaItem>();
    public DbSet<Playlist> Playlists => Set<Playlist>();
    public DbSet<PlaylistTrack> PlaylistTracks => Set<PlaylistTrack>();
    public DbSet<MediaShare> MediaShares => Set<MediaShare>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<PlayHistory> PlayHistories => Set<PlayHistory>();

    IQueryable<MediaItem> IApplicationDbContext.MediaItems => MediaItems;
    IQueryable<Playlist> IApplicationDbContext.Playlists => Playlists;
    IQueryable<PlaylistTrack> IApplicationDbContext.PlaylistTracks => PlaylistTracks;
    IQueryable<MediaShare> IApplicationDbContext.MediaShares => MediaShares;
    IQueryable<Notification> IApplicationDbContext.Notifications => Notifications;
    IQueryable<Favorite> IApplicationDbContext.Favorites => Favorites;
    IQueryable<PlayHistory> IApplicationDbContext.PlayHistories => PlayHistories;

    public new void Add<T>(T entity) where T : class => Set<T>().Add(entity);
    public new void Remove<T>(T entity) where T : class => Set<T>().Remove(entity);

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(TuneVaultDbContext).Assembly);
    }
}
