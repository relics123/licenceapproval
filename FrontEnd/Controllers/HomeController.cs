using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FrontEnd.Models;
using FrontEnd.Helpers;
namespace FrontEnd.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }
    }
}