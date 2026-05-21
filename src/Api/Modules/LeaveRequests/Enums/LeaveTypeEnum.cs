using Microsoft.OpenApi.Attributes;

namespace LeaveFlowHR.Api.Modules.LeaveRequests.Enums;

public enum LeaveType
{
    [Display(name: "Annual")]
    ANNUAL = 1,
    [Display(name: "Sick")]
    SICK = 2
}