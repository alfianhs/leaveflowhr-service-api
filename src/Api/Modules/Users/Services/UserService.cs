using LeaveFlowHR.Api.Common.Dtos;
using LeaveFlowHR.Api.Common.Results;
using LeaveFlowHR.Api.Infrastructure.Database;
using LeaveFlowHR.Api.Modules.Users.DTOs;
using LeaveFlowHR.Api.Modules.Users.Entities;
using LeaveFlowHR.Api.Modules.Users.Enums;
using LeaveFlowHR.Api.Modules.Users.Mappers;
using Microsoft.EntityFrameworkCore;

namespace LeaveFlowHR.Api.Modules.Users.Services;

public interface IUserService
{
    Task<ServiceResult<PaginationResponseDto<UserWithoutManagerResponseDto>>> GetPaginatedListAsync(PaginationRequestDto request);
    Task<ServiceResult<UserWithoutManagerResponseDto?>> GetByIdAsync(Guid id);
    Task<ServiceResult<UserWithoutManagerResponseDto?>> CreateAsync(UserCreateRequestDto request);
}

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;

    public UserService(
        AppDbContext dbContext
    )
    {
        _dbContext = dbContext;
    }

    public async Task<ServiceResult<PaginationResponseDto<UserWithoutManagerResponseDto>>> GetPaginatedListAsync(PaginationRequestDto request)
    {
            List<UserWithoutManagerResponseDto> users = await _dbContext.Users
                .AsNoTracking()
                .Where(x => x.DeletedAt == null)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => x.ToUserWithoutManagerResponseDto())
                .ToListAsync();
    
            var paginationResponse = new PaginationResponseDto<UserWithoutManagerResponseDto>
            {
                Items = users,
                TotalItems = await _dbContext.Users.CountAsync(x => x.DeletedAt == null),
                Page = request.Page,
                PageSize = request.PageSize
            };

            return ServiceResult<PaginationResponseDto<UserWithoutManagerResponseDto>>.Success(paginationResponse);
    }

    public async Task<ServiceResult<UserWithoutManagerResponseDto?>> GetByIdAsync(Guid id)
    {
        UserWithoutManagerResponseDto? result = await _dbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == id && x.DeletedAt == null)
            .Select(x => x.ToUserWithoutManagerResponseDto())
            .FirstOrDefaultAsync();

        if (result == null)
        {
            return ServiceResult<UserWithoutManagerResponseDto?>.Failure("User not found");
        }

        return ServiceResult<UserWithoutManagerResponseDto?>.Success(result);
    }

    public async Task<ServiceResult<UserWithoutManagerResponseDto?>> CreateAsync(UserCreateRequestDto request)
    {
        // Check if user with email exists
        if (await _dbContext.Users.AsNoTracking().Where(x => x.Email == request.Email && x.DeletedAt == null).AnyAsync())
        {
            return ServiceResult<UserWithoutManagerResponseDto?>.Failure("User with this email already exists");
        }

        // Check if manager exists
        if (request.ManagerId != null)
        {
            if (!await _dbContext.Users.AsNoTracking().Where(x => x.Id == request.ManagerId && x.Role == EmployeeRole.MANAGER && x.DeletedAt == null).AnyAsync())
            {
                return ServiceResult<UserWithoutManagerResponseDto?>.Failure("Manager not found");
            }
        }

        // Create user
        User user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Department = request.Department,
            ManagerId = request.ManagerId,
            Role = request.Role
        };
        _dbContext.Users.Add(user);

        // Save
        await _dbContext.SaveChangesAsync();

        return ServiceResult<UserWithoutManagerResponseDto?>.Success(user.ToUserWithoutManagerResponseDto());
    }
}