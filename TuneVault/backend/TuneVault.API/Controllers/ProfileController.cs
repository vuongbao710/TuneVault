using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuneVault.Application.Common;
using TuneVault.Application.Profile;

namespace TuneVault.API.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> Get()
    {
        var result = await _mediator.Send(new GetProfileQuery());
        return Ok(ApiResponse.Ok(result));
    }

    [HttpPut]
    public async Task<ActionResult<ApiResponse<object>>> Update([FromBody] UpdateProfileCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(ApiResponse.Ok(result));
    }
}
