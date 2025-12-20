using FrontEnd.Models;

namespace FrontEnd.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterViewModel model);
        Task<AuthResponse> LoginAsync(LoginViewModel model);
    }
}