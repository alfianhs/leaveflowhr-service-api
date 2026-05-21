using LeaveFlowHR.Api.Common.Entities;
using LeaveFlowHR.Api.Modules.Users.Enums;

namespace LeaveFlowHR.Api.Modules.Users.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Department { get; set; } = null!;
    public Guid? ManagerId { get; set; }
    public EmployeeRole Role { get; set; } = EmployeeRole.EMPLOYEE;

    public User? Manager { get; set; }
}