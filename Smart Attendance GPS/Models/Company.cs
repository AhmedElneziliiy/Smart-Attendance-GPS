namespace Smart_Attendance_GPS.Models
{
    public class Company
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public decimal CompanyLatitude { get; set; }
        public decimal CompanyLongitude { get; set; }

        public ICollection<UserCompany> UserCompanies { get; set; }

    }
}
