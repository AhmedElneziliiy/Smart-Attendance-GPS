using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Smart_Attendance_GPS.DTOs
{
    public class CreateUserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Required]
        public string EmployeeName { get; set; }
        [Phone]
        public string EmployeePhone { get; set; }
    }

}
