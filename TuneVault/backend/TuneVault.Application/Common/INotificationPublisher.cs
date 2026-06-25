using TuneVault.Application.Notifications.Dtos;

namespace TuneVault.Application.Common;

public interface INotificationPublisher
{
    Task PublishAsync(string userId, NotificationDto notification, CancellationToken cancellationToken = default);
}
