using System.Net;
using LeaveFlowHR.Api.Common.Responses;
using LeaveFlowHR.Api.Infrastructure.Extensions;
using LeaveFlowHR.Api.Modules.Auth.DTOs;
using LeaveFlowHR.Api.Modules.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserDto = LeaveFlowHR.Api.Modules.Users.DTOs;

namespace LeaveFlowHR.Api.Modules.Auth.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login", Name = "Login")]
    [Tags("Auth")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);
        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponse<object>.Fail(result.Error, null, (int)HttpStatusCode.BadRequest));
        }

        return Ok(ApiResponse<LoginResponseDto?>.Success(result.Value, "Logged in successfully", (int)HttpStatusCode.OK));
    }

    [HttpGet("me", Name = "Get Me")]
    [Tags("Auth")]
    [Authorize]
    public async Task<IActionResult> GetMeAsync()
    {
        Guid userId = User.GetUserId();

        var result = await _authService.GetMeAsync(userId);
        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponse<object>.Fail(result.Error, null, (int)HttpStatusCode.BadRequest));
        }

        return Ok(ApiResponse<UserDto.UserWithManagerResponseDto?>.Success(result.Value, "User retrieved successfully", (int)HttpStatusCode.OK));
    }
}