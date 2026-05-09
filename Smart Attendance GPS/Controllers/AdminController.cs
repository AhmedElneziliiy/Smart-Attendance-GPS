using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smart_Attendance_GPS.DTOs;
using Smart_Attendance_GPS.Services.IService;

namespace Smart_Attendance_GPS.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // Get all employees
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(users);
        }

        // Get an employee by ID
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
           
            var user = await _adminService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }

        // Create a new employee user
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto model)
        {
            // Validate model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  
            }
            // Check if email already exists using a service method
            var existingUser = await _adminService.GetUserByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest("A user with this email already exists.");
            }
            var user = await _adminService.CreateUserAsync(model);
            if (user == null)
            {
                return BadRequest("User creation failed");
            }

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        // Update an employee user
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // Return validation errors
            }

            var user = await _adminService.UpdateUserAsync(id, model);
            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }
        
        // Assign a role to a user (Admin can assign roles like Admin or Employee)
        [HttpPost("users/{id}/role")]
        public async Task<IActionResult> AssignRoleToUser(string id, [FromBody] AssignRoleDto model)
        {
            // Validate the role
            if (model.Role != "Admin" && model.Role != "Employee")
            {
                return BadRequest("Invalid role. Only 'Admin' or 'Employee' roles can be assigned.");
            }

            // Ensure the role is added successfully by calling the service method
            var roleAdded = await _adminService.AddRoleToUserAsync(id, model.Role);

            if (!roleAdded)
            {
                return BadRequest("Failed to assign role. Possible reasons: user not found, role already assigned, or invalid role.");
            }

            return Ok($"Role {model.Role} assigned to user {id}");
        }

        // Delete an employee user
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _adminService.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound("User not found");
            }

            return Ok($"User With ID ({id}) deleted successfully");
        }

        [HttpPost("users/{userId}/assign-companies")]
        public async Task<IActionResult> AssignUserToCompanies(string userId, [FromBody] List<int> companyIds)
        {
            if (companyIds == null || companyIds.Count == 0)
            {
                return BadRequest("At least one company must be provided.");
            }

            foreach (var companyId in companyIds)
            {
                var result = await _adminService.AssignUserToCompanyAsync(userId, companyId);
                if (!result)
                {
                    return BadRequest($"User could not be assigned to company with ID {companyId}. This could be due to the user already being assigned or the company not existing.");
                }
            }

            return Ok("User successfully assigned to selected companies.");
        }

        [HttpPost("users/{userId}/unassign-company/{companyId}")]
        public async Task<IActionResult> UnassignUserFromCompany(string userId, int companyId)
        {
            var result = await _adminService.UnassignUserFromCompanyAsync(userId, companyId);

            if (result)
            {
                return Ok($"User {userId} successfully unassigned from company {companyId}.");
            }

            return BadRequest("User or Company not found, or the user is not assigned to this company.");
        }

    }
}
