namespace PaymentService.Services
{
    public interface ILicenseServiceClient
    {
        Task<bool> UpdateLicensePaymentStatusAsync(int licenseId, int tenantId);
    }
}