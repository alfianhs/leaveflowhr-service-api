using LeaveFlowHR.Api.Common.Dtos;

namespace LeaveFlowHR.Api.Modules.Users.DTOs;

public class UserWithoutManagerResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Department { get; set; } = null!;
    public Guid? ManagerId { get; set; }
    public EnumResponseDto Role { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UserWithManagerResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Department { get; set; } = null!;
    public Guid? ManagerId { get; set; }
    public UserSimpleResponseDto? Manager { get; set; }
    public EnumResponseDto Role { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UserSimpleResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Department { get; set; } = null!;
}
