using FluentValidation;
using LeaveFlowHR.Api.Modules.LeaveRequests.DTOs;
using LeaveFlowHR.Api.Modules.LeaveRequests.Enums;

namespace LeaveFlowHR.Api.Modules.LeaveRequests.Validators;

public class LeaveRequestCreateRequestValidator : AbstractValidator<LeaveRequestCreateRequestDto>
{
    public LeaveRequestCreateRequestValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Leave type is required")
            .IsInEnum().WithMessage("Invalid leave type");
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow)).When(x => x.Type == LeaveType.SICK).WithMessage("Start date cannot be in the past")
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow)).When(x => x.Type == LeaveType.ANNUAL).WithMessage("Start date cannot be in the past or today");
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date must be after start date");
    }
}