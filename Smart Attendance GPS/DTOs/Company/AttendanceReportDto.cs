namespace Smart_Attendance_GPS.DTOs.Company
{
    public class AttendanceReportDto
    {
        public int AttendanceId { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public double Duration { get; set; } // Duration in hours
        public string CompanyName { get; set; } // Company the employee attended
    }
}
