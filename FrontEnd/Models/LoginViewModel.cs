using System.ComponentModel.DataAnnotations;

namespace FrontEnd.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Tenant code is required")]
        public string TenantCode { get; set; }
    }
}