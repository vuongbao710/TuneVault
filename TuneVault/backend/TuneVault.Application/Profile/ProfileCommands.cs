using FluentValidation;
using MediatR;
using TuneVault.Application.Auth.Dtos;
using TuneVault.Application.Common;

namespace TuneVault.Application.Profile;

public record GetProfileQuery : IRequest<UserDto>;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, UserDto>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;

    public GetProfileQueryHandler(ICurrentUserService currentUser, IIdentityService identityService)
    {
        _currentUser = currentUser;
        _identityService = identityService;
    }

    public async Task<UserDto> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId))
        {
            throw new UnauthorizedAccessException();
        }

        var user = await _identityService.GetUserAsync(_currentUser.UserId, cancellationToken);
        return user ?? throw new KeyNotFoundException("User not found.");
    }
}

public record UpdateProfileCommand(string DisplayName, string? Bio, string? AvatarUrl) : IRequest<UserDto>;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Bio).MaximumLength(500);
        RuleFor(x => x.AvatarUrl).MaximumLength(500);
    }
}

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UserDto>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;

    public UpdateProfileCommandHandler(ICurrentUserService currentUser, IIdentityService identityService)
    {
        _currentUser = currentUser;
        _identityService = identityService;
    }

    public async Task<UserDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_currentUser.UserId))
        {
            throw new UnauthorizedAccessException();
        }

        var user = await _identityService.UpdateProfileAsync(
            _currentUser.UserId,
            request.DisplayName,
            request.Bio,
            request.AvatarUrl,
            cancellationToken);

        return user ?? throw new KeyNotFoundException("User not found.");
    }
}
