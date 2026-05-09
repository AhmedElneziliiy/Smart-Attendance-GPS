using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Smart_Attendance_GPS.DTOs;
using Smart_Attendance_GPS.Models;
using Smart_Attendance_GPS.Services.IService;

namespace Smart_Attendance_GPS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IAdminService _adminService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(IAuthService authService,
            UserManager<ApplicationUser> userManager
            ,IAdminService adminService) 
        {
            _authService = authService;
            _adminService = adminService;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModelDto model)
        {
            // Validate the user's credentials
            var isValidUser = await _authService.ValidateUserCredentials(model.Email, model.Password);

            if (!isValidUser)
            {
                return Unauthorized("Invalid email or password");
            }

            // Retrieve the user from the database
            var user = await _userManager.FindByEmailAsync(model.Email);

            // Generate the JWT token with multiple roles
            var token = await _authService.GenerateTokenForUser(user);

            var response=new LoginResponseDto
            {
                Token=token,
                Id=user.Id,
                Email=user.Email,
                EmployeeName=user.EmployeeName,
                EmployeePhone=user.PhoneNumber,
                Roles=await _userManager.GetRolesAsync(user),
                Companies=await _adminService.GetUserCompaniesAsync(user.Id)
            };

            return Ok(response);
        }

        //change password with email only without having the old password
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound("User not found");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest("Password change failed");
            }
            return Ok("Password changed successfully");
        }





    }
}
