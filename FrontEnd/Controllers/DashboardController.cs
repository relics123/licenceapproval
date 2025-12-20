using Microsoft.AspNetCore.Mvc;
using FrontEnd.Helpers;
using FrontEnd.Models;
using FrontEnd.Services;

namespace FrontEnd.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILicenseService _licenseService;

        public DashboardController(ILicenseService licenseService)
        {
            _licenseService = licenseService;
        }

        public async Task<IActionResult> Index()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Login", "Account");
            }

            var userInfo = SessionHelper.GetUserInfo(HttpContext.Session);

            var viewModel = new DashboardViewModel
            {
                CurrentUser = userInfo,
                RecentLicenses = new List<LicenseInfo>()
            };

            // Get licenses based on role
            List<LicenseInfo> licenses;
            if (userInfo.Role == "TenantAdmin")
            {
                // Admin sees all licenses in tenant
                licenses = await _licenseService.GetLicensesAsync(userInfo.TenantId, null);
            }
            else
            {
                // Regular user sees only their licenses
                licenses = await _licenseService.GetLicensesAsync(userInfo.TenantId, null);
            }

            viewModel.RecentLicenses = licenses.Take(5).ToList();

            // Get active license for current user
            viewModel.ActiveLicense = await _licenseService.GetActiveLicenseAsync(userInfo.UserId, userInfo.UserId);

            // Calculate stats
            viewModel.Stats = new DashboardStats
            {
                TotalLicenses = licenses.Count,
                ActiveLicenses = licenses.Count(l => l.Status == "Approved" && l.ExpiryDate > DateTime.UtcNow),
                ExpiredLicenses = licenses.Count(l => l.Status == "Expired"),
                PendingPayment = licenses.Count(l => l.Status == "PaymentPending")
            };

            return View(viewModel);
        }
    }
}