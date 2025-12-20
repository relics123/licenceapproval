using IdentityService.Models;

namespace IdentityService.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}