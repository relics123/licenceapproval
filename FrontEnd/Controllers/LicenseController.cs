using Microsoft.AspNetCore.Mvc;
using FrontEnd.Helpers;
using FrontEnd.Models;
using FrontEnd.Services;

namespace WebApp.Controllers
{
    public class LicenseController : Controller
    {
        private readonly ILicenseService _licenseService;
        private readonly IPaymentService _paymentService;
        private readonly IDocumentService _documentService;

        public LicenseController(
            ILicenseService licenseService,
            IPaymentService paymentService,
            IDocumentService documentService)
        {
            _licenseService = licenseService;
            _paymentService = paymentService;
            _documentService = documentService;
        }

        [HttpGet]
        public IActionResult Generate()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Login", "Account");
            }

            var userInfo = SessionHelper.GetUserInfo(HttpContext.Session);
            
            if (userInfo.Role != "TenantAdmin")
            {
                TempData["ErrorMessage"] = "Only administrators can generate licenses";
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Generate(LicenseApplicationViewModel model)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Login", "Account");
            }

            var userInfo = SessionHelper.GetUserInfo(HttpContext.Session);

            if (userInfo.Role != "TenantAdmin")
            {
                TempData["ErrorMessage"] = "Only administrators can generate licenses";
                return RedirectToAction("Index", "Dashboard");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }


    // ✅ Print all userInfo parameters
    Console.WriteLine("═══════════════════════════════════════");
    Console.WriteLine("CreateLicenseAsync - Parameters:");
    Console.WriteLine("═══════════════════════════════════════");
    Console.WriteLine($"userInfo.TenantId: {userInfo.TenantId}");
    Console.WriteLine($"userInfo.UserId: {userInfo.UserId}");
    Console.WriteLine($"userInfo.Username: {userInfo.Username}");
    Console.WriteLine($"userInfo.Email: {userInfo.Email}");
    Console.WriteLine($"userInfo.FullName: {userInfo.FullName}");
    Console.WriteLine($"userInfo.TenantCode: {userInfo.TenantCode}");
    Console.WriteLine($"userInfo.Role: {userInfo.Role}");
    Console.WriteLine("═══════════════════════════════════════");

            var license = await _licenseService.CreateLicenseAsync(model, userInfo.TenantId, userInfo.UserId);

            if (license == null)
            {
                ModelState.AddModelError("", "Failed to create license application");
                return View(model);
            }

            TempData["SuccessMessage"] = $"License application created successfully. License Number: {license.LicenseNumber}";
            return RedirectToAction("Payment", new { licenseId = license.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Payment(int licenseId)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Login", "Account");
            }

            var userInfo = SessionHelper.GetUserInfo(HttpContext.Session);
            var license = await _licenseService.GetLicenseByIdAsync(licenseId, userInfo.UserId);

            if (license == null)
            {
                TempData["ErrorMessage"] = "License not found";
                return RedirectToAction("Index", "Dashboard");
            }

            var viewModel = new PaymentViewModel
            {
                LicenseId = license.Id,
                LicenseNumber = license.LicenseNumber,
                ApplicantName = license.ApplicantName,
                CompanyName = license.CompanyName,
                Amount = license.PaymentAmount,
                TenantId = license.TenantId
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(int licenseId)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Login", "Account");
            }

            var userInfo = SessionHelper.GetUserInfo(HttpContext.Session);
            var license = await _licenseService.GetLicenseByIdAsync(licenseId, userInfo.UserId);

            if (license == null)
            {
                TempData["ErrorMessage"] = "License not found";
                return RedirectToAction("Index", "Dashboard");
            }

            // Process payment
            var paymentSuccess = await _paymentService.MakePaymentAsync(license.Id, license.TenantId, license.PaymentAmount);

            if (!paymentSuccess)
            {
                TempData["ErrorMessage"] = "Payment processing failed";
                return RedirectToAction("Payment", new { licenseId = license.Id });
            }

            // Update license status
            var updateSuccess = await _licenseService.UpdatePaymentAsync(license.Id, license.TenantId);

            if (!updateSuccess)
            {
                TempData["ErrorMessage"] = "Failed to update license status";
                return RedirectToAction("Payment", new { licenseId = license.Id });
            }

            // Get updated license
            license = await _licenseService.GetLicenseByIdAsync(licenseId, userInfo.UserId);

            // Generate document
            var documentGenerated = await _documentService.GenerateDocumentAsync(license);

            if (!documentGenerated)
            {
                TempData["WarningMessage"] = "Payment successful but document generation failed";
            }
            else
            {
                TempData["SuccessMessage"] = "Payment successfully completed and license document generated!";
            }

            return RedirectToAction("ViewLicenses");
        }

        [HttpGet]
        public async Task<IActionResult> ViewLicenses()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Login", "Account");
            }

            var userInfo = SessionHelper.GetUserInfo(HttpContext.Session);

            List<LicenseInfo> licenses;
            if (userInfo.Role == "TenantAdmin")
            {
                licenses = await _licenseService.GetLicensesAsync(userInfo.TenantId, null);
            }
            else
            {
                licenses = await _licenseService.GetLicensesAsync(userInfo.TenantId,null);
            }

            return View(licenses);
        }

        [HttpGet]
        public async Task<IActionResult> Download(int licenseId)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Login", "Account");
            }

            var userInfo = SessionHelper.GetUserInfo(HttpContext.Session);
            var license = await _licenseService.GetLicenseByIdAsync(licenseId, userInfo.UserId);

            if (license == null)
            {
                TempData["ErrorMessage"] = "License not found";
                return RedirectToAction("ViewLicenses");
            }

            if (license.Status != "Approved")
            {
                TempData["ErrorMessage"] = "License is not approved yet";
                return RedirectToAction("ViewLicenses");
            }

            var fileBytes = await _documentService.DownloadDocumentAsync(licenseId, userInfo.UserId);

            if (fileBytes == null)
            {
                TempData["ErrorMessage"] = "Document not found";
                return RedirectToAction("ViewLicenses");
            }

            return File(fileBytes, "application/pdf", $"License_{license.LicenseNumber}.pdf");
        }
    }
}