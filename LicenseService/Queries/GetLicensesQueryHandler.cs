using MediatR;
using LicenseService.Data;
using LicenseService.Models;
using Microsoft.EntityFrameworkCore;

namespace LicenseService.Queries
{
    public class GetLicensesQueryHandler : IRequestHandler<GetLicensesQuery, List<License>>
    {
        private readonly AppDbContext _context;

        public GetLicensesQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<License>> Handle(GetLicensesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Licenses.Where(l => l.TenantId == request.TenantId);

            if (request.UserId.HasValue)
            {
                query = query.Where(l => l.UserId == request.UserId.Value);
            }

            return await query
                .OrderByDescending(l => l.CreatedDate)
                .ToListAsync(cancellationToken);
        }
    }
}