namespace NotificationService.DTOs
{
    public class EmailResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int NotificationId { get; set; }
    }
}