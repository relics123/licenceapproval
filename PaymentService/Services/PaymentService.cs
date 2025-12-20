using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.DTOs;
using PaymentService.Models;

namespace PaymentService.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        private readonly ILicenseServiceClient _licenseServiceClient;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            AppDbContext context,
            ILicenseServiceClient licenseServiceClient,
            ILogger<PaymentService> logger)
        {
            _context = context;
            _licenseServiceClient = licenseServiceClient;
            _logger = logger;
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

            _logger.LogInformation(
                "Payment completed. TransactionId: {TransactionId}, LicenseId: {LicenseId}",
                transactionId, request.LicenseId);

            // ✨ NEW: Call LicenseService to update payment status
            var licenseUpdated = await _licenseServiceClient.UpdateLicensePaymentStatusAsync(
                request.LicenseId,
                request.TenantId);

            if (!licenseUpdated)
            {
                _logger.LogWarning(
                    "Payment succeeded but failed to update license {LicenseId}. Manual intervention may be needed.",
                    request.LicenseId);
            }

            return new PaymentResponse
            {
                Success = true,
                Message = "Payment successfully completed",
                TransactionId = transactionId,
                PaymentDate = payment.PaymentDate,
                LicenseUpdated = licenseUpdated // NEW: Indicate if license was updated
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