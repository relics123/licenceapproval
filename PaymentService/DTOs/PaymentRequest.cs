namespace PaymentService.DTOs
{
    public class PaymentRequest
    {
        public int LicenseId { get; set; }
        public int TenantId { get; set; }
        public decimal Amount { get; set; }
    }
}