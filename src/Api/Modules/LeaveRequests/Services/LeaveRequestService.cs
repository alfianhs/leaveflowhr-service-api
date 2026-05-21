using LeaveFlowHR.Api.Common.Dtos;
using LeaveFlowHR.Api.Common.Results;
using LeaveFlowHR.Api.Common.Utilites;
using LeaveFlowHR.Api.Infrastructure.Database;
using LeaveFlowHR.Api.Modules.LeaveRequests.DTOs;
using LeaveFlowHR.Api.Modules.LeaveRequests.Entities;
using LeaveFlowHR.Api.Modules.LeaveRequests.Enums;
using LeaveFlowHR.Api.Modules.LeaveRequests.Mappers;
using LeaveFlowHR.Api.Modules.Users.Entities;
using LeaveFlowHR.Api.Modules.Users.Enums;
using Microsoft.EntityFrameworkCore;

namespace LeaveFlowHR.Api.Modules.LeaveRequests.Services;

public interface ILeaveRequestService
{
    Task<ServiceResult<PaginationResponseDto<LeaveRequestResponseDto>>> GetPaginatedListAsync(PaginationRequestDto request, Guid userId);
    Task<ServiceResult<LeaveRequestResponseDto?>> GetByIdAsync(Guid id);
    Task<ServiceResult<LeaveRequestSimpleWithoutUserResponseDto?>> CreateAsync(LeaveRequestCreateRequestDto request, Guid userId);
    Task<ServiceResult<LeaveRequestSimpleWithoutUserResponseDto?>> CancelAsync(Guid id, Guid userId, string role);
    Task<ServiceResult<LeaveRequestSimpleWithoutUserResponseDto?>> ApproveAsync(ApprovalRequestDto request, Guid leaveRequestId, Guid approverId, string approverRole);
    Task<ServiceResult<LeaveRequestSimpleWithoutUserResponseDto?>> RejectAsync(ApprovalRequestDto request, Guid leaveRequestId, Guid approverId, string approverRole);
    Task<ServiceResult<int>> GetSickLeaveUsedDaysCountAsync(Guid userId, int year);
}

public class LeaveRequestService : ILeaveRequestService
{
    private readonly AppDbContext _dbContext;


