using System.ComponentModel.DataAnnotations;

namespace Smart_Attendance_GPS.DTOs.Company
{
    public class UpdateCompanyDto
    {
        [Required]
        public string CompanyName { get; set; }

        [Required]
        [Range(-90, 90)]
        public decimal CompanyLatitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public decimal CompanyLongitude { get; set; }
    }

}
