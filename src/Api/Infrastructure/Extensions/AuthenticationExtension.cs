using System.Text;
using LeaveFlowHR.Api.Common.Responses;
using LeaveFlowHR.Api.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace LeaveFlowHR.Api.Infrastructure.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        AppConfiguration appConfiguration)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = appConfiguration.Jwt.Issuer,
                    ValidAudience = appConfiguration.Jwt.Audience,

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(appConfiguration.Jwt.SecretKey))
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var result = ApiResponse<object>.Error(
                            "Unauthorized: token is missing or invalid",
                            StatusCodes.Status401Unauthorized);

                        return context.Response.WriteAsJsonAsync(result);
                    },

                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        var result = ApiResponse<object>.Error(
                            "Forbidden: token is missing or invalid",
                            StatusCodes.Status403Forbidden);

                        return context.Response.WriteAsJsonAsync(result);
                    }
                };
            });

        return services;
    }
}