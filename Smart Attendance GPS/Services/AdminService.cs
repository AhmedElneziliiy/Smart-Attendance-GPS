using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Smart_Attendance_GPS.DTOs;
using Smart_Attendance_GPS.Models;
using Smart_Attendance_GPS.Models.Context;
using Smart_Attendance_GPS.Services.IService;

namespace Smart_Attendance_GPS.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminService(UserManager<ApplicationUser> userManager
            , RoleManager<IdentityRole> roleManager
            , ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        public async Task<UserDto> CreateUserAsync(CreateUserDto model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmployeeName = model.EmployeeName,
                PhoneNumber = model.EmployeePhone
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // By default, assign the "Employee" role to the user
                await _userManager.AddToRoleAsync(user, "Employee");
                return new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    EmployeeName = user.EmployeeName,
                    EmployeePhone = user.PhoneNumber,
                    Roles = await _userManager.GetRolesAsync(user)
                };
            }
            return null;

        }

        public async Task<UserDto> UpdateUserAsync(string id, UpdateUserDto model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            user.Email = model.Email;
            user.EmployeeName = model.EmployeeName;
            user.PhoneNumber = model.EmployeePhone;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return null;
            }

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                EmployeeName = user.EmployeeName,
                EmployeePhone = user.PhoneNumber,
                Roles = await _userManager.GetRolesAsync(user)
            };
        }

        public async Task<UserDto> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                EmployeeName = user.EmployeeName,
                EmployeePhone = user.PhoneNumber,
                Roles = await _userManager.GetRolesAsync(user),
                Companies = await GetUserCompaniesAsync(user.Id)

            };
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    EmployeeName = user.EmployeeName,
                    EmployeePhone = user.PhoneNumber,
                    Roles = await _userManager.GetRolesAsync(user),
                    Companies = await GetUserCompaniesAsync(user.Id)

                };

                userDtos.Add(userDto);
            }

            return userDtos;
        }

        public async Task<bool> AddRoleToUserAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            // Check if the role is valid (only allow Admin or Employee)
            if (role != "Admin" && role != "Employee")
            {
                return false;  // Invalid role
            }
            //check if user already has the role
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Contains(role))
            {
                return false;
            }

            // Check if role exists, otherwise create it
            var roleExist = await _roleManager.RoleExistsAsync(role);
            if (!roleExist)
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }

            // Add role to user (user can have multiple roles)
            await _userManager.AddToRoleAsync(user, role);
            return true;
        }

        public async Task<bool> AssignUserToCompanyAsync(string userId, int companyId)
        {
            // Find the user by ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;  // User not found
            }

            // Check if the company exists
            var company = await _context.Companies.FindAsync(companyId);
            if (company == null)
            {
                return false;  // Company not found
            }

            // Check if the user is already assigned to this company
            var existingAssignment = await _context.UserCompanies
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CompanyId == companyId);

            if (existingAssignment != null)
            {
                return false;  // User is already assigned to this company
            }

            // Assign the user to the company (create a new UserCompany record)
            var userCompany = new UserCompany
            {
                UserId = userId,
                CompanyId = companyId
            };

            _context.UserCompanies.Add(userCompany);
            await _context.SaveChangesAsync();

            return true;  // Successfully assigned
        }

        public async Task<bool> UnassignUserFromCompanyAsync(string userId, int companyId)
        {
            // Find the user by ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;  // User not found
            }

            // Check if the company exists
            var company = await _context.Companies.FindAsync(companyId);
            if (company == null)
            {
                return false;  // Company not found
            }

            // Find the user-company relationship from the join table
            var userCompany = await _context.UserCompanies
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CompanyId == companyId);

            if (userCompany == null)
            {
                return false;  // User is not assigned to this company
            }

            // Remove the user-company relationship
            _context.UserCompanies.Remove(userCompany);
            await _context.SaveChangesAsync();

            return true;  // Successfully unassigned
        }

        public async Task<IList<string>> GetUserCompaniesAsync(string userId)
        {
            var userCompanies = await _context.UserCompanies
                .Where(uc => uc.UserId == userId)
                .Include(uc => uc.Company)
                .ToListAsync();

            var companyNames = userCompanies.Select(uc => uc.Company.CompanyName).ToList();
            return companyNames;
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                EmployeeName = user.EmployeeName,
                EmployeePhone = user.PhoneNumber,
                Roles = await _userManager.GetRolesAsync(user),
                Companies = await GetUserCompaniesAsync(user.Id)
            };
        }
    }
    
}
