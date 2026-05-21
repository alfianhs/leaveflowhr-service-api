using FluentValidation;
using LeaveFlowHR.Api.Modules.Auth.DTOs;

namespace LeaveFlowHR.Api.Modules.Auth.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}
