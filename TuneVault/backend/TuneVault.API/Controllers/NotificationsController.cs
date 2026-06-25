using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuneVault.Application.Common;
using TuneVault.Application.Notifications;

namespace TuneVault.API.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> List()
    {
        var result = await _mediator.Send(new GetNotificationsQuery());
        return Ok(ApiResponse.Ok(result));
    }

    [HttpPut("{id:guid}/read")]
    public async Task<ActionResult<ApiResponse<object>>> MarkRead(Guid id)
    {
        var ok = await _mediator.Send(new MarkNotificationReadCommand(id));
        if (!ok) return NotFound(ApiResponse.Fail("Notification not found."));
        return Ok(ApiResponse.Ok(new { read = true }));
    }

    [HttpPut("read-all")]
    public async Task<ActionResult<ApiResponse<object>>> MarkAllRead()
    {
        var count = await _mediator.Send(new MarkAllNotificationsReadCommand());
        return Ok(ApiResponse.Ok(new { updated = count }));
    }
}
