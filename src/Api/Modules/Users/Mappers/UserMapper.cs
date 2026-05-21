using LeaveFlowHR.Api.Modules.Users.DTOs;
using LeaveFlowHR.Api.Modules.Users.Entities;
using Microsoft.OpenApi.Extensions;

namespace LeaveFlowHR.Api.Modules.Users.Mappers;

public static class UserMapper
{
    public static UserSimpleResponseDto ToUserSimpleResponseDto(this User user)
    {
        return new UserSimpleResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Department = user.Department
        };
    }

    public static UserWithManagerResponseDto ToUserWithManagerResponseDto(this User user)
    {
        return new UserWithManagerResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Department = user.Department,
            ManagerId = user.ManagerId,
            Role = new Common.Dtos.EnumResponseDto
            {
                Key = (int)user.Role,
                Value = user.Role.GetDisplayName()
            },
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            Manager = user.Manager?.ToUserSimpleResponseDto() ?? null
        };
    }

    public static UserWithoutManagerResponseDto ToUserWithoutManagerResponseDto(this User user)
    {
        return new UserWithoutManagerResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Department = user.Department,
            ManagerId = user.ManagerId,
            Role = new Common.Dtos.EnumResponseDto
            {
                Key = (int)user.Role,
                Value = user.Role.GetDisplayName()
            },
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}