namespace Smart_Attendance_GPS.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeePhone { get; set; }
        public IList<string> Roles { get; set; }  // List of roles the user has
        public IList<string> Companies { get; set; }  // List of company names the user is assigned to

    }

}
