using LeaveFlowHR.Api.Modules.Users.DTOs;

namespace LeaveFlowHR.Api.Modules.LeaveRequests.DTOs;

public class LeaveBalanceResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public UserSimpleResponseDto User { get; set; } = null!;
    public int Year { get; set; }
    public int EntitledDays { get; set; }
    public int UsedDays { get; set; }
    public int PendingDays { get; set; }
    public int RemainingDays { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class LeaveBalanceSimpleResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Year { get; set; }
    public int EntitledDays { get; set; }
    public int UsedDays { get; set; }
    public int PendingDays { get; set; }
}