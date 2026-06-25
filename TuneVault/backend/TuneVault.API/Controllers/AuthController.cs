using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuneVault.Application.Auth;
using TuneVault.Application.Common;

namespace TuneVault.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(ApiResponse.Ok(result));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(ApiResponse.Ok(result));
    }
}
