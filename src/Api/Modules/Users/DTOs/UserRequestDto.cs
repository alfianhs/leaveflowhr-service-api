using LeaveFlowHR.Api.Modules.Users.Enums;

namespace LeaveFlowHR.Api.Modules.Users.DTOs;

public class UserCreateRequestDto
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Department { get; set; } = null!;
    public Guid? ManagerId { get; set; }
    public EmployeeRole Role { get; set; }
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}