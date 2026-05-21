using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using LeaveFlowHR.Api.Common.Responses;

namespace LeaveFlowHR.Api.Extensions;

public static class ExceptionHandlerExtension
{
    public static IApplicationBuilder ApiExceptionHandler(this IApplicationBuilder app)
    {
        var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

        app.UseExceptionHandler(error =>
        {
            error.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerFeature>();
                var request = context.Features.Get<IHttpRequestFeature>();
                var exception = feature?.Error ?? new Exception("An unexpected error occurred.");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                if (env.IsProduction())
                {
                    var response = ApiResponse<object>.Error("Something went wrong", context.Response.StatusCode);
                    await context.Response.WriteAsJsonAsync(response);
                }
                else
                {
                    var response = ApiResponse<object>.Error(exception.Message, context.Response.StatusCode, request?.Path, exception.StackTrace);
                    await context.Response.WriteAsJsonAsync(response);
                }
            });
        });

        return app;
    }
}