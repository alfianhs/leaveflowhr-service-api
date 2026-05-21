using LeaveFlowHR.Api.Common.Dtos;
using LeaveFlowHR.Api.Modules.LeaveRequests.DTOs;
using LeaveFlowHR.Api.Modules.LeaveRequests.Entities;
using LeaveFlowHR.Api.Modules.LeaveRequests.Enums;
using LeaveFlowHR.Api.Modules.Users.Mappers;
using Microsoft.OpenApi.Extensions;

namespace LeaveFlowHR.Api.Modules.LeaveRequests.Mappers;

public static class LeaveRequestMapper
{
    public static LeaveRequestResponseDto ToLeaveRequestResponseDto(this LeaveRequest leaveRequest)
    {
        var lastReviewedApproval = leaveRequest.Approvals
        .Where(x => x.Decision != null)
        .OrderByDescending(x => x.UpdatedAt)
        .FirstOrDefault();

        return new LeaveRequestResponseDto
        {
            Id = leaveRequest.Id,
            UserId = leaveRequest.UserId,
            User = leaveRequest.User.ToUserSimpleResponseDto(),
            Type = new EnumResponseDto
            {
                Key = (int)leaveRequest.Type,
                Value = leaveRequest.Type.GetDisplayName()
            },
            StartDate = leaveRequest.StartDate,
            EndDate = leaveRequest.EndDate,
            TotalDays = leaveRequest.TotalDays,
            Reason = leaveRequest.Reason,
            Status = new EnumResponseDto
            {
                Key = (int)leaveRequest.Status,
                Value = leaveRequest.Status.GetDisplayName()
            },
            LastReview = lastReviewedApproval != null ? new LastReviewDto
            {
                By = lastReviewedApproval?.Approver?.Name,
                Note = lastReviewedApproval?.Note
            } : null,
            CreatedAt = leaveRequest.CreatedAt,
            UpdatedAt = leaveRequest.UpdatedAt
        };
    }

    public static LeaveRequestSimpleWithUserResponseDto ToLeaveRequestSimpleWithUserResponseDto(this LeaveRequest leaveRequest)
    {
        return new LeaveRequestSimpleWithUserResponseDto
        {
            Id = leaveRequest.Id,
            UserId = leaveRequest.UserId,
            User = leaveRequest.User.ToUserSimpleResponseDto(),
            Type = new EnumResponseDto
            {
                Key = (int)leaveRequest.Type,
                Value = leaveRequest.Type.GetDisplayName()
            },
            StartDate = leaveRequest.StartDate,
            EndDate = leaveRequest.EndDate,
            TotalDays = leaveRequest.TotalDays,
            Reason = leaveRequest.Reason,
            CreatedAt = leaveRequest.CreatedAt
        };
    }

    public static LeaveRequestSimpleWithoutUserResponseDto ToLeaveRequestSimpleWithoutUserResponseDto(this LeaveRequest leaveRequest)
    {
        return new LeaveRequestSimpleWithoutUserResponseDto
        {
            Id = leaveRequest.Id,
            UserId = leaveRequest.UserId,
            Type = new EnumResponseDto
            {
                Key = (int)leaveRequest.Type,
                Value = leaveRequest.Type.GetDisplayName()
            },
            StartDate = leaveRequest.StartDate,
            EndDate = leaveRequest.EndDate,
            Reason = leaveRequest.Reason
        };
    }
}