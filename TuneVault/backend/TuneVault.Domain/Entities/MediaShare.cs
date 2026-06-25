namespace TuneVault.Domain.Entities;

public class MediaShare
{
    public Guid Id { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string ReceiverId { get; set; } = string.Empty;
    public Guid? MediaItemId { get; set; }
    public MediaItem? MediaItem { get; set; }
    public Guid? PlaylistId { get; set; }
    public Playlist? Playlist { get; set; }
    public DateTime SharedAt { get; set; } = DateTime.UtcNow;
}
