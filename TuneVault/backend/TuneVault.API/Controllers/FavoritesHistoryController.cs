using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuneVault.Application.Common;
using TuneVault.Application.Favorites;
using TuneVault.Application.History;

namespace TuneVault.API.Controllers;

[ApiController]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly IMediator _mediator;

    public FavoritesController(IMediator mediator) => _mediator = mediator;

    [HttpPost("api/favorites/{mediaId:guid}/toggle")]
    public async Task<ActionResult<ApiResponse<object>>> Toggle(Guid mediaId)
    {
        var isFavorite = await _mediator.Send(new ToggleFavoriteCommand(mediaId));
        return Ok(ApiResponse.Ok(new { isFavorite }));
    }

    [HttpGet("api/favorites")]
    public async Task<ActionResult<ApiResponse<object>>> List()
    {
        var result = await _mediator.Send(new GetFavoritesQuery());
        return Ok(ApiResponse.Ok(result));
    }
}

[ApiController]
[Authorize]
public class HistoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public HistoryController(IMediator mediator) => _mediator = mediator;

    [HttpPost("api/history/{mediaId:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Record(Guid mediaId)
    {
        await _mediator.Send(new RecordPlayHistoryCommand(mediaId));
        return Ok(ApiResponse.Ok(new { recorded = true }));
    }

    [HttpGet("api/history")]
    public async Task<ActionResult<ApiResponse<object>>> List()
    {
        var result = await _mediator.Send(new GetPlayHistoryQuery());
        return Ok(ApiResponse.Ok(result));
    }
}
