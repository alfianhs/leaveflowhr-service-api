namespace LeaveFlowHR.Api.Common.Dtos;

public class PaginationResponseDto<T>
{
    public List<T> Items { get; set; } = null!;
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}