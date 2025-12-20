using System.Text;
using System.Text.Json;

namespace PaymentService.Services
{
    public class LicenseServiceClient : ILicenseServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<LicenseServiceClient> _logger;
        private readonly string _licenseServiceUrl;

        public LicenseServiceClient(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<LicenseServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _licenseServiceUrl = configuration.GetValue<string>("ServiceUrls:LicenseService") 
                ?? "http://localhost:5002";
        }

        public async Task<bool> UpdateLicensePaymentStatusAsync(int licenseId, int tenantId)
        {
            try
            {
                var request = new
                {
                    licenseId = licenseId,
                    tenantId = tenantId
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation(
                    "Calling LicenseService to update payment status for License {LicenseId}",
                    licenseId);

                var response = await _httpClient.PostAsync(
                    $"{_licenseServiceUrl}/api/license/update-payment",
                    content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation(
                        "Successfully updated license {LicenseId} payment status",
                        licenseId);
                    return true;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning(
                    "Failed to update license {LicenseId}. Status: {StatusCode}, Error: {Error}",
                    licenseId, response.StatusCode, errorContent);

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error calling LicenseService to update payment status for license {LicenseId}",
                    licenseId);
                return false;
            }
        }
    }
}