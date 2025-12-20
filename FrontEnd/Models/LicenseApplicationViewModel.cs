using System.ComponentModel.DataAnnotations;

namespace FrontEnd.Models
{
    public class LicenseApplicationViewModel
    {
        [Required(ErrorMessage = "Applicant name is required")]
        public string ApplicantName { get; set; }

        [Required(ErrorMessage = "GST number is required")]
        [RegularExpression(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$", 
            ErrorMessage = "Invalid GST number format")]
        public string GstNumber { get; set; }

        [Required(ErrorMessage = "Company name is required")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }
    }
}