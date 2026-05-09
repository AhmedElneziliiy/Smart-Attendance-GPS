using Smart_Attendance_GPS.DTOs.Company;
using Smart_Attendance_GPS.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Smart_Attendance_GPS.Services.IService
{
    public interface ICompanyService
    {
        Task<IEnumerable<AllCompaniesDto>> GetAllCompaniesAsync();
        Task<CompanyDto> GetCompanyByIdAsync(int id);
        Task<CompanyDto> AddCompanyAsync(CreateCompanyDto model);
        Task<AllCompaniesDto> UpdateCompanyAsync(int id, UpdateCompanyDto model);
        Task<bool> DeleteCompanyAsync(int id);
    }
}
