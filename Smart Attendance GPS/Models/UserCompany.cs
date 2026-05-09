namespace Smart_Attendance_GPS.Models
{
    public class UserCompany
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
