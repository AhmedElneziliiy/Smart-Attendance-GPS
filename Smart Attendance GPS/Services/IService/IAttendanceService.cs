using Smart_Attendance_GPS.DTOs.Company;

namespace Smart_Attendance_GPS.Services.IService
{
    public interface IAttendanceService
    {
        Task<bool> CheckInAsync(string userId, double latitude, double longitude);
        Task<bool> CheckOutAsync(string userId, double latitude, double longitude);
        Task<IEnumerable<AttendanceReportDto>> GetAttendanceReportAsync(string userId, DateTime fromDate, DateTime toDate);
        Task<bool> CheckUserExistenceAsync(string userId);
        DateTime GetFirstDayOfCurrentMonth();
    }
}
