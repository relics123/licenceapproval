namespace FrontEnd.Models
{
    public class PaymentViewModel
    {
        public int LicenseId { get; set; }
        public string LicenseNumber { get; set; }
        public string ApplicantName { get; set; }
        public string CompanyName { get; set; }
        public decimal Amount { get; set; }
        public int TenantId { get; set; }
    }
}