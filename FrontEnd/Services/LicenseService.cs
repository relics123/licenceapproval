using System.Text;
using System.Text.Json;
using FrontEnd.Models;

namespace FrontEnd.Services
{
    public class LicenseService : ILicenseService
    {
        private readonly HttpClient _httpClient;
        private readonly string _licenseServiceUrl;

        public LicenseService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _licenseServiceUrl = configuration.GetValue<string>("ServiceUrls:LicenseService");
        }

        public async Task<List<LicenseInfo>> GetLicensesAsync(int tenantId, int? userId = null)
        {
            try
            {
                var url = $"{_licenseServiceUrl}/api/license/list?tenantId={tenantId}";
                // if (userId.HasValue)
                // {
                //     url += $"&userId={userId.Value}";
                // }

                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<LicenseInfo>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<LicenseInfo>();
                }

                return new List<LicenseInfo>();
            }
            catch
            {
                return new List<LicenseInfo>();
            }
        }

        public async Task<LicenseInfo> GetActiveLicenseAsync(int tenantId, int userId)
        {
            try
            {
                var url = $"{_licenseServiceUrl}/api/license/active?tenantId={tenantId}&userId={userId}";
                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<LicenseInfo>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

       public async Task<LicenseInfo> CreateLicenseAsync(LicenseApplicationViewModel model, int tenantId, int userId)
{
    try
    {
        // ✅ Compact debug output
        Console.WriteLine($"\n🔍 CreateLicenseAsync(tenantId={tenantId}, userId={userId})");
        Console.WriteLine($"   Applicant: {model.ApplicantName}, Company: {model.CompanyName}");
        
        var request = new
        {
            applicantName = model.ApplicantName,
            gstNumber = model.GstNumber,
            companyName = model.CompanyName,
            city = model.City,
            tenantId = tenantId,
            userId = userId
        };

        var json = JsonSerializer.Serialize(request);
        Console.WriteLine($"   Payload: {json}");
        
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_licenseServiceUrl}/api/license/create", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        
        Console.WriteLine($"   Response [{response.StatusCode}]: {responseContent}\n");
        
        if (response.IsSuccessStatusCode)
        {
            var result = JsonSerializer.Deserialize<CreateLicenseResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result.Success)
            {
                return await GetLicenseByIdAsync(result.LicenseId, tenantId);
            }
        }

        return null;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Exception in CreateLicenseAsync: {ex.Message}");
        return null;
    }
}

        public async Task<bool> UpdatePaymentAsync(int licenseId, int tenantId)
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

                var response = await _httpClient.PostAsync($"{_licenseServiceUrl}/api/license/update-payment", content);
                
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

       public async Task<LicenseInfo> GetLicenseByIdAsync(int licenseId, int tenantId)
{
    try
    {
        Console.WriteLine($"\n🔍 GetLicenseByIdAsync(licenseId={licenseId}, tenantId={tenantId})");
        
        var licenses = await GetLicensesAsync(tenantId);
        
        Console.WriteLine($"   Found {licenses?.Count ?? 0} license(s) for tenant");
        
        var result =  licenses?.FirstOrDefault(l => l.Id == licenseId);
        
        if (result != null)
        {
            Console.WriteLine($"   ✅ License {result.LicenseNumber} found\n");
        }
        else
        {
            Console.WriteLine($"   ❌ License ID {licenseId} NOT found\n");
        }
        
        return result;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"   ❌ Exception: {ex.Message}\n");
        return null;
    }
}

        private class CreateLicenseResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public int LicenseId { get; set; }
            public string LicenseNumber { get; set; }
        }
    }
}