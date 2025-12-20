using Microsoft.AspNetCore.Mvc;
using FrontEnd.Models;
using FrontEnd.Services;
using FrontEnd.Helpers;

namespace FrontEnd.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _authService.RegisterAsync(model);

            if (response.Success)
            {
                SessionHelper.SetToken(HttpContext.Session, response.Token);
                SessionHelper.SetUserInfo(HttpContext.Session, response.User);

                TempData["SuccessMessage"] = $"Registration successful! Your Tenant Code is: {response.User.TenantCode}";
                return RedirectToAction("Index", "Dashboard");
            }

            ModelState.AddModelError("", response.Message);
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

           var result = await _authService.LoginAsync(model);

    if (result.Success)
    {
        SessionHelper.SetUserInfo(HttpContext.Session, new UserInfo
        {
            UserId = result.User.UserId,
            TenantId = result.User.TenantId, // ✅ TEMPORARY FIX - This should be actual TenantId from API
            Username = result.User.Username,
            Email = result.User.Email,
            FullName = result.User.FullName,
            TenantCode = result.User.TenantCode,
            Role = result.User.Role
        });

        SessionHelper.SetToken(HttpContext.Session, result.Token);

        return RedirectToAction("Index", "Dashboard");
    }
            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            SessionHelper.ClearSession(HttpContext.Session);
            return RedirectToAction("Index", "Home");
        }
    }
}