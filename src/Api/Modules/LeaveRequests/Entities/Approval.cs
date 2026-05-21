using LeaveFlowHR.Api.Common.Entities;
using LeaveFlowHR.Api.Modules.LeaveRequests.Enums;
using LeaveFlowHR.Api.Modules.Users.Entities;
using LeaveFlowHR.Api.Modules.Users.Enums;

namespace LeaveFlowHR.Api.Modules.LeaveRequests.Entities;

public class Approval : BaseEntity
{
    public Guid LeaveRequestId { get; set; }
    public Guid? ApproverId { get; set; }
    public EmployeeRole ApproverRole { get; set; }
    public ApprovalDecisionEnum? Decision { get; set; }
    public string? Note { get; set; }

    public LeaveRequest LeaveRequest { get; set; } = null!;
    public User? Approver { get; set; }
}