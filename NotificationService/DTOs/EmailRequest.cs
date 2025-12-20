namespace NotificationService.DTOs
{
    public class EmailRequest
    {
        public int LicenseId { get; set; }
        public string LicenseNumber { get; set; }
        public string RecipientEmail { get; set; }
        public string RecipientName { get; set; }
        public string NotificationType { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int TenantId { get; set; }
    }
}