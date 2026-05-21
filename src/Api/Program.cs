using LeaveFlowHR.Api.Extensions;
using LeaveFlowHR.Api.Common.Responses;
using LeaveFlowHR.Api.Configurations;
using LeaveFlowHR.Api.Infrastructure.Database;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using LeaveFlowHR.Api.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// App Configuration
var appConfiguration = builder.Configuration.Get<AppConfiguration>() ?? throw new InvalidOperationException("App Configuration not found");
builder.Services.Configure<AppConfiguration>(builder.Configuration);

// DB Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(appConfiguration.ConnectionStrings.DefaultConnection));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocs();

// Controllers
builder.Services.AddApiControllers();

// App Services
builder.Services.AddApplicationServices();

// Infrastructure Services
builder.Services.AddInfrastructureServices();

// Fluent Validation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Authentication JWT
builder.Services.AddJwtAuthentication(appConfiguration);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("dev", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

var app = builder.Build();

// Development only: Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
}

// ApiExceptionHandler
app.ApiExceptionHandler();

// Seed Database
await app.SeedDatabaseAsync();

// CORS
app.UseCors("dev");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == StatusCodes.Status404NotFound && !context.Response.HasStarted)
    {
        context.Response.ContentType = "application/json";

        var response = ApiResponse<object>.Fail("Resource not found", null, StatusCodes.Status404NotFound);

        await context.Response.WriteAsJsonAsync(response);
    }
});

app.Run();
