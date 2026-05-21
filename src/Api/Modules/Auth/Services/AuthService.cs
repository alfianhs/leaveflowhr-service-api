using LeaveFlowHR.Api.Common.Results;
using LeaveFlowHR.Api.Infrastructure.Database;
using LeaveFlowHR.Api.Infrastructure.Services;
using LeaveFlowHR.Api.Modules.Auth.DTOs;
using LeaveFlowHR.Api.Modules.Users.Entities;
using LeaveFlowHR.Api.Modules.Users.Mappers;
using Microsoft.EntityFrameworkCore;
using UserDto = LeaveFlowHR.Api.Modules.Users.DTOs;

namespace LeaveFlowHR.Api.Modules.Auth.Services;

public interface IAuthService
{
    Task<ServiceResult<LoginResponseDto?>> LoginAsync(LoginRequestDto request);
    Task<ServiceResult<UserDto.UserWithManagerResponseDto?>> GetMeAsync(Guid userId);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IJwtService _jwtService;

    public AuthService(
        AppDbContext dbContext,
        IJwtService jwtService
    )
    {
        _dbContext = dbContext;
        _jwtService = jwtService;
    }

    public async Task<ServiceResult<LoginResponseDto?>> LoginAsync(LoginRequestDto request)
    {
        // Check if user with email exists
        User? user = await _dbContext.Users.AsNoTracking().Where(x => x.Email == request.Email && x.DeletedAt == null).FirstOrDefaultAsync();
        if (user == null)
        {
            return ServiceResult<LoginResponseDto?>.Failure("Invalid Email or Password");
        }

        // Check if password is correct
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return ServiceResult<LoginResponseDto?>.Failure("Invalid Email or Password");
        }

        DateTime now = DateTime.UtcNow;

        return ServiceResult<LoginResponseDto?>.Success(new LoginResponseDto
        {
            AccessToken = _jwtService.GenerateToken(user.Id, user.Role.ToString(), now),
            ExpiredAt = _jwtService.GetTokenExpiredAt(now),
            User = user.ToUserWithoutManagerResponseDto()
        });
    }

    public async Task<ServiceResult<UserDto.UserWithManagerResponseDto?>> GetMeAsync(Guid userId)
    {
        User? user = await _dbContext.Users
            .AsNoTracking()
            .Include(x => x.Manager)
            .Where(x => x.Id == userId && x.DeletedAt == null)
            .FirstOrDefaultAsync();
        if (user == null)
        {
            return ServiceResult<UserDto.UserWithManagerResponseDto?>.Failure("User not found");
        }

        return ServiceResult<UserDto.UserWithManagerResponseDto?>.Success(user.ToUserWithManagerResponseDto());
    }
}