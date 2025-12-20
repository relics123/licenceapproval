using Microsoft.EntityFrameworkCore;
using IdentityService.Data;
using IdentityService.DTOs;
using IdentityService.Helpers;
using IdentityService.Models;

namespace IdentityService.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthService(AppDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
{
    if (string.IsNullOrWhiteSpace(request.Username) ||
        string.IsNullOrWhiteSpace(request.Password) ||
        string.IsNullOrWhiteSpace(request.Email))
    {
        return new AuthResponse
        {
            Success = false,
            Message = "Username, email and password are required"
        };
    }

    // 🔒 Prevent duplicate tenant by email
    var existingTenant = await _context.Tenants
        .FirstOrDefaultAsync(t => t.Email == request.Email);

    if (existingTenant != null)
    {
        return new AuthResponse
        {
            Success = false,
            Message = "A tenant with this email already exists"
        };
    }

    // Generate unique tenant code
    var tenantCode = GenerateTenantCode(request.CompanyName);
    while (await _context.Tenants.AnyAsync(t => t.TenantCode == tenantCode))
    {
        tenantCode = GenerateTenantCode(request.CompanyName);
    }

    var tenant = new Tenant
    {
        TenantCode = tenantCode,
        CompanyName = request.CompanyName,
        Email = request.Email,
        PhoneNumber = request.PhoneNumber ?? "",
        IsActive = true,
        CreatedDate = DateTime.UtcNow
    };

    _context.Tenants.Add(tenant);
    await _context.SaveChangesAsync();

    // 🔒 Prevent duplicate username inside same tenant
    var existingUser = await _context.Users
        .AnyAsync(u => u.Username == request.Username && u.TenantId == tenant.Id);

    if (existingUser)
    {
        return new AuthResponse
        {
            Success = false,
            Message = "Username already exists for this tenant"
        };
    }

    var role = await _context.UserRoles
        .FirstAsync(r => r.Name == "TenantAdmin");

    var user = new User
    {
        Username = request.Username,
        Email = request.Email,
        PasswordHash = PasswordHelper.HashPassword(request.Password),
        FullName = request.FullName,
        TenantId = tenant.Id,
        UserRoleId = role.Id,
        IsActive = true,
        CreatedDate = DateTime.UtcNow
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    user.Tenant = tenant;
    user.UserRole = role;

    var token = _jwtService.GenerateToken(user);

    return new AuthResponse
    {
        Success = true,
        Message = "Registration successful",
        Token = token,
        User = new UserInfo
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            TenantCode = tenant.TenantCode,
            Role = user.UserRole.Name
        }
    };
}
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Username) || 
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.TenantCode))
            {
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = "Username, password and tenant code are required" 
                };
            }

            // Find tenant
            var tenant = await _context.Tenants
                .FirstOrDefaultAsync(t => t.TenantCode == request.TenantCode && t.IsActive);

            if (tenant == null)
            {
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = "Invalid tenant code" 
                };
            }

            // Find user
            var user = await _context.Users
                .Include(u => u.Tenant)
                .Include(u => u.UserRole)
                .FirstOrDefaultAsync(u => u.Username == request.Username && 
                                         u.TenantId == tenant.Id && 
                                         u.IsActive);

            if (user == null)
            {
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = "Invalid username or password" 
                };
            }

            // Verify password
            if (!PasswordHelper.VerifyPassword(request.Password, user.PasswordHash))
            {
                return new AuthResponse 
                { 
                    Success = false, 
                    Message = "Invalid username or password" 
                };
            }

            var token = _jwtService.GenerateToken(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                User = new UserInfo
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    TenantCode = user.Tenant.TenantCode,
                    Role = user.UserRole.Name
                }
            };
        }

        private string GenerateTenantCode(string companyName)
        {
            var prefix = new string(companyName.Where(char.IsLetter).Take(3).ToArray()).ToUpper();
            if (string.IsNullOrEmpty(prefix)) prefix = "TEN";
            
            var randomPart = new Random().Next(1000, 9999).ToString();
            return $"{prefix}{randomPart}";
        }
    }
}