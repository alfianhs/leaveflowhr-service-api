using LeaveFlowHR.Api.Modules.LeaveRequests.DTOs;
using LeaveFlowHR.Api.Modules.LeaveRequests.Entities;
using LeaveFlowHR.Api.Modules.Users.Mappers;

namespace LeaveFlowHR.Api.Modules.LeaveRequests.Mappers;

public static class LeaveBalanceMapper
{
    public static LeaveBalanceResponseDto ToLeaveBalanceResponseDto(this LeaveBalance leaveBalance)
    {
        return new LeaveBalanceResponseDto
        {
            Id = leaveBalance.Id,
            UserId = leaveBalance.UserId,
            User = leaveBalance.User.ToUserSimpleResponseDto(),
            Year = leaveBalance.Year,
            EntitledDays = leaveBalance.EntitledDays,
            UsedDays = leaveBalance.UsedDays,
            PendingDays = leaveBalance.PendingDays,
            RemainingDays = leaveBalance.EntitledDays - (leaveBalance.UsedDays + leaveBalance.PendingDays),
            CreatedAt = leaveBalance.CreatedAt,
            UpdatedAt = leaveBalance.UpdatedAt
        };
    }

    public static LeaveBalanceSimpleResponseDto ToLeaveBalanceSimpleResponseDto(this LeaveBalance leaveBalance)
    {
        return new LeaveBalanceSimpleResponseDto
        {
            Id = leaveBalance.Id,
            UserId = leaveBalance.UserId,
            Year = leaveBalance.Year,
            EntitledDays = leaveBalance.EntitledDays,
            UsedDays = leaveBalance.UsedDays,
            PendingDays = leaveBalance.PendingDays
        };
    }
}