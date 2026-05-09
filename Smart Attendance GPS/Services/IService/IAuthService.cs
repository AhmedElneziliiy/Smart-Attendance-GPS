using Smart_Attendance_GPS.Models;

namespace Smart_Attendance_GPS.Services.IService
{
    public interface IAuthService
    {
        Task<string> GenerateTokenForUser(ApplicationUser user);

        Task<bool> ValidateUserCredentials(string email, string password);
    }
}
