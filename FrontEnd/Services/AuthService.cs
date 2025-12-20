using System.Text;
using System.Text.Json;
using FrontEnd.Models;

namespace FrontEnd.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _identityServiceUrl;

        public AuthService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _identityServiceUrl = configuration.GetValue<string>("ServiceUrls:IdentityService");
        }

        public async Task<AuthResponse> RegisterAsync(RegisterViewModel model)
        {
            var request = new
            {
                username = model.Username,
                email = model.Email,
                password = model.Password,
                fullName = model.FullName,
                companyName = model.CompanyName,
                phoneNumber = model.PhoneNumber
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_identityServiceUrl}/api/auth/register", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<AuthResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<AuthResponse> LoginAsync(LoginViewModel model)
        {
            var request = new
            {
                username = model.Username,
                password = model.Password,
                tenantCode = model.TenantCode
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_identityServiceUrl}/api/auth/login", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<AuthResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}