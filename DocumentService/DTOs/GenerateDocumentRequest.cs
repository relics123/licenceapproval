namespace DocumentService.DTOs
{
    public class GenerateDocumentRequest
    {
        public int LicenseId { get; set; }
        public string LicenseNumber { get; set; }
        public string ApplicantName { get; set; }
        public string GstNumber { get; set; }
        public string CompanyName { get; set; }
        public string City { get; set; }
        public DateTime ApprovalDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int TenantId { get; set; }
    }
}