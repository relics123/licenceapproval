namespace FrontEnd.Models
{
    public class LicenseInfo
    {
        public int Id { get; set; }
        public string LicenseNumber { get; set; }
        public string ApplicantName { get; set; }
        public string GstNumber { get; set; }
        public string CompanyName { get; set; }
        public string City { get; set; }
        public string Status { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool PaymentCompleted { get; set; }
        public decimal PaymentAmount { get; set; }
        public bool DocumentGenerated { get; set; }
        public int TenantId { get; set; }
        public int UserId { get; set; }
    }
}