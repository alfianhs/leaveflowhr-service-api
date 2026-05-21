using LeaveFlowHR.Api.Common.Dtos;
using LeaveFlowHR.Api.Common.Results;
using LeaveFlowHR.Api.Infrastructure.Database;
using LeaveFlowHR.Api.Modules.LeaveRequests.DTOs;
using LeaveFlowHR.Api.Modules.LeaveRequests.Mappers;
using LeaveFlowHR.Api.Modules.Users.Enums;
using Microsoft.EntityFrameworkCore;

namespace LeaveFlowHR.Api.Modules.LeaveRequests.Services;

public interface IApprovalService
{
    Task<ServiceResult<PaginationResponseDto<ApprovalResponseDto>>> GetPaginatedListAsync(ApprovalPagedRequestDto request, Guid approverId, string approverRole);
    Task<ServiceResult<int>> GetPendingApprovalsCountAsync(Guid approverId, string approverRole);
}

public class ApprovalService : IApprovalService
{
    private readonly AppDbContext _dbContext;

    public ApprovalService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ServiceResult<PaginationResponseDto<ApprovalResponseDto>>> GetPaginatedListAsync(ApprovalPagedRequestDto request, Guid approverId, string approverRole)
    {
        List<ApprovalResponseDto> approvals = await _dbContext.Approvals
            .AsNoTracking()
            .Include(x => x.Approver)
            .Include(x => x.LeaveRequest)
            .ThenInclude(x => x.User)
            .Where(x => 
                (x.ApproverId == null || x.ApproverId == approverId) 
                && x.ApproverRole.ToString() == approverRole 
                && (request.IsReviewed ? x.Decision != null : x.Decision == null)
                && (approverRole == EmployeeRole.HR.ToString() ? x.LeaveRequest.UserId != approverId : true)
                && x.DeletedAt == null
            )
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => x.ToApprovalResponseDto())
            .ToListAsync();

        var paginationResponse = new PaginationResponseDto<ApprovalResponseDto>
        {
            Items = approvals,
            TotalItems = await _dbContext.Approvals.CountAsync(x => 
                (x.ApproverId == null || x.ApproverId == approverId) 
                && x.ApproverRole.ToString() == approverRole 
                && (request.IsReviewed ? x.Decision != null : x.Decision == null)
                && (approverRole == EmployeeRole.HR.ToString() ? x.LeaveRequest.UserId != approverId : true)
                && x.DeletedAt == null
            ),
            Page = request.Page,
            PageSize = request.PageSize
        };
        
        return ServiceResult<PaginationResponseDto<ApprovalResponseDto>>.Success(paginationResponse);
    }

    public async Task<ServiceResult<int>> GetPendingApprovalsCountAsync(Guid approverId, string approverRole)
    {
        int count = await _dbContext.Approvals
            .AsNoTracking()
            .Where(x => 
                (x.ApproverId == null || x.ApproverId == approverId) 
                && x.ApproverRole.ToString() == approverRole 
                && x.Decision == null 
                && (approverRole == EmployeeRole.HR.ToString() ? x.LeaveRequest.UserId != approverId : true)
                && x.DeletedAt == null)
            .CountAsync();
        
        return ServiceResult<int>.Success(count);
    }
}