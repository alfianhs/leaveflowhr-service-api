using LeaveFlowHR.Api.Modules.LeaveRequests.Enums;

namespace LeaveFlowHR.Api.Modules.LeaveRequests.DTOs;

public class LeaveRequestCreateRequestDto
{
    public LeaveType Type { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string? Reason { get; set; }
}