    public LeaveRequestService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<ServiceResult<PaginationResponseDto<LeaveRequestResponseDto>>> GetPaginatedListAsync(PaginationRequestDto request, Guid userId)
    {
        List<LeaveRequestResponseDto> leaveRequests = await _dbContext.LeaveRequests
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.Approvals)!
            .ThenInclude(x => x.Approver)
            .Where(x => x.UserId == userId && x.DeletedAt == null)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => x.ToLeaveRequestResponseDto())
            .ToListAsync();

        var paginationResponse = new PaginationResponseDto<LeaveRequestResponseDto>
        {
            Items = leaveRequests,
            TotalItems = await _dbContext.LeaveRequests.CountAsync(x => x.UserId == userId && x.DeletedAt == null),
            Page = request.Page,
            PageSize = request.PageSize
        };
        
        return ServiceResult<PaginationResponseDto<LeaveRequestResponseDto>>.Success(paginationResponse);
    }

    public async Task<ServiceResult<LeaveRequestResponseDto?>> GetByIdAsync(Guid id)
    {
        LeaveRequestResponseDto? leaveRequest = await _dbContext.LeaveRequests
            .AsNoTracking()
            .Include(x => x.User)
            .Where(x => x.Id == id && x.DeletedAt == null)
            .Select(x => x.ToLeaveRequestResponseDto())
            .FirstOrDefaultAsync();

        if (leaveRequest == null)
        {
            return ServiceResult<LeaveRequestResponseDto?>.Failure("Leave request not found");
        }

        return ServiceResult<LeaveRequestResponseDto?>.Success(leaveRequest);
    }
    
    public async Task<ServiceResult<LeaveRequestSimpleWithoutUserResponseDto?>> CreateAsync(LeaveRequestCreateRequestDto request, Guid userId)
    {
        // Check if user exists
        User? user = await _dbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == userId && x.DeletedAt == null)
            .FirstOrDefaultAsync();
        if (user == null)
        {
            return ServiceResult<LeaveRequestSimpleWithoutUserResponseDto?>.Failure("User not found");
        }

        // Check user leave balance this year
        int currentYear = DateTime.UtcNow.Year;
        LeaveBalance? leaveBalance = await _dbContext.LeaveBalances
            .Where(x => x.UserId == userId && x.Year == currentYear && x.DeletedAt == null)
            .FirstOrDefaultAsync();
        if (leaveBalance == null)
        {
            return ServiceResult<LeaveRequestSimpleWithoutUserResponseDto?>.Failure("Leave balance not found");
        }

        // Check if user has enough leave balance
        int requestedDays = DateUtility.DifferenceInBusinessDays(request.StartDate, request.EndDate);
        if (requestedDays > (leaveBalance.EntitledDays - leaveBalance.UsedDays - leaveBalance.PendingDays))
        {
            return ServiceResult<LeaveRequestSimpleWithoutUserResponseDto?>.Failure("Not enough leave balance");
        }

        // Check if user has overlapping request
        bool hasOverlappingRequest = await _dbContext.LeaveRequests
        .AsNoTracking()
        .Where(x => x.UserId == userId
                && x.DeletedAt == null
                && (x.Status == LeaveRequestStatus.APPROVED || x.Status == LeaveRequestStatus.PENDING)
                && x.StartDate <= request.EndDate
                && x.EndDate >= request.StartDate)
        .AnyAsync();
        if (hasOverlappingRequest)
        {
            return ServiceResult<LeaveRequestSimpleWithoutUserResponseDto?>.Failure("You already have an approved leave request on the selected dates");
        }

        // Create leave request
        LeaveRequest newLeaveRequest = new LeaveRequest
        {
            UserId = userId,
            Type = request.Type,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalDays = requestedDays,
            Reason = request.Reason,
            Status = LeaveRequestStatus.PENDING
        };
        _dbContext.LeaveRequests.Add(newLeaveRequest);

        // Update leave balance pending days
        leaveBalance.PendingDays += requestedDays;
        leaveBalance.UpdatedAt = DateTime.UtcNow;
        _dbContext.LeaveBalances.Update(leaveBalance);

        // Check user manager if null direct approval to HR
        if (user.ManagerId == null)
        {
            // Direct approval to HR
            Approval hrApproval = new Approval
            {
                LeaveRequest = newLeaveRequest,
                ApproverId = null,
                ApproverRole = EmployeeRole.HR,
                Decision = null,
                Note = null
            };
            _dbContext.Approvals.Add(hrApproval);
        }
        else
        {
            // Approval to manager first
            Approval managerApproval = new Approval
            {
                LeaveRequest = newLeaveRequest,
                ApproverId = user.ManagerId.Value,
                ApproverRole = EmployeeRole.MANAGER,
                Decision = null,
                Note = null
            };
            _dbContext.Approvals.Add(managerApproval);
        }

        // Save
        await _dbContext.SaveChangesAsync();
        
        // Return leave request
        return ServiceResult<LeaveRequestSimpleWithoutUserResponseDto?>.Success(newLeaveRequest.ToLeaveRequestSimpleWithoutUserResponseDto());
    }

    public async Task<ServiceResult<LeaveRequestSimpleWithoutUserResponseDto?>> CancelAsync(Guid id, Guid userId, string role)
    {
        // Check if leave request exists
        LeaveRequest? leaveRequest = await _dbContext.LeaveRequests
            .Include(x => x.User)
            .Where(x => x.Id == id && x.DeletedAt == null)
            .FirstOrDefaultAsync();
        if (leaveRequest == null)
        {
            return ServiceResult<LeaveRequestSimpleWithoutUserResponseDto?>.Failure("Leave request not found");
        }

        // Check if user is owner of the leave request or HR
        if (leaveRequest.UserId != userId && role != EmployeeRole.HR.ToString())
        {
            return ServiceResult<LeaveRequestSimpleWithoutUserResponseDto?>.Failure("You are not authorized to cancel this leave request");
        }

         // Only pending leave request can be cancelled
        if (leaveRequest.Status != LeaveRequestStatus.PENDING)
        {
            return ServiceResult<LeaveRequestSimpleWithoutUserResponseDto?>.Failure("Only pending leave request can be cancelled");
        }

        // Update leave request status to cancelled
        leaveRequest.Status = LeaveRequestStatus.CANCELLED;
        leaveRequest.UpdatedAt = DateTime.UtcNow;
        _dbContext.LeaveRequests.Update(leaveRequest);

        // Update leave balance pending days and used days
        int currentYear = DateTime.UtcNow.Year;
        LeaveBalance? leaveBalance = await _dbContext.LeaveBalances
            .Where(x => x.UserId == leaveRequest.UserId && x.Year == currentYear && x.DeletedAt == null)
            .FirstOrDefaultAsync();
        if (leaveBalance != null)
        {
            leaveBalance.PendingDays -= leaveRequest.TotalDays;
            leaveBalance.UpdatedAt = DateTime.UtcNow;
            _dbContext.LeaveBalances.Update(leaveBalance);
        }

        // Delete non decided approvals related to this leave request
        List<Approval> approvals = await _dbContext.Approvals
            .Where(x => x.LeaveRequestId == leaveRequest.Id && x.Decision == null && x.DeletedAt == null)
            .ToListAsync();
        foreach (var approval in approvals)
        {
            approval.DeletedAt = DateTime.UtcNow;
        }

        // Save
        await _dbContext.SaveChangesAsync();

        // Return leave request
        return ServiceResult<LeaveRequestSimpleWithoutUserResponseDto?>.Success(leaveRequest.ToLeaveRequestSimpleWithoutUserResponseDto());
    }

    public async Task<ServiceResult<LeaveRequestSimpleWithoutUserResponseDto?>> ApproveAsync(ApprovalRequestDto request, Guid leaveRequestId, Guid approverId, string approverRole)
    {
        // Check if leave request exists
        LeaveRequest? leaveRequest = await _dbContext.LeaveRequests
            .Include(x => x.User)
            .Where(x => x.Id == leaveRequestId && x.DeletedAt == null)
            .FirstOrDefaultAsync();
        if (leaveRequest == null)
        {
            return ServiceResult<LeaveRequestSimpleWithoutUserResponseDto?>.Failure("Leave request not found");
        }

        // Check if approval exists for this approver
        Approval? approval = await _dbContext.Approvals
            .Where(x => x.LeaveRequestId == leaveRequestId && (x.ApproverId == null || x.ApproverId == approverId) && x.ApproverRole.ToString() == approverRole && x.DeletedAt == null)
            .FirstOrDefaultAsync();
        if (approval == null)
        {
            return ServiceResult<LeaveRequestSimpleWithoutUserResponseDto?>.Failure("Approval not found for this approver");
        }

        // Update approval decision and note
        approval.ApproverId = approverId;
        approval.Decision = ApprovalDecisionEnum.APPROVED;
        approval.Note = request.Note;
        approval.UpdatedAt = DateTime.UtcNow;
        _dbContext.Approvals.Update(approval);

        // If approver is hr, update leave request status to approved and update leave balance pending days and used days
        if (approverRole == EmployeeRole.HR.ToString())
        {
            leaveRequest.Status = LeaveRequestStatus.APPROVED;
            leaveRequest.UpdatedAt = DateTime.UtcNow;
            _dbContext.LeaveRequests.Update(leaveRequest);

            int currentYear = DateTime.UtcNow.Year;
            LeaveBalance? leaveBalance = await _dbContext.LeaveBalances
                .Where(x => x.UserId == leaveRequest.UserId && x.Year == currentYear && x.DeletedAt == null)
                .FirstOrDefaultAsync();
            if (leaveBalance != null)
            {
                leaveBalance.PendingDays -= leaveRequest.TotalDays;
                leaveBalance.UsedDays += leaveRequest.TotalDays;
                leaveBalance.UpdatedAt = DateTime.UtcNow;
                _dbContext.LeaveBalances.Update(leaveBalance);
            }
        }
        else
        {
            // If approver is manager, create approval for hr
            Approval hrApproval = new Approval
            {
                LeaveRequestId = leaveRequestId,
                ApproverId = null,
                ApproverRole = EmployeeRole.HR,
                Decision = null,
                Note = null
            };
            _dbContext.Approvals.Add(hrApproval);
        }

        // Save
        await _dbContext.SaveChangesAsync();

        return ServiceResult<LeaveRequestSimpleWithoutUserResponseDto?>.Success(leaveRequest.ToLeaveRequestSimpleWithoutUserResponseDto());
    }

    public async Task<ServiceResult<LeaveRequestSimpleWithoutUserResponseDto?>> RejectAsync(ApprovalRequestDto request, Guid leaveRequestId, Guid approverId, string approverRole)
    {
        // Check if leave request exists
        LeaveRequest? leaveRequest = await _dbContext.LeaveRequests
            .Where(x => x.Id == leaveRequestId && x.DeletedAt == null)
            .FirstOrDefaultAsync();
        if (leaveRequest == null)
        {
            return ServiceResult<LeaveRequestSimpleWithoutUserResponseDto?>.Failure("Leave request not found");
        }

        // Check if approval exists for this approver
        Approval? approval = await _dbContext.Approvals
            .Where(x => x.LeaveRequestId == leaveRequestId && (x.ApproverId == null || x.ApproverId == approverId) && x.ApproverRole.ToString() == approverRole && x.DeletedAt == null)
            .FirstOrDefaultAsync();
        if (approval == null)
        {
            return ServiceResult<LeaveRequestSimpleWithoutUserResponseDto?>.Failure("Approval not found for this approver");
        }

        // Update approval decision and note
        approval.Decision = ApprovalDecisionEnum.REJECTED;
        approval.Note = request.Note;
        approval.UpdatedAt = DateTime.UtcNow;
        _dbContext.Approvals.Update(approval);

        // Update leave request status to rejected
        leaveRequest.Status = LeaveRequestStatus.REJECTED;
        leaveRequest.UpdatedAt = DateTime.UtcNow;
        _dbContext.LeaveRequests.Update(leaveRequest);

        // Update leave balance pending days and used days
        int currentYear = DateTime.UtcNow.Year;
        LeaveBalance? leaveBalance = await _dbContext.LeaveBalances
            .Where(x => x.UserId == leaveRequest.UserId && x.Year == currentYear && x.DeletedAt == null)
            .FirstOrDefaultAsync();
        if (leaveBalance != null)
        {
            leaveBalance.PendingDays -= leaveRequest.TotalDays;
            leaveBalance.UpdatedAt = DateTime.UtcNow;
            _dbContext.LeaveBalances.Update(leaveBalance);
        }

        // Save
        await _dbContext.SaveChangesAsync();

        return ServiceResult<LeaveRequestSimpleWithoutUserResponseDto?>.Success(leaveRequest.ToLeaveRequestSimpleWithoutUserResponseDto());
    }

    public async Task<ServiceResult<int>> GetSickLeaveUsedDaysCountAsync(Guid userId, int year)
    {
        int usedSickLeaveDays = await _dbContext.LeaveRequests
            .Where(x => x.UserId == userId && x.Type == LeaveType.SICK && x.StartDate.Year == year && x.Status == LeaveRequestStatus.APPROVED && x.DeletedAt == null)
            .SumAsync(x => (int?)x.TotalDays) ?? 0;

        return ServiceResult<int>.Success(usedSickLeaveDays);
    }
}