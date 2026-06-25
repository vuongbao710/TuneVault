using TuneVault.Application.Media.Dtos;

namespace TuneVault.Application.Shares.Dtos;

public record ShareDto(
    Guid Id,
    string SenderId,
    string SenderName,
    string ReceiverId,
    string ReceiverName,
    Guid? MediaItemId,
    Guid? PlaylistId,
    MediaItemDto? Media,
    string? PlaylistName,
    DateTime SharedAt);
