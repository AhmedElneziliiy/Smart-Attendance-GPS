using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Smart_Attendance_GPS.DTOs.Company;
using Smart_Attendance_GPS.Models;
using Smart_Attendance_GPS.Models.Context;
using Smart_Attendance_GPS.Services.IService;

namespace Smart_Attendance_GPS.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AttendanceService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<bool> CheckInAsync(string email, double latitude, double longitude)
            {
                // Retrieve user by email
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return false;  // User not found
                }

                // Check if the user is already checked in on the same day
                var existingAttendance = await _context.Attendances
                    .Where(a => a.UserId == user.Id && a.CheckOutDate == null && a.CheckInDate.Value.Date == DateTime.UtcNow.Date)
                    .FirstOrDefaultAsync();

                if (existingAttendance != null)
                {
                    return false;  // User is already checked in
                }

                // Get the companies the user is assigned to
                var userCompanies = await _context.UserCompanies
                    .Where(uc => uc.UserId == user.Id)
                    .Include(uc => uc.Company)
                    .ToListAsync();

                bool isWithinRange = false;
                int companyId = 0;

            // Loop through all companies the user is assigned to
            foreach (var userCompany in userCompanies)
                {
                    var company = userCompany.Company;
                    var companyLat = (double)company.CompanyLatitude;
                    var companyLon = (double)company.CompanyLongitude;

                    // Calculate the distance between the employee's coordinates and the company's coordinates
                    var distance = CalculateDistance(companyLat, companyLon, latitude, longitude);

                    // Tolerance of 50 meters (0.05 km)
                    if (distance <= 0.05)
                    {
                        isWithinRange = true;
                        companyId = company.CompanyId;
                        break;  // Found a matching company location
                    }
                }

                if (!isWithinRange)
                {
                    return false;  // Coordinates are outside the tolerance range for all assigned companies
                }

                // Create new attendance record for check-in
                var attendance = new Attendance
                {
                    UserId = user.Id,
                    CompanyId = companyId,
                    CheckInDate = DateTime.UtcNow,
                    CheckOutDate = null  // Check-out will be updated later
                };

                _context.Attendances.Add(attendance);
                await _context.SaveChangesAsync();

                return true;  // Check-in successful
            }
        public async Task<bool> CheckOutAsync(string email, double latitude, double longitude)
        {
            // Retrieve user by email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return false;  // User not found
            }

            // Find the active attendance (check-in without check-out)
            var attendance = await _context.Attendances
                .Where(a => a.UserId == user.Id && a.CheckOutDate == null)
                .FirstOrDefaultAsync();

            if (attendance == null)
            {
                return false;  // No active check-in found
            }

            // Get the companies the user is assigned to
            var userCompanies = await _context.UserCompanies
                .Where(uc => uc.UserId == user.Id)
                .Include(uc => uc.Company)
                .ToListAsync();

            bool isWithinRange = false;

            // Loop through all companies the user is assigned to
            foreach (var userCompany in userCompanies)
            {
                var company = userCompany.Company;
                var companyLat = (double)company.CompanyLatitude;
                var companyLon = (double)company.CompanyLongitude;

                // Calculate the distance between the employee's coordinates and the company's coordinates
                var distance = CalculateDistance(companyLat, companyLon, latitude, longitude);

                // Tolerance of 50 meters (0.05 km)
                if (distance <= 0.05)
                {
                    isWithinRange = true;
                    break;  // Found a matching company location
                }
            }

            if (!isWithinRange)
            {
                return false;  // Coordinates are outside the tolerance range for all assigned companies
            }

            // Check-out the user (update check-out time)
            attendance.CheckOutDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;  // Check-out successful
        }
        public async Task<IEnumerable<AttendanceReportDto>> GetAttendanceReportAsync(string userId, DateTime fromDate, DateTime toDate)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;  // User not found
            }

            var attendances = await _context.Attendances
                .Where(a => a.UserId == userId && a.CheckInDate >= fromDate && a.CheckOutDate <= toDate)
                .Include(a => a.Company) // Include the company details
                .ToListAsync();

            var attendanceReports = attendances.Select(a => new AttendanceReportDto
            {
                AttendanceId = a.AttendanceId,
                CheckInDate = a.CheckInDate,
                CheckOutDate = a.CheckOutDate,
                Duration = CalculateDurationInHours(a.CheckInDate, a.CheckOutDate), 
                CompanyName = a.Company.CompanyName 
            }).ToList();

            return attendanceReports;
        }
        // Helper method to calculate the duration in hours between check-in and check-out
        private double CalculateDurationInHours(DateTime? checkInDate, DateTime? checkOutDate)
        {
            if (checkInDate == null || checkOutDate == null)
            {
                return 0; // No duration if either date is null
            }

            // Calculate duration in minutes and then convert it to hours
            var durationInMinutes = (checkOutDate.Value - checkInDate.Value).TotalMinutes;
            return Math.Round(durationInMinutes / 60,2); // Convert to hours
        }
        // Helper method to get the first day of the current month
        public DateTime GetFirstDayOfCurrentMonth()
        {
            var currentDate = DateTime.UtcNow;
            return new DateTime(currentDate.Year, currentDate.Month, 1);
        }

        // Helper method to calculate the distance between two points using the Haversine formula
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // Radius of the earth in km
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = R * c; // Distance in km
            return distance; // Return distance in km
        }

        // Helper method to convert degrees to radians
        private double ToRadians(double degree)
        {
            return degree * (Math.PI / 180);
        }
        // Method to check if a user exists in the system
        public async Task<bool> CheckUserExistenceAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user != null;
        }
    }
}
