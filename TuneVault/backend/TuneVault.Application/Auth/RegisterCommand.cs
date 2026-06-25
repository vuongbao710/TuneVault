using FluentValidation;
using MediatR;
using TuneVault.Application.Auth.Dtos;
using TuneVault.Application.Common;

namespace TuneVault.Application.Auth;

public record RegisterCommand(string Email, string Password, string DisplayName) : IRequest<AuthResponseDto>;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100);
    }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IIdentityService _identityService;

    public RegisterCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        => _identityService.RegisterAsync(request.Email, request.Password, request.DisplayName, cancellationToken);
}
