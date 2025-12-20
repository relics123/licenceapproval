namespace FrontEnd.Services
{
    public interface IPaymentService
    {
        Task<bool> MakePaymentAsync(int licenseId, int tenantId, decimal amount);
    }
}