using LeaveFlowHR.Api.Common.Results;
using LeaveFlowHR.Api.Infrastructure.Database;
using LeaveFlowHR.Api.Modules.LeaveRequests.DTOs;
using LeaveFlowHR.Api.Modules.LeaveRequests.Entities;
using LeaveFlowHR.Api.Modules.LeaveRequests.Mappers;
using LeaveFlowHR.Api.Modules.Users.Enums;
using Microsoft.EntityFrameworkCore;

namespace LeaveFlowHR.Api.Modules.LeaveRequests.Services;

public interface ILeaveBalanceService
{
    Task<ServiceResult<LeaveBalanceResponseDto?>> GetLeaveBalanceByUserIdAndYearAsync(Guid userId, int year);
    Task<ServiceResult<LeaveBalanceSimpleResponseDto?>> CreateAsync(LeaveBalanceCreateRequestDto request);
}

public class LeaveBalanceService : ILeaveBalanceService
{
    private readonly AppDbContext _dbContext;

    public LeaveBalanceService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ServiceResult<LeaveBalanceResponseDto?>> GetLeaveBalanceByUserIdAndYearAsync(Guid userId, int year)
    {
        LeaveBalance? leaveBalance = await _dbContext.LeaveBalances
            .AsNoTracking()
            .Include(x => x.User)
            .Where(x => x.UserId == userId && x.Year == year && x.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (leaveBalance == null)
        {
            return ServiceResult<LeaveBalanceResponseDto?>.Failure("Leave balance not found");
        }

        return ServiceResult<LeaveBalanceResponseDto?>.Success(leaveBalance.ToLeaveBalanceResponseDto());
    }

    public async Task<ServiceResult<LeaveBalanceSimpleResponseDto?>> CreateAsync(LeaveBalanceCreateRequestDto request)
    {
        var existingLeaveBalance = await _dbContext.LeaveBalances
            .Where(x => x.UserId == request.UserId && x.Year == request.Year && x.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (existingLeaveBalance != null)
        {
            return ServiceResult<LeaveBalanceSimpleResponseDto?>.Failure("Leave balance already exists for the user and year");
        }

        var leaveBalance = new LeaveBalance
        {
            UserId = request.UserId,
            Year = request.Year,
            EntitledDays = request.EntitledDays,
            UsedDays = 0,
            PendingDays = 0
        };

        _dbContext.LeaveBalances.Add(leaveBalance);
        await _dbContext.SaveChangesAsync();

        return ServiceResult<LeaveBalanceSimpleResponseDto?>.Success(leaveBalance.ToLeaveBalanceSimpleResponseDto());
    }
}
