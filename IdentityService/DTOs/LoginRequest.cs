namespace IdentityService.DTOs
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string TenantCode { get; set; }
    }
}