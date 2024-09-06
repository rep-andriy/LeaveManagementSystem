using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole 
                { 
                    Id = "6d9ed3ff-bebb-42bc-ad07-0255bb0f7edb",
                    Name = "Employee", 
                    NormalizedName = "EMPLOYEE"
                }, 
                new IdentityRole 
                {
                    Id = "cc4fcb01-de88-4c20-b4ac-8df5c2a65160",
                    Name = "Supervisor",
                    NormalizedName = "SUPERVISOR"
                }, 
                new IdentityRole 
                {
                    Id = "e9f639de-624f-4a4e-b8bf-2381725462f1",
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR"
                }
                
            );

            var hasher = new PasswordHasher<ApplicationUser>();
            builder.Entity<ApplicationUser>()
                .HasData(new ApplicationUser
                {
                    Id = "408aa945-3d84-4421-8342-7269ec64d949",
                    Email = "admin@localhost.com",
                    NormalizedEmail = "ADMIN@LOCALHOST.COM",
                    UserName = "admin@localhost.com",
                    PasswordHash = hasher.HashPassword(null, "P@ssword123"),
                    EmailConfirmed = true,
                    FirstName = "Default",
                    LastName = "Admin",
                    DateOfBirth = new DateOnly(1950, 12, 01)
                });

            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> 
                { 
                    RoleId = "e9f639de-624f-4a4e-b8bf-2381725462f1",
                    UserId = "408aa945-3d84-4421-8342-7269ec64d949",
                });
            
        }

        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeaveAllocation> LeaveAllocations { get; set; }
        public DbSet<Period> Periods { get; set; }
    }
}
