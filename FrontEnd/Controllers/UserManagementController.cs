using Microsoft.AspNetCore.Mvc;
using FrontEnd.Helpers;
using FrontEnd.Models;
using FrontEnd.Services;

namespace FrontEnd.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly IUserManagementService _userManagementService;

        public UserManagementController(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Login", "Account");
            }

            var userInfo = SessionHelper.GetUserInfo(HttpContext.Session);
            
            if (userInfo.Role != "TenantAdmin")
            {
                TempData["ErrorMessage"] = "Only administrators can manage users";
                return RedirectToAction("Index", "Dashboard");
            }

            var users = await _userManagementService.GetUsersAsync(userInfo.TenantId);

            var viewModel = new UserListViewModel
            {
                Users = users
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Login", "Account");
            }

            var userInfo = SessionHelper.GetUserInfo(HttpContext.Session);
            
            if (userInfo.Role != "TenantAdmin")
            {
                TempData["ErrorMessage"] = "Only administrators can create users";
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

//         [HttpPost]
//         public async Task<IActionResult> Create(CreateUserViewModel model)
//         {
//             if (!SessionHelper.IsAuthenticated(HttpContext.Session))
//             {
//                 return RedirectToAction("Login", "Account");
//             }

//             var userInfo = SessionHelper.GetUserInfo(HttpContext.Session);

//             if (userInfo.Role != "TenantAdmin")
//             {
//                 TempData["ErrorMessage"] = "Only administrators can create users";
//                 return RedirectToAction("Index", "Dashboard");
//             }

//             if (!ModelState.IsValid)
//             {
//                 return View(model);
//             }

//             try{
//             var success = await _userManagementService.CreateUserAsync(model, userInfo.TenantId);

//             if (success)
//             {
//                 TempData["SuccessMessage"] = "User created successfully";
//                 return RedirectToAction("Index");
//             }

           
//             }
//             catch (Exception ex)
//             {
//  ModelState.AddModelError("", "Failed to create user. Username might already exist.");
// // ✅ Add debug info to ModelState
//     ModelState.AddModelError("DEBUG", $"TenantId = {userInfo.TenantId}");
//     ModelState.AddModelError("DEBUG", $"UserId = {userInfo.UserId}");
//     ModelState.AddModelError("DEBUG", $"Role = {userInfo.Role}");
//     ModelState.AddModelError("DEBUG", $"Calling CreateUserAsync with TenantId = {userInfo.TenantId}");


                
//             }
//             return View(model);
//         }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Login", "Account");
            }

            var userInfo = SessionHelper.GetUserInfo(HttpContext.Session);
            
            if (userInfo.Role != "TenantAdmin")
            {
                TempData["ErrorMessage"] = "Only administrators can edit users";
                return RedirectToAction("Index", "Dashboard");
            }

            var users = await _userManagementService.GetUsersAsync(userInfo.TenantId);
            var user = users.FirstOrDefault(u => u.UserId == id);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found";
                return RedirectToAction("Index");
            }

            var model = new EditUserViewModel
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                IsActive = user.IsActive
            };

            return View(model);
        }

[HttpPost]
public async Task<IActionResult> Create(CreateUserViewModel model)
{
    if (!SessionHelper.IsAuthenticated(HttpContext.Session))
    {
        return RedirectToAction("Login", "Account");
    }

    var userInfo = SessionHelper.GetUserInfo(HttpContext.Session);

    if (userInfo.Role != "TenantAdmin")
    {
        TempData["ErrorMessage"] = "Only administrators can create users";
        return RedirectToAction("Index", "Dashboard");
    }

    if (!ModelState.IsValid)
    {
        // ✅ Add debug info when validation fails
        ViewBag.DebugTenantId = userInfo.TenantId;
        ViewBag.DebugUserId = userInfo.UserId;
        return View(model);
    }

    try
    {
        // ✅ Call service with TenantId (not UserId!)
        var success = await _userManagementService.CreateUserAsync(model, userInfo.TenantId);

        if (success)
        {
            TempData["SuccessMessage"] = $"User '{model.Username}' created successfully";
            return RedirectToAction("Index");
        }

        // Failed but no exception - show generic error
        ModelState.AddModelError("", "Failed to create user. Please check the details and try again.");
        ViewBag.DebugTenantId = userInfo.TenantId;
        ViewBag.DebugUserId = userInfo.UserId;
        return View(model);
    }
    catch (Exception ex)
    {
        // Log exception (you should use ILogger here)
        ModelState.AddModelError("", $"Error: {ex.Message}");
        ViewBag.DebugTenantId = userInfo.TenantId;
        ViewBag.DebugUserId = userInfo.UserId;
        return View(model);
    }
}

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Login", "Account");
            }

            var userInfo = SessionHelper.GetUserInfo(HttpContext.Session);

            if (userInfo.Role != "TenantAdmin")
            {
                TempData["ErrorMessage"] = "Only administrators can edit users";
                return RedirectToAction("Index", "Dashboard");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var success = await _userManagementService.UpdateUserAsync(model, userInfo.UserId);

            if (success)
            {
                TempData["SuccessMessage"] = "User updated successfully";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Failed to update user");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!SessionHelper.IsAuthenticated(HttpContext.Session))
            {
                return RedirectToAction("Login", "Account");
            }

            var userInfo = SessionHelper.GetUserInfo(HttpContext.Session);

            if (userInfo.Role != "TenantAdmin")
            {
                TempData["ErrorMessage"] = "Only administrators can delete users";
                return RedirectToAction("Index", "Dashboard");
            }

            var success = await _userManagementService.DeleteUserAsync(id, userInfo.UserId);

            if (success)
            {
                TempData["SuccessMessage"] = "User deleted successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete user";
            }

            return RedirectToAction("Index");
        }
    }
}