namespace Smart_Attendance_GPS.Models
{
    public class Attendance
    {
        public int AttendanceId { get; set; }
        public string UserId { get; set; }  // Foreign Key to ApplicationUser (Employee)
        public ApplicationUser User { get; set; }  // Navigation Property


        public int? CompanyId { get; set; }  // Foreign Key to Company
        public Company Company { get; set; }  // Navigation Property to Company

        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }

    }

}
