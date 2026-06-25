using MediatR;
using Microsoft.EntityFrameworkCore;
using TuneVault.Application.Common;
using TuneVault.Application.Notifications.Dtos;

namespace TuneVault.Application.Notifications;

public record GetNotificationsQuery : IRequest<IReadOnlyList<NotificationDto>>;

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, IReadOnlyList<NotificationDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetNotificationsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId)) throw new UnauthorizedAccessException();

        return await _db.Notifications
            .Where(n => n.UserId == _currentUser.UserId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto(n.Id, n.Type, n.PayloadJson, n.IsRead, n.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}

public record MarkNotificationReadCommand(Guid Id) : IRequest<bool>;

public class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand, bool>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public MarkNotificationReadCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId)) throw new UnauthorizedAccessException();

        var notification = await _db.Notifications.FirstOrDefaultAsync(n => n.Id == request.Id, cancellationToken);
        if (notification == null) return false;
        if (notification.UserId != _currentUser.UserId) throw new UnauthorizedAccessException();

        notification.IsRead = true;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public record MarkAllNotificationsReadCommand : IRequest<int>;

public class MarkAllNotificationsReadCommandHandler : IRequestHandler<MarkAllNotificationsReadCommand, int>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public MarkAllNotificationsReadCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(MarkAllNotificationsReadCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId)) throw new UnauthorizedAccessException();

        var notifications = await _db.Notifications
            .Where(n => n.UserId == _currentUser.UserId && !n.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var n in notifications) n.IsRead = true;
        await _db.SaveChangesAsync(cancellationToken);
        return notifications.Count;
    }
}
