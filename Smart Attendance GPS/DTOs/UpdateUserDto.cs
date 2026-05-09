using System.ComponentModel.DataAnnotations;

namespace Smart_Attendance_GPS.DTOs
{
    public class UpdateUserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
       
        [Required]
        public string EmployeeName { get; set; }
        [Required]
        public string EmployeePhone { get; set; }

    }

}
