using UserDto = LeaveFlowHR.Api.Modules.Users.DTOs;

namespace LeaveFlowHR.Api.Modules.Auth.DTOs;

public class LoginResponseDto
{
    public string AccessToken { get; set; } = null!;
    public DateTime ExpiredAt { get; set; }
    public UserDto.UserWithoutManagerResponseDto User { get; set; } = new();
}