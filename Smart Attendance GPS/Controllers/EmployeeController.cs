using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smart_Attendance_GPS.DTOs;
using Smart_Attendance_GPS.Services;
using Smart_Attendance_GPS.Services.IService;
using System.Security.Claims;

namespace Smart_Attendance_GPS.Controllers
{
    [Route("api/employee")]
    [ApiController]
    [Authorize]  // Ensure only authenticated users can access these endpoints
    public class EmployeeController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IAdminService _adminService;

        public EmployeeController(IAttendanceService attendanceService, IAdminService adminService)
        {
            _attendanceService = attendanceService;
            _adminService = adminService;
        }

        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] CheckinCheckoutRequest request)
        {
            var userId = User.Identity.Name; // Get the logged-in user's ID (from the token)

            var success = await _attendanceService.CheckInAsync(userId, request.Latitude, request.Longitude);

            if (success)
            {
                return Ok("Check-in successful.");
            }

            return BadRequest("Failed to check-in. Coordinates may be out of range or user already checked-in.");
        }

        // Endpoint for Check-out
        [HttpPost("checkout")]
        public async Task<IActionResult> CheckOut([FromBody] CheckinCheckoutRequest request)
        {
            var userId = User.Identity.Name; // Get the logged-in user's ID (from the token)

           var success = await _attendanceService.CheckOutAsync(userId, request.Latitude, request.Longitude);

            if (success)
            {
                return Ok("Check-out successful.");
            }

            return BadRequest("Failed to check-out. Coordinates may be out of range or no active check-in found.");
        }

        //to do get user profile and get attendance history from date to date 

        // Endpoint to get user's attendance report based on from and to dates
        [HttpGet("attendance/report")]
        public async Task<IActionResult> GetAttendanceReport(
            [FromQuery] string? userId,  // userId is now optional
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime? toDate = null)
        {
            // If no userId is provided in the request, use the one from the token
            if (string.IsNullOrEmpty(userId)){userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; }  // Get userId from the token (JWT)

            // Validate the userId
            if (string.IsNullOrWhiteSpace(userId)){return BadRequest("User ID cannot be empty.");}

            // If no toDate is provided, default it to today's date
            if (toDate == null){toDate = DateTime.UtcNow; } // Default to today's date if no toDate is provided


            // Validate that fromDate is not in the future
            if (fromDate > DateTime.UtcNow){return BadRequest("From date cannot be in the future.");}

            // Validate that toDate is not before fromDate
            if (toDate < fromDate){return BadRequest("To date cannot be before from date.");}

            // If fromDate is provided but toDate is not, set to first day of the current month
            if (fromDate == null){fromDate = _attendanceService.GetFirstDayOfCurrentMonth(); } // Default to the first day of the current month


            // Check if user exists and is assigned to the company
            var userExists = await _attendanceService.CheckUserExistenceAsync(userId);
            if (!userExists){return NotFound("User not found.");}

            // Get the attendance report
            var report = await _attendanceService.GetAttendanceReportAsync(userId, fromDate, toDate.Value);

            if (report == null || !report.Any())
            {
                return NotFound("No attendance records found for the given dates.");
            }

            return Ok(report);  // Return the attendance report
        }

        //get employee profile will call get by id endpoint in admin controller
        [HttpGet("users/Profile")]
        public async Task<IActionResult> GetUserProfile([FromQuery]string? userId)
        {
            if (string.IsNullOrEmpty(userId)) { userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; }  // Get userId from the token (JWT)
           
            var user = await _adminService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }
    }
}
