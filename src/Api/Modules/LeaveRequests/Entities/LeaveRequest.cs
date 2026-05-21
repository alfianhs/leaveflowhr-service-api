using LeaveFlowHR.Api.Common.Entities;
using LeaveFlowHR.Api.Modules.LeaveRequests.Enums;
using LeaveFlowHR.Api.Modules.Users.Entities;

namespace LeaveFlowHR.Api.Modules.LeaveRequests.Entities;

public class LeaveRequest : BaseEntity
{
    public Guid UserId { get; set; }
    public LeaveType Type { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int TotalDays { get; set; }
    public string? Reason { get; set; }
    public LeaveRequestStatus Status { get; set; } = LeaveRequestStatus.PENDING;

    public User User { get; set; } = null!;
    public List<Approval> Approvals { get; set; } = new List<Approval>();
}