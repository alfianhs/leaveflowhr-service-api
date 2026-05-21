using FluentValidation;
using LeaveFlowHR.Api.Modules.LeaveRequests.DTOs;

namespace LeaveFlowHR.Api.Modules.LeaveRequests.Validators;

public class LeaveBalanceCreateRequestValidator : AbstractValidator<LeaveBalanceCreateRequestDto>
{
    public LeaveBalanceCreateRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.Year)
            .GreaterThan(0).WithMessage("Year must be greater than 0.");

        RuleFor(x => x.EntitledDays)
            .GreaterThanOrEqualTo(0).WithMessage("EntitledDays must be greater than or equal to 0.");
    }
}