using Microsoft.AspNetCore.SignalR;
using TuneVault.Application.Common;
using TuneVault.Application.Notifications.Dtos;
using TuneVault.Infrastructure.SignalR;

namespace TuneVault.Infrastructure.Services;

public class NotificationPublisher : INotificationPublisher
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationPublisher(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task PublishAsync(string userId, NotificationDto notification, CancellationToken cancellationToken = default)
        => _hubContext.Clients.Group(userId).SendAsync("ReceiveNotification", notification, cancellationToken);
}
