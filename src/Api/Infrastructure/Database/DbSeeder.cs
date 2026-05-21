using LeaveFlowHR.Api.Modules.LeaveRequests.Entities;
using LeaveFlowHR.Api.Modules.Users.Entities;
using LeaveFlowHR.Api.Modules.Users.Enums;
using Microsoft.EntityFrameworkCore;

namespace LeaveFlowHR.Api.Infrastructure.Database;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        var users = new List<User>()
        {
            new User
            {
                Name = "Admin",
                Email = "admin@mail.com",
                Department = "Administration",
                Role = EmployeeRole.ADMIN,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("securepassword")
            },
            new User
            {
                Name = "HR",
                Email = "hr@mail.com",
                Department = "Human Resources",
                Role = EmployeeRole.HR,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("securepassword")
            },
            new User
            {
                Name = "Manager",
                Email = "manager@mail.com",
                Department = "IT",
                Role = EmployeeRole.MANAGER,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("securepassword")
            },
        };
        var employee = new User
        {
            Name = "Employee",
            Email = "employee@mail.com",
            ManagerId = users[2].Id,
            Department = "IT",
            Role = EmployeeRole.EMPLOYEE,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("securepassword")
        };
        users.Add(employee);
        context.Users.AddRange(users);

        var leaveBalances = new List<LeaveBalance>()
        {
            new LeaveBalance
            {
              UserId = users[1].Id,
              EntitledDays = 15,
              Year = DateTime.UtcNow.Year
            },
            new LeaveBalance
            {
              UserId = users[2].Id,
              EntitledDays = 15,
              Year = DateTime.UtcNow.Year
            },
            new LeaveBalance
            {
              UserId = users[3].Id,
              EntitledDays = 15,
              Year = DateTime.UtcNow.Year
            },
        };
        context.LeaveBalances.AddRange(leaveBalances);

        await context.SaveChangesAsync();
    }
}