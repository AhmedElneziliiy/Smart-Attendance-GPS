using Microsoft.AspNetCore.Identity;
using Smart_Attendance_GPS.Models;
using Smart_Attendance_GPS.Models.Context;

namespace Smart_Attendance_GPS.Seeders
{
    public class DbSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public DbSeeder(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            // Seed roles if they don't exist
            await SeedRolesAsync();

            // Seed an Admin user and 2 Employees if no users exist
            await SeedUsersAsync();
        }

        private async Task SeedRolesAsync()
        {
            if (!_context.Roles.Any())  // Check if roles already exist
            {
                // Create Roles: Admin and Employee
                var roles = new string[] { "Admin", "Employee" };
                foreach (var role in roles)
                {
                    var roleExist = await _roleManager.RoleExistsAsync(role);
                    if (!roleExist)
                    {
                        await _roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }
        }

        private async Task SeedUsersAsync()
        {
            if (!_context.Users.Any())  // Check if there are any users
            {
                // Create Admin user
                var admin = new ApplicationUser
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    EmployeeName = "Admin User"
                };

                var result = await _userManager.CreateAsync(admin, "Admin@123");  // Set a default password
                if (result.Succeeded)
                {
                    // Assign the Admin role to the user
                    await _userManager.AddToRoleAsync(admin, "Admin");
                }

                // Create 2 Employee users
                for (int i = 1; i <= 2; i++)
                {
                    var employee = new ApplicationUser
                    {
                        UserName = $"employee{i}@example.com",
                        Email = $"employee{i}@example.com",
                        EmployeeName = $"Employee {i}"
                    };

                    var employeeResult = await _userManager.CreateAsync(employee, "Employee@123");  // Set default password
                    if (employeeResult.Succeeded)
                    {
                        // Assign the Employee role to the user
                        await _userManager.AddToRoleAsync(employee, "Employee");
                    }
                }
            }
        }
    }
}
