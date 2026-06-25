using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuneVault.Application.Common;
using TuneVault.Application.Shares;

namespace TuneVault.API.Controllers;

[ApiController]
[Route("api/shares")]
[Authorize]
public class SharesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SharesController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<ApiResponse<object>>> Share([FromBody] ShareMediaCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(ApiResponse.Ok(result));
    }

    [HttpGet("with-me")]
    public async Task<ActionResult<ApiResponse<object>>> WithMe()
    {
        var result = await _mediator.Send(new GetSharesWithMeQuery());
        return Ok(ApiResponse.Ok(result));
    }

    [HttpGet("by-me")]
    public async Task<ActionResult<ApiResponse<object>>> ByMe()
    {
        var result = await _mediator.Send(new GetSharesByMeQuery());
        return Ok(ApiResponse.Ok(result));
    }
}
