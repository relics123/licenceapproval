

namespace IdentityService.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }
        
        public int RoleId { get; set; }
        public UserRole Role { get; set; }
    }
}