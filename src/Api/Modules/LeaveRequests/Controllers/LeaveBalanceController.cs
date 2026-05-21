using LeaveFlowHR.Api.Common.Responses;
using LeaveFlowHR.Api.Infrastructure.Extensions;
using LeaveFlowHR.Api.Modules.LeaveRequests.DTOs;
using LeaveFlowHR.Api.Modules.LeaveRequests.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveFlowHR.Api.Modules.LeaveRequests.Controllers;

[Route("api/leave-balances")]
[ApiController]
public class LeaveBalanceController : Controller
{
    private readonly ILeaveBalanceService _leaveBalanceService;
    
    public LeaveBalanceController(ILeaveBalanceService leaveBalanceService)
    {
        _leaveBalanceService = leaveBalanceService;
    }

    [HttpGet("get-by-year", Name = "Get Leave Balance By Year")]
    [Tags("Leave Balances")]
    [Authorize]
    public async Task<IActionResult> GetByYearAsync([FromQuery] int year)
    {
        Guid userId = User.GetUserId();

        // Default to current year if year is not provided or invalid
        if (year <= 0)
        {
            year = DateTime.UtcNow.Year;
        }

        var result = await _leaveBalanceService.GetLeaveBalanceByUserIdAndYearAsync(userId, year);
        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponse<object>.Fail(result.Error, null, (int)System.Net.HttpStatusCode.BadRequest));
        }

        return Ok(ApiResponse<LeaveBalanceResponseDto?>.Success(result.Value, "Leave balance retrieved successfully", (int)System.Net.HttpStatusCode.OK));
    }

    [HttpPost(Name = "Create Leave Balance")]
    [Tags("Leave Balances")]
    [Authorize(Roles = "HR")]
    public async Task<IActionResult> CreateAsync([FromBody] LeaveBalanceCreateRequestDto request)
    {
        var result = await _leaveBalanceService.CreateAsync(request);
        if (!result.IsSuccess)        {
            return BadRequest(ApiResponse<object>.Fail(result.Error, null, (int)System.Net.HttpStatusCode.BadRequest));
        }

        return Ok(ApiResponse<LeaveBalanceSimpleResponseDto?>.Success(result.Value, "Leave balance created successfully", (int)System.Net.HttpStatusCode.OK));
    }
}