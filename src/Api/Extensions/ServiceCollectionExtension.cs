using LeaveFlowHR.Api.Modules.Auth.Services;
using LeaveFlowHR.Api.Modules.LeaveRequests.Services;
using LeaveFlowHR.Api.Modules.Users.Services;

namespace LeaveFlowHR.Api.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Services from modules
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ILeaveRequestService, LeaveRequestService>();
        services.AddScoped<ILeaveBalanceService, LeaveBalanceService>();
        services.AddScoped<IApprovalService, ApprovalService>();
        
        return services;
    }
}