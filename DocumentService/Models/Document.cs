namespace DocumentService.Models
{
    public class Document
    {
        public int Id { get; set; }
        public int LicenseId { get; set; }
        public string LicenseNumber { get; set; }
        public string DocumentPath { get; set; }
        public string FileName { get; set; }
        public int TenantId { get; set; }
        public DateTime GeneratedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}