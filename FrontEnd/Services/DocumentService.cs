using System.Text;
using System.Text.Json;
using FrontEnd.Models;

namespace FrontEnd.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly HttpClient _httpClient;
        private readonly string _documentServiceUrl;

        public DocumentService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _documentServiceUrl = configuration.GetValue<string>("ServiceUrls:DocumentService");
        }

        public async Task<bool> GenerateDocumentAsync(LicenseInfo license)
        {
            try
            {
                var request = new
                {
                    licenseId = license.Id,
                    licenseNumber = license.LicenseNumber,
                    applicantName = license.ApplicantName,
                    gstNumber = license.GstNumber,
                    companyName = license.CompanyName,
                    city = license.City,
                    approvalDate = license.ApprovalDate,
                    expiryDate = license.ExpiryDate,
                    tenantId = license.TenantId
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_documentServiceUrl}/api/document/generate", content);
                
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<byte[]> DownloadDocumentAsync(int licenseId, int tenantId)
        {
            try
            {
                var url = $"{_documentServiceUrl}/api/document/download/{licenseId}?tenantId={tenantId}";
                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}