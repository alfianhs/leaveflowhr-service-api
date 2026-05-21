using Microsoft.OpenApi.Attributes;

namespace LeaveFlowHR.Api.Modules.Users.Enums;

public enum EmployeeRole
{
    [Display(name: "Employee")]
    EMPLOYEE = 1,
    [Display(name: "Manager")]
    MANAGER = 2,
    [Display(name: "HR")]
    HR = 3,
    [Display(name: "Admin")]
    ADMIN = 4,
}