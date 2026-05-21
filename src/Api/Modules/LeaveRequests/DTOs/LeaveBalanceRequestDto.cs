namespace LeaveFlowHR.Api.Modules.LeaveRequests.DTOs;

public class LeaveBalanceCreateRequestDto
{
    public Guid UserId { get; set; }
    public int Year { get; set; }
    public int EntitledDays { get; set; }
}