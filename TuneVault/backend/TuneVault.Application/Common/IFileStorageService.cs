namespace TuneVault.Application.Common;

public interface IFileStorageService
{
    Task<(string relativePath, string contentType)> SaveMediaAsync(IFileUpload file, CancellationToken cancellationToken = default);
    Task<(Stream stream, string contentType, long length)> OpenReadAsync(string relativePath, CancellationToken cancellationToken = default);
    bool IsAllowedExtension(string extension);
}

public interface IFileUpload
{
    string FileName { get; }
    string ContentType { get; }
    long Length { get; }
    Task CopyToAsync(Stream target, CancellationToken cancellationToken = default);
}
