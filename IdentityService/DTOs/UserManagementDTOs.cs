namespace IdentityService.DTOs
{
    public class CreateSubUserRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public int TenantId { get; set; }
    }

    public class UpdateUserRequest
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public bool IsActive { get; set; }
        public int TenantId { get; set; }
    }

    public class UserResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public UserDetail User { get; set; }
    }

    public class UserDetail
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class UsersListResponse
    {
        public bool Success { get; set; }
        public List<UserDetail> Users { get; set; }
    }
}