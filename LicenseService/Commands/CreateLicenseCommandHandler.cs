using MediatR;
using LicenseService.Data;
using LicenseService.Models;
using Microsoft.EntityFrameworkCore;

namespace LicenseService.Commands
{
    public class CreateLicenseCommandHandler : IRequestHandler<CreateLicenseCommand, CreateLicenseResult>
    {
        private readonly AppDbContext _context;

        public CreateLicenseCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CreateLicenseResult> Handle(CreateLicenseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Generate unique license number
                var licenseNumber = await GenerateLicenseNumber();

                var license = new License
                {
                    LicenseNumber = licenseNumber,
                    ApplicantName = request.ApplicantName,
                    GstNumber = request.GstNumber,
                    CompanyName = request.CompanyName,
                    City = request.City,
                    Status = "PaymentPending",
                    ApplicationDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddYears(1),
                    PaymentCompleted = false,
                    PaymentAmount = 5000.00m,
                    DocumentGenerated = false,
                    TenantId = request.TenantId,
                    UserId = request.UserId,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Licenses.Add(license);
                await _context.SaveChangesAsync(cancellationToken);

                return new CreateLicenseResult
                {
                    Success = true,
                    Message = "License application created successfully",
                    LicenseId = license.Id,
                    LicenseNumber = license.LicenseNumber
                };
            }
            catch (Exception ex)
            {
                return new CreateLicenseResult
                {
                    Success = false,
                    Message = $"Error creating license: {ex.Message}"
                };
            }
        }

        private async Task<string> GenerateLicenseNumber()
        {
            var year = DateTime.UtcNow.Year;
            var lastLicense = await _context.Licenses
                .Where(l => l.LicenseNumber.StartsWith($"LIC{year}"))
                .OrderByDescending(l => l.Id)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastLicense != null)
            {
                var lastNumber = lastLicense.LicenseNumber.Substring(7);
                if (int.TryParse(lastNumber, out int num))
                {
                    nextNumber = num + 1;
                }
            }

            return $"LIC{year}{nextNumber:D4}";
        }
    }
}