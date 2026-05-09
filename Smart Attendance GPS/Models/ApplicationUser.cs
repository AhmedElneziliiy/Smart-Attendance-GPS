using Microsoft.AspNetCore.Identity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Smart_Attendance_GPS.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string EmployeeName { get; set; }   // For Employees only
        //public int? CompanyId { get; set; }         // Foreign Key to Company

        public ICollection<UserCompany> UserCompanies { get; set; }

        /// <summary>
        /// Enable/disable face verification for this specific user
        /// </summary>
        public bool IsFaceVerificationEnabled { get; set; } = false;

        /// <summary>
        /// Face embedding (512 floats = 2048 bytes) for face recognition
        /// Stored as byte array for efficient storage
        /// </summary>
        public byte[]? FaceEmbedding { get; set; }

        /// <summary>
        /// Timestamp when face was enrolled for this user
        /// </summary>
        public DateTime? FaceEnrolledAt { get; set; }

    }
}
