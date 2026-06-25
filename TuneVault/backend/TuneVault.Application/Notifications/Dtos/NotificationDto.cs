using TuneVault.Domain.Enums;

namespace TuneVault.Application.Notifications.Dtos;

public record NotificationDto(
    Guid Id,
    NotificationType Type,
    string PayloadJson,
    bool IsRead,
    DateTime CreatedAt);
