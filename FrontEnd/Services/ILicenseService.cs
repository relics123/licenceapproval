using FrontEnd.Models;

namespace FrontEnd.Services
{
    public interface ILicenseService
    {
        Task<List<LicenseInfo>> GetLicensesAsync(int tenantId, int? userId = null);
        Task<LicenseInfo> GetActiveLicenseAsync(int tenantId, int userId);
        Task<LicenseInfo> CreateLicenseAsync(LicenseApplicationViewModel model, int tenantId, int userId);
        Task<bool> UpdatePaymentAsync(int licenseId, int tenantId);
        Task<LicenseInfo> GetLicenseByIdAsync(int licenseId, int tenantId);
    }
}