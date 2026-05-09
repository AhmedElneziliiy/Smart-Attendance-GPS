using System.ComponentModel.DataAnnotations;

namespace Smart_Attendance_GPS.DTOs
{
    public class AssignRoleDto
    {
        [Required]
        public string Role { get; set; }  // Role to assign to the user (e.g., Admin, Employee)
    }

}
