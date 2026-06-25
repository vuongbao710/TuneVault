using Microsoft.AspNetCore.Identity;
using TuneVault.Application.Auth.Dtos;
using TuneVault.Application.Common;
using TuneVault.Infrastructure.Identity;

namespace TuneVault.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> RegisterAsync(string email, string password, string displayName, CancellationToken cancellationToken = default)
    {
        var existing = await _userManager.FindByEmailAsync(email);
        if (existing != null) throw new InvalidOperationException("Email already registered.");

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            DisplayName = displayName
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        var token = _jwtTokenService.GenerateToken(user.Id, user.Email!, user.DisplayName);
        return new AuthResponseDto(token, user.Id, user.Email!, user.DisplayName);
    }

    public async Task<AuthResponseDto> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) throw new UnauthorizedAccessException("Invalid credentials.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded) throw new UnauthorizedAccessException("Invalid credentials.");

        var token = _jwtTokenService.GenerateToken(user.Id, user.Email!, user.DisplayName);
        return new AuthResponseDto(token, user.Id, user.Email!, user.DisplayName);
    }

    public async Task<UserDto?> GetUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user == null ? null : Map(user);
    }

    public async Task<UserDto?> UpdateProfileAsync(string userId, string displayName, string? bio, string? avatarUrl, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;

        user.DisplayName = displayName;
        user.Bio = bio;
        user.AvatarUrl = avatarUrl;
        await _userManager.UpdateAsync(user);
        return Map(user);
    }

    public async Task<bool> UserExistsAsync(string userId, CancellationToken cancellationToken = default)
        => await _userManager.FindByIdAsync(userId) != null;

    public async Task<string?> GetDisplayNameAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user?.DisplayName;
    }

    private static UserDto Map(ApplicationUser user)
        => new(user.Id, user.Email!, user.DisplayName, user.Bio, user.AvatarUrl);
}
