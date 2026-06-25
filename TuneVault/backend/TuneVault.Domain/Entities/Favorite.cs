namespace TuneVault.Domain.Entities;

public class Favorite
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid MediaItemId { get; set; }
    public MediaItem MediaItem { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
