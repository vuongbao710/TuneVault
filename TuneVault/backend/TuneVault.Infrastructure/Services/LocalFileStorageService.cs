using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TuneVault.Application.Common;

namespace TuneVault.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".mp3", ".wav", ".mp4", ".webm"
    };

    private readonly string _mediaRoot;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LocalFileStorageService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
    {
        _mediaRoot = Path.Combine(env.ContentRootPath, "wwwroot", "media");
        Directory.CreateDirectory(_mediaRoot);
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAllowedExtension(string extension) => AllowedExtensions.Contains(extension);

    public async Task<(string relativePath, string contentType)> SaveMediaAsync(IFileUpload file, CancellationToken cancellationToken = default)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!IsAllowedExtension(ext))
        {
            throw new InvalidOperationException("File type not allowed.");
        }

        var fileName = $"{Guid.NewGuid()}{ext}";
        var fullPath = Path.Combine(_mediaRoot, fileName);
        await using var stream = File.Create(fullPath);
        await file.CopyToAsync(stream, cancellationToken);

        var relativePath = Path.Combine("media", fileName).Replace('\\', '/');
        return (relativePath, string.IsNullOrWhiteSpace(file.ContentType) ? GetContentType(ext) : file.ContentType);
    }

    public Task<(Stream stream, string contentType, long length)> OpenReadAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_mediaRoot, Path.GetFileName(relativePath));
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("Media file not found.", fullPath);
        }

        Stream stream = File.OpenRead(fullPath);
        var ext = Path.GetExtension(fullPath);
        return Task.FromResult((stream, GetContentType(ext), stream.Length));
    }

    private static string GetContentType(string ext) => ext switch
    {
        ".mp3" => "audio/mpeg",
        ".wav" => "audio/wav",
        ".mp4" => "video/mp4",
        ".webm" => "video/webm",
        _ => "application/octet-stream"
    };
}

public class FormFileUploadAdapter : IFileUpload
{
    private readonly IFormFile _file;

    public FormFileUploadAdapter(IFormFile file) => _file = file;

    public string FileName => _file.FileName;
    public string ContentType => _file.ContentType;
    public long Length => _file.Length;

    public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        => _file.CopyToAsync(target, cancellationToken);
}
