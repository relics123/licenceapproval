namespace FrontEnd.Models
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public UserInfo User { get; set; }
    }

    public class UserInfo
    {
        public int UserId { get; set; }
        public string Username { get; set; }

        public int TenantId { get; set; } // ✅ ADD
        public string Email { get; set; }
        public string FullName { get; set; }
        public string TenantCode { get; set; }
        public string Role { get; set; }
    }
}