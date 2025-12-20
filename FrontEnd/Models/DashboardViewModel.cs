namespace FrontEnd.Models
{
    public class DashboardViewModel
    {
        public UserInfo CurrentUser { get; set; }
        public LicenseInfo ActiveLicense { get; set; }
        public List<LicenseInfo> RecentLicenses { get; set; }
        public DashboardStats Stats { get; set; }
    }

    public class DashboardStats
    {
        public int TotalLicenses { get; set; }
        public int ActiveLicenses { get; set; }
        public int ExpiredLicenses { get; set; }
        public int PendingPayment { get; set; }
    }
}