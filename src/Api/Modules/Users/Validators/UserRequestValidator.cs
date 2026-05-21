using FluentValidation;
using LeaveFlowHR.Api.Modules.Users.DTOs;

namespace LeaveFlowHR.Api.Modules.Users.Validators;

public class UserCreateRequestValidator : AbstractValidator<UserCreateRequestDto>
{
    public UserCreateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email address")
            .NotEmpty().WithMessage("Email is required");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters");
        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Passwords do not match");
        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required");
        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required")
            .IsInEnum().WithMessage("Invalid role");
    }
}