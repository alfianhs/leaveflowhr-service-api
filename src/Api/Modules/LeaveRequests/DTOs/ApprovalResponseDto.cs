using LeaveFlowHR.Api.Common.Dtos;
using LeaveFlowHR.Api.Modules.Users.DTOs;

namespace LeaveFlowHR.Api.Modules.LeaveRequests.DTOs;

public class ApprovalResponseDto
{
    public Guid Id { get; set; }
    public Guid LeaveRequestId { get; set; }
    public LeaveRequestSimpleWithUserResponseDto LeaveRequest { get; set; } = null!;
    public Guid? ApproverId { get; set; }
    public UserSimpleResponseDto? Approver { get; set; }
    public EnumResponseDto? Decision { get; set; } = null!;
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class LastReviewDto
{
    public string? By { get; set; }
    public string? Note { get; set; }
}
