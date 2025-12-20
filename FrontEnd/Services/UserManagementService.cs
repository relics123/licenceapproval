using System.Text;
using System.Text.Json;
using FrontEnd.Models;

namespace FrontEnd.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly HttpClient _httpClient;
        private readonly string _identityServiceUrl;
        private readonly ILogger<UserManagementService> _logger; // ✅ Add logger

        public UserManagementService(
            IHttpClientFactory httpClientFactory, 
            IConfiguration configuration,
            ILogger<UserManagementService> logger) // ✅ Inject logger
        {
            _httpClient = httpClientFactory.CreateClient();
            _identityServiceUrl = configuration.GetValue<string>("ServiceUrls:IdentityService");
            _logger = logger; // ✅ Store logger
        }

        public async Task<List<UserItemViewModel>> GetUsersAsync(int tenantId)
        {
            try
            {
                _logger.LogInformation("=== GetUsersAsync called ===");
                _logger.LogInformation("TenantId: {TenantId}", tenantId);
                
                var response = await _httpClient.GetAsync($"{_identityServiceUrl}/api/usermanagement/list/{tenantId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<UsersListResponse>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    var users = result?.Users?.Select(u => new UserItemViewModel
                    {
                        UserId = u.UserId,
                        Username = u.Username,
                        Email = u.Email,
                        FullName = u.FullName,
                        Role = u.Role,
                        IsActive = u.IsActive,
                        CreatedDate = u.CreatedDate
                    }).ToList() ?? new List<UserItemViewModel>();

                    _logger.LogInformation("Found {Count} users", users.Count);
                    return users;
                }

                _logger.LogWarning("GetUsersAsync failed with status: {StatusCode}", response.StatusCode);
                return new List<UserItemViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetUsersAsync for TenantId: {TenantId}", tenantId);
                return new List<UserItemViewModel>();
            }
        }

        public async Task<bool> CreateUserAsync(CreateUserViewModel model, int tenantId)
        {
            try
            {
                // ✅ LOG EVERYTHING
                Console.WriteLine("=======================================");
                Console.WriteLine("=== CreateUserAsync - DEBUG INFO ===");
                Console.WriteLine("=======================================");
                Console.WriteLine($"TenantId passed: {tenantId}");
                Console.WriteLine($"Username: {model.Username}");
                Console.WriteLine($"Email: {model.Email}");
                Console.WriteLine($"FullName: {model.FullName}");
                Console.WriteLine($"API URL: {_identityServiceUrl}/api/usermanagement/create");
                Console.WriteLine("=======================================");

                _logger.LogInformation("=== CreateUserAsync called ===");
                _logger.LogInformation("TenantId: {TenantId}", tenantId);
                _logger.LogInformation("Username: {Username}", model.Username);
                _logger.LogInformation("Email: {Email}", model.Email);
                _logger.LogInformation("FullName: {FullName}", model.FullName);

                var request = new
                {
                    username = model.Username,
                    email = model.Email,
                    password = model.Password,
                    fullName = model.FullName,
                    tenantId = tenantId
                };

                var json = JsonSerializer.Serialize(request);
                
                // ✅ Log the JSON payload
                Console.WriteLine("JSON Payload:");
                Console.WriteLine(json);
                _logger.LogInformation("Request JSON: {Json}", json);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(
                    $"{_identityServiceUrl}/api/usermanagement/create", 
                    content);

                var responseContent = await response.Content.ReadAsStringAsync();

                // ✅ Log the response
                Console.WriteLine($"Response Status: {response.StatusCode}");
                Console.WriteLine($"Response Body: {responseContent}");
                Console.WriteLine("=======================================");

                _logger.LogInformation("Response Status: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Response Body: {Response}", responseContent);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("User created successfully");
                    return true;
                }
                else
                {
                    _logger.LogWarning("CreateUser failed with status: {StatusCode}", response.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine("=======================================");

                _logger.LogError(ex, "Error in CreateUserAsync for TenantId: {TenantId}", tenantId);
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(EditUserViewModel model, int tenantId)
        {
            try
            {
                _logger.LogInformation("=== UpdateUserAsync called ===");
                _logger.LogInformation("TenantId: {TenantId}", tenantId);
                _logger.LogInformation("UserId: {UserId}", model.UserId);
                _logger.LogInformation("Username: {Username}", model.Username);

                var request = new
                {
                    userId = model.UserId,
                    username = model.Username,
                    email = model.Email,
                    fullName = model.FullName,
                    isActive = model.IsActive,
                    tenantId = tenantId
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(
                    $"{_identityServiceUrl}/api/usermanagement/update", 
                    content);

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Response Status: {StatusCode}", response.StatusCode);
                _logger.LogInformation("Response Body: {Response}", responseContent);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateUserAsync for UserId: {UserId}", model.UserId);
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int userId, int tenantId)
        {
            try
            {
                _logger.LogInformation("=== DeleteUserAsync called ===");
                _logger.LogInformation("TenantId: {TenantId}", tenantId);
                _logger.LogInformation("UserId: {UserId}", userId);

                var response = await _httpClient.DeleteAsync(
                    $"{_identityServiceUrl}/api/usermanagement/delete/{userId}/{tenantId}");

                _logger.LogInformation("Response Status: {StatusCode}", response.StatusCode);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteUserAsync for UserId: {UserId}", userId);
                return false;
            }
        }

        private class UsersListResponse
        {
            public bool Success { get; set; }
            public List<UserDetail> Users { get; set; }
        }

        private class UserDetail
        {
            public int UserId { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string FullName { get; set; }
            public string Role { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedDate { get; set; }
        }
    }
}