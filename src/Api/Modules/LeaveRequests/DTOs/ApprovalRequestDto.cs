using LeaveFlowHR.Api.Common.Dtos;
using LeaveFlowHR.Api.Modules.LeaveRequests.Enums;

namespace LeaveFlowHR.Api.Modules.LeaveRequests.DTOs;

public class ApprovalRequestDto
{
    public string? Note { get; set; }
}

public class ApprovalPagedRequestDto : PaginationRequestDto
{
    public bool IsReviewed { get; set; }
}
