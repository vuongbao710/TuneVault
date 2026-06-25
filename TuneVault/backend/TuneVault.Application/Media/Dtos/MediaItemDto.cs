using TuneVault.Domain.Enums;

namespace TuneVault.Application.Media.Dtos;

public record MediaItemDto(
    Guid Id,
    string Title,
    string Artist,
    string Genre,
    MediaType Type,
    int DurationSeconds,
    string? Description,
    string OwnerId,
    string OwnerName,
    DateTime CreatedAt);
