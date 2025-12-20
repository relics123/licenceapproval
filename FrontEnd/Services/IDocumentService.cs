using FrontEnd.Models;

namespace FrontEnd.Services
{
    public interface IDocumentService
    {
        Task<bool> GenerateDocumentAsync(LicenseInfo license);
        Task<byte[]> DownloadDocumentAsync(int licenseId, int tenantId);
    }
}