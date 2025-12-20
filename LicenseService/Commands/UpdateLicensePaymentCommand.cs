using MediatR;

namespace LicenseService.Commands
{
    public class UpdateLicensePaymentCommand : IRequest<UpdatePaymentResult>
    {
        public int LicenseId { get; set; }
        public int TenantId { get; set; }
    }

    public class UpdatePaymentResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}