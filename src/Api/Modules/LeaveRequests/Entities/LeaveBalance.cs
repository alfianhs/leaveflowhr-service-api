using LeaveFlowHR.Api.Common.Entities;
using LeaveFlowHR.Api.Modules.Users.Entities;

namespace LeaveFlowHR.Api.Modules.LeaveRequests.Entities;

public class LeaveBalance : BaseEntity
{
    public Guid UserId { get; set; }
    public int Year { get; set; }
    public int EntitledDays { get; set; }
    public int UsedDays { get; set; }
    public int PendingDays { get; set; }

    public User User { get; set; } = null!;
}