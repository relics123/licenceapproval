using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.DTOs;
using PaymentService.Models;

namespace PaymentService.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentResponse> MakePaymentAsync(PaymentRequest request)
        {
            if (request.LicenseId <= 0 || request.TenantId <= 0 || request.Amount <= 0)
            {
                return new PaymentResponse
                {
                    Success = false,
                    Message = "Invalid payment details"
                };
            }

            var transactionId = GenerateTransactionId();

            var payment = new Payment
            {
                LicenseId = request.LicenseId,
                TenantId = request.TenantId,
                Amount = request.Amount,
                TransactionId = transactionId,
                Status = "Completed",
                PaymentDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return new PaymentResponse
            {
                Success = true,
                Message = "Payment successfully completed",
                TransactionId = transactionId,
                PaymentDate = payment.PaymentDate
            };
        }

        public async Task<PaymentResponse?> GetPaymentStatusAsync(string transactionId)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId);

            if (payment == null)
                return null;

            return new PaymentResponse
            {
                TransactionId = payment.TransactionId,
                
                Status = payment.Status,
               
                PaymentDate = payment.PaymentDate
            };
        }

        private string GenerateTransactionId()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"TXN{timestamp}{random}";
        }
    }
}
