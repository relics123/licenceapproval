using MediatR;
using LicenseService.Data;
using LicenseService.Models;
using Microsoft.EntityFrameworkCore;

namespace LicenseService.Queries
{
    public class GetActiveLicenseQueryHandler : IRequestHandler<GetActiveLicenseQuery, License>
    {
        private readonly AppDbContext _context;

        public GetActiveLicenseQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<License> Handle(GetActiveLicenseQuery request, CancellationToken cancellationToken)
        {
            return await _context.Licenses
                .Where(l => l.TenantId == request.TenantId &&
                           l.UserId == request.UserId && 
                           l.Status == "Approved" &&
                           l.ExpiryDate > DateTime.UtcNow)
                .OrderByDescending(l => l.ExpiryDate)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}