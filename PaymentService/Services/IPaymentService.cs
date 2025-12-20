using PaymentService.DTOs;

namespace PaymentService.Services
{
    public interface IPaymentService
    {
        Task<PaymentResponse> MakePaymentAsync(PaymentRequest request);
        Task<PaymentResponse?> GetPaymentStatusAsync(string transactionId);
    }
}
