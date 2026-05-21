using LeaveFlowHR.Api.Common.Dtos;
using LeaveFlowHR.Api.Modules.LeaveRequests.DTOs;
using LeaveFlowHR.Api.Modules.LeaveRequests.Entities;
using LeaveFlowHR.Api.Modules.Users.DTOs;
using LeaveFlowHR.Api.Modules.Users.Mappers;
using Microsoft.OpenApi.Extensions;

namespace LeaveFlowHR.Api.Modules.LeaveRequests.Mappers;

public static class ApprovalMapper
{
    public static ApprovalResponseDto ToApprovalResponseDto(this Approval approval)
    {
        return new ApprovalResponseDto
        {
            Id = approval.Id,
            LeaveRequestId = approval.LeaveRequestId,
            LeaveRequest = approval.LeaveRequest.ToLeaveRequestSimpleWithUserResponseDto(),
            ApproverId = approval.ApproverId,
            Approver = approval.Approver != null ? approval.Approver.ToUserSimpleResponseDto() : null,
            Decision = approval.Decision != null ? new EnumResponseDto
            {
                Key = (int)approval.Decision,
                Value = approval.Decision.Value.GetDisplayName()
            } : null,
            Note = approval.Note,
            CreatedAt = approval.CreatedAt,
            UpdatedAt = approval.UpdatedAt
        };
    }
}