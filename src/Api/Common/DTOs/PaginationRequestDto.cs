namespace LeaveFlowHR.Api.Common.Dtos;

public class PaginationRequestDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}