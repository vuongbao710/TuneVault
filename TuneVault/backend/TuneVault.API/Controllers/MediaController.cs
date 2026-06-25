using System.Net.Http.Headers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuneVault.Application.Common;
using TuneVault.Application.Media;
using TuneVault.Domain.Enums;
using TuneVault.Infrastructure.Services;

namespace TuneVault.API.Controllers;

[ApiController]
[Route("api/media")]
public class MediaController : ControllerBase
{
    private readonly IMediator _mediator;

    public MediaController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> List()
    {
        var result = await _mediator.Send(new GetMediaListQuery());
        return Ok(ApiResponse.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetMediaByIdQuery(id));
        if (result == null) return NotFound(ApiResponse.Fail("Media not found."));
        return Ok(ApiResponse.Ok(result));
    }

    [HttpPost("upload")]
    [Authorize]
    [RequestSizeLimit(104_857_600)]
    public async Task<ActionResult<ApiResponse<object>>> Upload(
        [FromForm] string title,
        [FromForm] string artist,
        [FromForm] string genre,
        [FromForm] MediaType type,
        [FromForm] int durationSeconds,
        [FromForm] string? description,
        IFormFile file)
    {
        if (file == null) return BadRequest(ApiResponse.Fail("File is required."));
        var command = new UploadMediaCommand(title, artist, genre, type, durationSeconds, description, new FormFileUploadAdapter(file));
        var result = await _mediator.Send(command);
        return Ok(ApiResponse.Ok(result));
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        var ok = await _mediator.Send(new DeleteMediaCommand(id));
        if (!ok) return NotFound(ApiResponse.Fail("Media not found."));
        return Ok(ApiResponse.Ok(new { deleted = true }));
    }

    [HttpGet("{id:guid}/stream")]
    [AllowAnonymous]
    public async Task<IActionResult> Stream(Guid id)
    {
        var result = await _mediator.Send(new GetMediaStreamQuery(id));
        if (result == null) return NotFound(ApiResponse.Fail("Media not found."));

        var rangeHeader = Request.Headers.Range.ToString();
        if (!string.IsNullOrEmpty(rangeHeader) && RangeHeaderValue.TryParse(rangeHeader, out var range))
        {
            var start = range.Ranges.First().From ?? 0;
            var end = range.Ranges.First().To ?? result.Length - 1;
            if (end >= result.Length) end = result.Length - 1;
            var length = end - start + 1;

            result.Stream.Seek(start, SeekOrigin.Begin);
            var buffer = new byte[length];
            await result.Stream.ReadExactlyAsync(buffer, 0, (int)length);

            Response.StatusCode = StatusCodes.Status206PartialContent;
            Response.Headers.ContentRange = $"bytes {start}-{end}/{result.Length}";
            Response.Headers.AcceptRanges = "bytes";
            return File(buffer, result.ContentType, enableRangeProcessing: false);
        }

        Response.Headers.AcceptRanges = "bytes";
        return File(result.Stream, result.ContentType, enableRangeProcessing: true);
    }
}
