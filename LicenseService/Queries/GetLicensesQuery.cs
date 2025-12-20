using MediatR;
using LicenseService.Models;

namespace LicenseService.Queries
{
    public class GetLicensesQuery : IRequest<List<License>>
    {
        public int TenantId { get; set; }
        public int? UserId { get; set; }
    }
}