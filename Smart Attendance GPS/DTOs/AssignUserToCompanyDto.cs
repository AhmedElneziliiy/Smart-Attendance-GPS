using System.ComponentModel.DataAnnotations;

namespace Smart_Attendance_GPS.DTOs
{
    public class AssignUserToCompanyDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public int CompanyId{get; set;}
    }
}
