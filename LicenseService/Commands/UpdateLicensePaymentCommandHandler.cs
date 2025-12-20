using MediatR;
using LicenseService.Data;
using Microsoft.EntityFrameworkCore;

namespace LicenseService.Commands
{
    public class UpdateLicensePaymentCommandHandler : IRequestHandler<UpdateLicensePaymentCommand, UpdatePaymentResult>
    {
        private readonly AppDbContext _context;

        public UpdateLicensePaymentCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UpdatePaymentResult> Handle(UpdateLicensePaymentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var license = await _context.Licenses
                    .FirstOrDefaultAsync(l => l.Id == request.LicenseId && l.TenantId == request.TenantId, cancellationToken);

                if (license == null)
                {
                    return new UpdatePaymentResult
                    {
                        Success = false,
                        Message = "License not found"
                    };
                }

                license.PaymentCompleted = true;
                license.Status = "Approved";
                license.ApprovalDate = DateTime.UtcNow;
                license.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                return new UpdatePaymentResult
                {
                    Success = true,
                    Message = "Payment completed and license approved"
                };
            }
            catch (Exception ex)
            {
                return new UpdatePaymentResult
                {
                    Success = false,
                    Message = $"Error updating payment: {ex.Message}"
                };
            }
        }
    }
}