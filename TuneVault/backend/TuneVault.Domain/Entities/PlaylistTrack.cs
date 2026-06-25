namespace TuneVault.Domain.Entities;

public class PlaylistTrack
{
    public Guid PlaylistId { get; set; }
    public Playlist Playlist { get; set; } = null!;
    public Guid MediaItemId { get; set; }
    public MediaItem MediaItem { get; set; } = null!;
    public int Order { get; set; }
}
