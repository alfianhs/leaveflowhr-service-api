using LeaveFlowHR.Api.Common.Entities;
using LeaveFlowHR.Api.Modules.LeaveRequests.Entities;
using LeaveFlowHR.Api.Modules.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeaveFlowHR.Api.Infrastructure.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Auto Create index DeletedAt for all entities that inherit from BaseEntity
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var deletedAtProp = entityType.FindProperty("DeletedAt");
                if (deletedAtProp != null)
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .HasIndex("DeletedAt")
                        .HasDatabaseName($"IX_{entityType.ClrType.Name}_DeletedAt");
                }
            }
        }

        // Configure User
        modelBuilder.Entity<User>()
            .HasOne(u => u.Manager)
            .WithMany()
            .HasForeignKey(u => u.ManagerId)
            .OnDelete(DeleteBehavior.NoAction);

        // Configure LeaveRequest
        modelBuilder.Entity<LeaveRequest>()
            .HasOne(lr => lr.User)
            .WithMany()
            .HasForeignKey(lr => lr.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        // Configure LeaveBalance
        modelBuilder.Entity<LeaveBalance>()
            .HasOne(lb => lb.User)
            .WithMany()
            .HasForeignKey(lb => lb.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<LeaveBalance>()
            .HasIndex(lb => lb.Year)
            .HasDatabaseName("IX_LeaveBalance_Year");

        // Configure Approval
        modelBuilder.Entity<Approval>()
            .HasOne(a => a.LeaveRequest)
            .WithMany(lr => lr.Approvals)
            .HasForeignKey(a => a.LeaveRequestId)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<Approval>()
            .HasOne(a => a.Approver)
            .WithMany()
            .HasForeignKey(a => a.ApproverId)    
            .OnDelete(DeleteBehavior.NoAction);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<LeaveRequest> LeaveRequests { get; set; }
    public DbSet<LeaveBalance> LeaveBalances { get; set; }
    public DbSet<Approval> Approvals { get; set; }
}