using LeaveFlowHR.Api.Common.Dtos;
using LeaveFlowHR.Api.Modules.Users.DTOs;

namespace LeaveFlowHR.Api.Modules.LeaveRequests.DTOs;

public class LeaveRequestResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public UserSimpleResponseDto User { get; set; } = null!;
    public EnumResponseDto Type { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int TotalDays { get; set; }
    public string? Reason { get; set; }
    public EnumResponseDto Status { get; set; } = null!;
    public LastReviewDto? LastReview { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class LeaveRequestSimpleWithUserResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public UserSimpleResponseDto User { get; set; } = null!;
    public EnumResponseDto Type { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int TotalDays { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class LeaveRequestSimpleWithoutUserResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public EnumResponseDto Type { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string? Reason { get; set; }
}
