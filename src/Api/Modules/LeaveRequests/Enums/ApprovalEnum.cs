using Microsoft.OpenApi.Attributes;

namespace LeaveFlowHR.Api.Modules.LeaveRequests.Enums;

public enum ApprovalDecisionEnum
{
    [Display(name: "Approved")]
    APPROVED = 1,
    [Display(name: "Rejected")]
    REJECTED = 2
}