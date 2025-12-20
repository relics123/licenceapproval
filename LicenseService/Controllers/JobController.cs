using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LicenseService.Data;

namespace LicenseService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly AppDbContext _context;

        public JobController(AppDbContext context)
        {
            _context = context;
        }

        // Trigger expiry check manually
        [HttpPost("check-expiring-licenses")]
        public IActionResult CheckExpiringLicenses()
        {
            // Fire and forget - runs in background
            BackgroundJob.Enqueue(() => CheckAndNotifyExpiringLicenses());
            
            return Ok(new { message = "Background job started" });
        }

        // Schedule recurring job (runs daily)
        [HttpPost("setup-daily-check")]
        public IActionResult SetupDailyCheck()
        {
            // Runs every day at 2 AM
            RecurringJob.AddOrUpdate(
                "daily-license-check",
                () => CheckAndNotifyExpiringLicenses(),
                Cron.Daily(2));

            return Ok(new { message = "Daily job scheduled at 2 AM" });
        }

        // The actual job logic - simple version
        public async Task CheckAndNotifyExpiringLicenses()
        {
            var thirtyDaysFromNow = DateTime.UtcNow.AddDays(365);

            // Find licenses expiring in next 30 days
            var expiringLicenses = await _context.Licenses
                .Where(l => l.Status == "Approved" && 
                           l.ExpiryDate <= thirtyDaysFromNow &&
                           l.ExpiryDate >= DateTime.UtcNow)
                .ToListAsync();

            // Update expired licenses
            var expiredLicenses = await _context.Licenses
                .Where(l => l.Status == "Approved" && l.ExpiryDate < DateTime.UtcNow)
                .ToListAsync();

            foreach (var license in expiredLicenses)
            {
                license.Status = "Expired";
            }

            await _context.SaveChangesAsync();

            // Log results (in real app, would send emails)
            Console.WriteLine($"Found {expiringLicenses.Count} expiring licenses");
            Console.WriteLine($"Updated {expiredLicenses.Count} expired licenses");
        }
    }
}