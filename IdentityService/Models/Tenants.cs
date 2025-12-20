namespace IdentityService.Models
{
    public class Tenant
    {
        public int Id { get; set; }
        public string TenantCode { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        
        public ICollection<User> Users { get; set; }
    }
}