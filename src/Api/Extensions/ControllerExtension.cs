using System.Text.Json;
using System.Text.Json.Serialization;
using LeaveFlowHR.Api.Common.Responses;
using Microsoft.AspNetCore.Mvc;

namespace LeaveFlowHR.Api.Extensions;

public static class ControllerExtensions
{
    public static IServiceCollection AddApiControllers(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            // using fluent validation, ignore the implicit [Required] attribute on non-nullable reference types
            options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value!.Errors.Count > 0)
                    .ToDictionary(
                        kvp => JsonNamingPolicy.CamelCase.ConvertName(kvp.Key),
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                var response = ApiResponse<object>.Fail("Validation failed", errors, StatusCodes.Status422UnprocessableEntity);

                return new UnprocessableEntityObjectResult(response);
            };
        });

        return services;
    }
}