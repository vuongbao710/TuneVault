using FluentValidation;
using MediatR;
using TuneVault.Application.Auth.Dtos;
using TuneVault.Application.Common;

namespace TuneVault.Application.Auth;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        => _identityService.LoginAsync(request.Email, request.Password, cancellationToken);
}
