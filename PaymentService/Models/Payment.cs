namespace PaymentService.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int LicenseId { get; set; }
        public int TenantId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}