using System.Net;
using LeaveFlowHR.Api.Common.Dtos;
using LeaveFlowHR.Api.Common.Responses;
using LeaveFlowHR.Api.Infrastructure.Extensions;
using LeaveFlowHR.Api.Modules.LeaveRequests.DTOs;
using LeaveFlowHR.Api.Modules.LeaveRequests.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveFlowHR.Api.Modules.LeaveRequests.Controllers;

[Route("api/approvals")]
[ApiController]
[Authorize(Roles = "MANAGER,HR")]
public class ApprovalController : Controller
{
    private readonly IApprovalService _approvalService;

    public ApprovalController(IApprovalService approvalService)
    {
        _approvalService = approvalService;
    }

    [HttpGet(Name = "Get Approval Paginated List")]
    [Tags("Approvals")]
    public async Task<IActionResult> GetPaginatedListAsync([FromQuery] ApprovalPagedRequestDto request)
    {
        Guid userId = User.GetUserId();
        string userRole = User.GetUserRole();

        var result = await _approvalService.GetPaginatedListAsync(request, userId, userRole);
        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponse<object>.Fail(result.Error, null, (int)HttpStatusCode.BadRequest));
        }

        return Ok(ApiResponse<List<ApprovalResponseDto>>.SuccessPaged(result.Value!.Items, result.Value.Page, result.Value.PageSize, result.Value.TotalItems, "Approvals retrieved successfully"));
    }

    [HttpGet("pending-count", Name = "Get Pending Approvals Count")]
    [Tags("Approvals")]
    public async Task<IActionResult> GetPendingApprovalsCountAsync()
    {
        Guid userId = User.GetUserId();
        string userRole = User.GetUserRole();

        var result = await _approvalService.GetPendingApprovalsCountAsync(userId, userRole);
        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponse<object>.Fail(result.Error, null, (int)HttpStatusCode.BadRequest));
        }

        return Ok(ApiResponse<int>.Success(result.Value, "Pending approvals count retrieved successfully", (int)HttpStatusCode.OK));
    }
}