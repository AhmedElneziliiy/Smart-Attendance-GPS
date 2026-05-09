namespace Smart_Attendance_GPS.DTOs.Company
{
    public class AllCompaniesDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public decimal CompanyLatitude { get; set; }
        public decimal CompanyLongitude { get; set; }
    }
}
