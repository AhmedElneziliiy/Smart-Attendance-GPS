namespace Smart_Attendance_GPS.DTOs.Company
{
    public class CompanyDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public decimal CompanyLatitude { get; set; }
        public decimal CompanyLongitude { get; set; }
        public List<EmployeeDto> Employees { get; set; } // List of employee names
    }

}
