namespace NotificationService.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int LicenseId { get; set; }
        public string LicenseNumber { get; set; }
        public string RecipientEmail { get; set; }
        public string RecipientName { get; set; }
        public string Subject { get; set; }
        public string MessageBody { get; set; }
        public string NotificationType { get; set; }
        public bool IsSent { get; set; }
        public DateTime? SentDate { get; set; }
        public int TenantId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}