using Microsoft.EntityFrameworkCore;
using Smart_Attendance_GPS.DTOs.Company;
using Smart_Attendance_GPS.Models;
using Smart_Attendance_GPS.Models.Context;
using Smart_Attendance_GPS.Services.IService;

namespace Smart_Attendance_GPS.Services
{
    public class CompanyService: ICompanyService
    {
        private readonly ApplicationDbContext _context;

        public CompanyService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all companies
        public async Task<IEnumerable<AllCompaniesDto>> GetAllCompaniesAsync()
        {
            var companies = await _context.Companies.ToListAsync();
            var companyDtos = companies.Select(c => new AllCompaniesDto
            {
                CompanyId = c.CompanyId,
                CompanyName = c.CompanyName,
                CompanyLatitude = c.CompanyLatitude,
                CompanyLongitude = c.CompanyLongitude
            }).ToList();

            return companyDtos;
        }

        // Get company by ID
        public async Task<CompanyDto> GetCompanyByIdAsync(int id)
        {
            var company = await _context.Companies
                .Include(c => c.UserCompanies)
                .ThenInclude(uc => uc.User)
                .FirstOrDefaultAsync(c => c.CompanyId == id);

            if (company == null)
            {
                return null;
            }

            var companyDto = new CompanyDto
            {
                CompanyId = company.CompanyId,
                CompanyName = company.CompanyName,
                CompanyLatitude = company.CompanyLatitude,
                CompanyLongitude = company.CompanyLongitude,
                Employees = company.UserCompanies.Select(uc => new EmployeeDto
                {
                    Id = uc.User.Id,
                    EmployeeName = uc.User.EmployeeName
                }).ToList()  // Map employees to EmployeeDto
            };

            return companyDto;
        }

        // Add a new company
        public async Task<CompanyDto> AddCompanyAsync(CreateCompanyDto model)
        {
            var company = new Company
            {
                CompanyName = model.CompanyName,
                CompanyLatitude = model.CompanyLatitude,
                CompanyLongitude = model.CompanyLongitude
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            var companyDto = new CompanyDto
            {
                CompanyId = company.CompanyId,
                CompanyName = company.CompanyName,
                CompanyLatitude = company.CompanyLatitude,
                CompanyLongitude = company.CompanyLongitude
            };

            return companyDto;
        }

        // Update an existing company
        public async Task<AllCompaniesDto> UpdateCompanyAsync(int id, UpdateCompanyDto model)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return null;
            }

            company.CompanyName = model.CompanyName;
            company.CompanyLatitude = model.CompanyLatitude;
            company.CompanyLongitude = model.CompanyLongitude;

            await _context.SaveChangesAsync();

            var companyDto = new AllCompaniesDto
            {
                CompanyId = company.CompanyId,
                CompanyName = company.CompanyName,
                CompanyLatitude = company.CompanyLatitude,
                CompanyLongitude = company.CompanyLongitude
            };

            return companyDto;
        }

        // Delete a company
        public async Task<bool> DeleteCompanyAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return false;
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return true;
        }
    
    }
}
