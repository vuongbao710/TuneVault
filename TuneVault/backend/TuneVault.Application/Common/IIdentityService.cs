using TuneVault.Application.Auth.Dtos;

namespace TuneVault.Application.Common;

public interface IIdentityService
{
    Task<AuthResponseDto> RegisterAsync(string email, string password, string displayName, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<UserDto?> UpdateProfileAsync(string userId, string displayName, string? bio, string? avatarUrl, CancellationToken cancellationToken = default);
    Task<bool> UserExistsAsync(string userId, CancellationToken cancellationToken = default);
    Task<string?> GetDisplayNameAsync(string userId, CancellationToken cancellationToken = default);
}
