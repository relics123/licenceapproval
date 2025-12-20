using MediatR;

namespace LicenseService.Commands
{
    public class CreateLicenseCommand : IRequest<CreateLicenseResult>
    {
        public string ApplicantName { get; set; }
        public string GstNumber { get; set; }
        public string CompanyName { get; set; }
        public string City { get; set; }
        public int TenantId { get; set; }
        public int UserId { get; set; }
    }

    public class CreateLicenseResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int LicenseId { get; set; }
        public string LicenseNumber { get; set; }
    }
}