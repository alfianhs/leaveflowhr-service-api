using System.Net;
using LeaveFlowHR.Api.Common.Dtos;
using LeaveFlowHR.Api.Common.Responses;
using LeaveFlowHR.Api.Infrastructure.Extensions;
using LeaveFlowHR.Api.Modules.LeaveRequests.DTOs;
using LeaveFlowHR.Api.Modules.LeaveRequests.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveFlowHR.Api.Modules.LeaveRequests.Controllers;

[Route("api/leave-requests")]
[ApiController]
public class LeaveRequestController : Controller
{
    private readonly ILeaveRequestService _leaveRequestService;

    public LeaveRequestController(ILeaveRequestService leaveRequestService)
    {
        _leaveRequestService = leaveRequestService;
    }

    [HttpGet(Name = "Get Leave Request Paginated List")]
    [Tags("Leave Requests")]
    [Authorize]
    public async Task<IActionResult> GetPaginatedListAsync([FromQuery] PaginationRequestDto request)
    {
        Guid userId = User.GetUserId();

        var result = await _leaveRequestService.GetPaginatedListAsync(request, userId);
        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponse<object>.Fail(result.Error, null, (int)HttpStatusCode.BadRequest));
        }

        return Ok(ApiResponse<List<LeaveRequestResponseDto>>.SuccessPaged(result.Value!.Items, result.Value.Page, result.Value.PageSize, result.Value.TotalItems, "Leave requests retrieved successfully"));
    }

    [HttpGet("{id}", Name = "Get Leave Request By Id")]
    [Tags("Leave Requests")]
    [Authorize]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var result = await _leaveRequestService.GetByIdAsync(id);
        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponse<object>.Fail(result.Error, null, (int)HttpStatusCode.BadRequest));
        }

        return Ok(ApiResponse<LeaveRequestResponseDto?>.Success(result.Value, "Leave request retrieved successfully", (int)HttpStatusCode.OK));
    }

    [HttpPost(Name = "Create Leave Request")]
    [Tags("Leave Requests")]
    [Authorize]
    public async Task<IActionResult> CreateAsync([FromBody] LeaveRequestCreateRequestDto request)
    {
        Guid userId = User.GetUserId();

        var result = await _leaveRequestService.CreateAsync(request, userId);
        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponse<object>.Fail(result.Error, null, (int)HttpStatusCode.BadRequest));
        }

        return Ok(ApiResponse<LeaveRequestSimpleWithoutUserResponseDto?>.Success(result.Value, "Leave request created successfully", (int)HttpStatusCode.Created));
    }

    [HttpPatch("{id}/cancel", Name = "Cancel Leave Request")]
    [Tags("Leave Requests")]
    [Authorize]
    public async Task<IActionResult> CancelAsync(Guid id)
    {
        Guid userId = User.GetUserId();
        string role = User.GetUserRole();

        var result = await _leaveRequestService.CancelAsync(id, userId, role);
        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponse<object>.Fail(result.Error, null, (int)HttpStatusCode.BadRequest));
        }

        return Ok(ApiResponse<LeaveRequestSimpleWithoutUserResponseDto?>.Success(result.Value, "Leave request cancelled successfully", (int)HttpStatusCode.OK));
    }

    [HttpPatch("{id}/approve", Name = "Approve Leave Request")]
    [Tags("Leave Requests")]
    [Authorize(Roles = "MANAGER,HR")]
    public async Task<IActionResult> ApproveAsync(Guid id, [FromBody] ApprovalRequestDto request)
    {
        Guid approverId = User.GetUserId();
        string approverRole = User.GetUserRole();

        var result = await _leaveRequestService.ApproveAsync(request, id, approverId, approverRole);
        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponse<object>.Fail(result.Error, null, (int)HttpStatusCode.BadRequest));
        }

        return Ok(ApiResponse<LeaveRequestSimpleWithoutUserResponseDto?>.Success(result.Value, "Leave request approved successfully", (int)HttpStatusCode.OK));
    }

    [HttpPatch("{id}/reject", Name = "Reject Leave Request")]
    [Tags("Leave Requests")]
    [Authorize(Roles = "MANAGER,HR")]
    public async Task<IActionResult> RejectAsync(Guid id, [FromBody] ApprovalRequestDto request)
    {
        Guid approverId = User.GetUserId();
        string approverRole = User.GetUserRole();

        var result = await _leaveRequestService.RejectAsync(request, id, approverId, approverRole);
        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponse<object>.Fail(result.Error, null, (int)HttpStatusCode.BadRequest));
        }

        return Ok(ApiResponse<LeaveRequestSimpleWithoutUserResponseDto?>.Success(result.Value, "Leave request rejected successfully", (int)HttpStatusCode.OK));
    }

    [HttpGet("sick-leave-used-days", Name = "Get Sick Leave Used Days Count")]
    [Tags("Leave Requests")]
    public async Task<IActionResult> GetSickLeaveUsedDaysCountAsync(int year)
    {
        Guid userId = User.GetUserId();

        // Default to current year if year is not provided or invalid
        if (year <= 0)
        {
            year = DateTime.UtcNow.Year;
        }

        var result = await _leaveRequestService.GetSickLeaveUsedDaysCountAsync(userId, year);
        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponse<object>.Fail(result.Error, null, (int)HttpStatusCode.BadRequest));
        }

        return Ok(ApiResponse<int>.Success(result.Value, "Sick leave used days count retrieved successfully", (int)HttpStatusCode.OK));
    }
}