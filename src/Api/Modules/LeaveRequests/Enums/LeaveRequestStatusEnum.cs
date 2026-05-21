using Microsoft.OpenApi.Attributes;

namespace LeaveFlowHR.Api.Modules.LeaveRequests.Enums;

public enum LeaveRequestStatus
{
    [Display(name: "Pending")]
    PENDING = 1,
    [Display(name: "Approved")]
    APPROVED = 2,
    [Display(name: "Rejected")]
    REJECTED = 3,
    [Display(name: "Cancelled")]
    CANCELLED = 4
}