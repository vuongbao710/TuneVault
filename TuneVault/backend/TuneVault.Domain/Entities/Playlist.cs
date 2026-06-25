namespace TuneVault.Domain.Entities;

public class Playlist
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; } = true;
    public string OwnerId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<PlaylistTrack> Tracks { get; set; } = new List<PlaylistTrack>();
}
