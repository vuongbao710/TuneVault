using TuneVault.Application.Media.Dtos;

namespace TuneVault.Application.Playlists.Dtos;

public record PlaylistDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsPublic,
    string OwnerId,
    string OwnerName,
    int TrackCount,
    DateTime CreatedAt);

public record PlaylistDetailDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsPublic,
    string OwnerId,
    string OwnerName,
    IReadOnlyList<MediaItemDto> Tracks,
    DateTime CreatedAt);
