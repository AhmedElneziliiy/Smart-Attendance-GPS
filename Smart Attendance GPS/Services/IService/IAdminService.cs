using Smart_Attendance_GPS.DTOs;
using Smart_Attendance_GPS.Models;

namespace Smart_Attendance_GPS.Services.IService
{
    public interface IAdminService
    {
        Task<UserDto> CreateUserAsync(CreateUserDto model);
        Task<UserDto> UpdateUserAsync(string id, UpdateUserDto model);
        Task<UserDto> GetUserByIdAsync(string id);
        Task<IList<string>> GetUserCompaniesAsync(string userId);
        Task<bool> DeleteUserAsync(string id);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<bool> AddRoleToUserAsync(string userId, string role);
        Task<bool> AssignUserToCompanyAsync(string userId, int companyId);
        Task<bool> UnassignUserFromCompanyAsync(string userId, int companyId);
    }
}
