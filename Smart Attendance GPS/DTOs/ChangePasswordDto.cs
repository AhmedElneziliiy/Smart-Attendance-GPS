using System.ComponentModel.DataAnnotations;

namespace Smart_Attendance_GPS.DTOs
{
    public class ChangePasswordDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
