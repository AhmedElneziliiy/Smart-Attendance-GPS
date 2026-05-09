using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smart_Attendance_GPS.DTOs.Company;
using Smart_Attendance_GPS.Services.IService;

namespace Smart_Attendance_GPS.Controllers
{
    [Route("api/companies")]
    [ApiController]
    //[Authorize(Roles = "Admin")]  // Only Admins can access these endpoints
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        // Get all companies
        [HttpGet]
        public async Task<IActionResult> GetAllCompanies()
        {
            var companies = await _companyService.GetAllCompaniesAsync();
            return Ok(companies);
        }

        // Get company by ID
        // Get all employees of a company
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyById(int id)
        {
            var company = await _companyService.GetCompanyByIdAsync(id);
            if (company == null)
            {
                return NotFound("Company not found");
            }
            return Ok(company);
        }

        // Add a new company
        [HttpPost]
        public async Task<IActionResult> AddCompany([FromBody] CreateCompanyDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }

            var company = await _companyService.AddCompanyAsync(model);
            return CreatedAtAction(nameof(GetCompanyById), new { id = company.CompanyId }, company);
        }

        // Update an existing company
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(int id, [FromBody] UpdateCompanyDto model)
        {
            var company = await _companyService.UpdateCompanyAsync(id, model);
            if (company == null)
            {
                return NotFound("Company not found");
            }
            return Ok(company);
        }

        // Delete a company
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var result = await _companyService.DeleteCompanyAsync(id);
            if (!result)
            {
                return NotFound("Company not found");
            }
            return Ok("Company Deleted Successfully");
        }

       
    }
}
