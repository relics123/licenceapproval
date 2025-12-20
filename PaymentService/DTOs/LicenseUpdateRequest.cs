namespace PaymentService.DTOs
{
    public class LicenseUpdateRequest
    {
        public int LicenseId { get; set; }
        public int TenantId { get; set; }
    }

    public class LicenseUpdateResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}