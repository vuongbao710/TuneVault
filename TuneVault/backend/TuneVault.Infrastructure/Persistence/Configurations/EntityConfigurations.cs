using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TuneVault.Domain.Entities;

namespace TuneVault.Infrastructure.Persistence.Configurations;

public class MediaItemConfiguration : IEntityTypeConfiguration<MediaItem>
{
    public void Configure(EntityTypeBuilder<MediaItem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Artist).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Genre).HasMaxLength(100).IsRequired();
        builder.Property(x => x.FilePath).HasMaxLength(500).IsRequired();
        builder.Property(x => x.ContentType).HasMaxLength(100).IsRequired();
    }
}

public class PlaylistConfiguration : IEntityTypeConfiguration<Playlist>
{
    public void Configure(EntityTypeBuilder<Playlist> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
    }
}

public class PlaylistTrackConfiguration : IEntityTypeConfiguration<PlaylistTrack>
{
    public void Configure(EntityTypeBuilder<PlaylistTrack> builder)
    {
        builder.HasKey(x => new { x.PlaylistId, x.MediaItemId });
        builder.HasOne(x => x.Playlist).WithMany(p => p.Tracks).HasForeignKey(x => x.PlaylistId);
        builder.HasOne(x => x.MediaItem).WithMany().HasForeignKey(x => x.MediaItemId);
    }
}

public class MediaShareConfiguration : IEntityTypeConfiguration<MediaShare>
{
    public void Configure(EntityTypeBuilder<MediaShare> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.SenderId, x.ReceiverId, x.MediaItemId, x.PlaylistId }).IsUnique();
    }
}

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PayloadJson).HasMaxLength(2000).IsRequired();
    }
}

public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.UserId, x.MediaItemId }).IsUnique();
        builder.HasOne(x => x.MediaItem).WithMany().HasForeignKey(x => x.MediaItemId);
    }
}

public class PlayHistoryConfiguration : IEntityTypeConfiguration<PlayHistory>
{
    public void Configure(EntityTypeBuilder<PlayHistory> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.MediaItem).WithMany().HasForeignKey(x => x.MediaItemId);
    }
}
