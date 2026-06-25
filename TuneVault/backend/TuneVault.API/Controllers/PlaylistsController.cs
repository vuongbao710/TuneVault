using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuneVault.Application.Common;
using TuneVault.Application.Playlists;

namespace TuneVault.API.Controllers;

[ApiController]
[Route("api/playlists")]
public class PlaylistsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlaylistsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> List()
    {
        var result = await _mediator.Send(new GetPlaylistsQuery());
        return Ok(ApiResponse.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> Get(Guid id)
    {
        var result = await _mediator.Send(new GetPlaylistByIdQuery(id));
        if (result == null) return NotFound(ApiResponse.Fail("Playlist not found."));
        return Ok(ApiResponse.Ok(result));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] CreatePlaylistCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(ApiResponse.Ok(result));
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> Update(Guid id, [FromBody] UpdatePlaylistRequest request)
    {
        var result = await _mediator.Send(new UpdatePlaylistCommand(id, request.Name, request.Description, request.IsPublic));
        if (result == null) return NotFound(ApiResponse.Fail("Playlist not found."));
        return Ok(ApiResponse.Ok(result));
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        var ok = await _mediator.Send(new DeletePlaylistCommand(id));
        if (!ok) return NotFound(ApiResponse.Fail("Playlist not found."));
        return Ok(ApiResponse.Ok(new { deleted = true }));
    }

    [HttpPost("{id:guid}/tracks/{mediaId:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> AddTrack(Guid id, Guid mediaId)
    {
        var ok = await _mediator.Send(new AddTrackToPlaylistCommand(id, mediaId));
        if (!ok) return NotFound(ApiResponse.Fail("Playlist or media not found."));
        return Ok(ApiResponse.Ok(new { added = true }));
    }

    [HttpDelete("{id:guid}/tracks/{mediaId:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> RemoveTrack(Guid id, Guid mediaId)
    {
        var ok = await _mediator.Send(new RemoveTrackFromPlaylistCommand(id, mediaId));
        if (!ok) return NotFound(ApiResponse.Fail("Track not found."));
        return Ok(ApiResponse.Ok(new { removed = true }));
    }
}

public record UpdatePlaylistRequest(string Name, string? Description, bool IsPublic);
