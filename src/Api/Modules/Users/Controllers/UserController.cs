using LeaveFlowHR.Api.Common.Responses;
using LeaveFlowHR.Api.Modules.Users.DTOs;
using LeaveFlowHR.Api.Modules.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveFlowHR.Api.Modules.Users.Controllers;

[Route("api/users")]
[ApiController]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet(Name = "Get Users")]
    [Tags("Users")]
    [Authorize(Roles = "ADMIN,HR")]
    public async Task<IActionResult> GetPaginatedListAsync([FromQuery] Common.Dtos.PaginationRequestDto request)
    {
        var result = await _userService.GetPaginatedListAsync(request);
        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponse<object>.Fail(result.Error, null, (int)System.Net.HttpStatusCode.BadRequest));
        }

        return Ok(ApiResponse<List<UserWithoutManagerResponseDto>>.SuccessPaged(result.Value!.Items, result.Value.Page, result.Value.PageSize, result.Value.TotalItems, "Users retrieved successfully"));
    }

    [HttpGet("{id}", Name = "Get User By Id")]
    [Tags("Users")]
    [Authorize(Roles = "ADMIN,HR")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var result = await _userService.GetByIdAsync(id);
        if (!result.IsSuccess)        {
            return BadRequest(ApiResponse<object>.Fail(result.Error, null, (int)System.Net.HttpStatusCode.BadRequest));
        }    

        return Ok(ApiResponse<UserWithoutManagerResponseDto?>.Success(result.Value, "User retrieved successfully", (int)System.Net.HttpStatusCode.OK));
    }

    [HttpPost(Name = "Create User")]
    [Tags("Users")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateAsync([FromBody] UserCreateRequestDto request)
    {
        var result = await _userService.CreateAsync(request);
        if (!result.IsSuccess)        {
            return BadRequest(ApiResponse<object>.Fail(result.Error, null, (int)System.Net.HttpStatusCode.BadRequest));
        }

        return Ok(ApiResponse<UserWithoutManagerResponseDto?>.Success(result.Value, "User created successfully", (int)System.Net.HttpStatusCode.OK));
    }
}