using MediatR;
using LicenseService.Models;

namespace LicenseService.Queries
{
    public class GetActiveLicenseQuery : IRequest<License>
    {
        public int TenantId { get; set; }
        public int UserId { get; set; }
    }
}