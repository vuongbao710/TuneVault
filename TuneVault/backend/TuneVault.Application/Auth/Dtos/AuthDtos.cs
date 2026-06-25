namespace TuneVault.Application.Auth.Dtos;

public record AuthResponseDto(string Token, string UserId, string Email, string DisplayName);

public record UserDto(string Id, string Email, string DisplayName, string? Bio, string? AvatarUrl);
