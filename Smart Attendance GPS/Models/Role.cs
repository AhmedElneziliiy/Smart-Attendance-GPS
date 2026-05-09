using Microsoft.AspNetCore.Identity;

namespace Smart_Attendance_GPS.Models
{
    public class Role : IdentityRole
    {
        public string Description { get; set; }
    }

}
