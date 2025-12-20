namespace PaymentService.DTOs
{
    public class PaymentResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string TransactionId { get; set; }

        public string Status {get;set;}
        public DateTime PaymentDate { get; set; }
    }
}