namespace Smart_Attendance_GPS.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeePhone { get; set; }
        public IList<string> Roles { get; set; } 
        public IList<string> Companies { get; set; }

    }
}
