using GhazwulShaf.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GhazwulShaf.Controllers.Admin.Auth
{
    [Route("/Admin/Auth/")]
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // GET: Login
        [HttpGet]
        [Route("Login")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            // Periksa apakah pengguna sudah login
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // Redirect ke halaman dashboard jika sudah login
                return RedirectToAction("Index", "Dashboard");
            }

            return View("/Views/Admin/Auth/Login.cshtml");
        }

        // POST: Login
        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (ModelState.IsValid && await _authService.LoginAsync(username, password, HttpContext))
                return RedirectToAction("Index", "Dashboard");

            ViewData["Login Error"] = "Invalid username or password.";
            return View("/Views/Admin/Auth/Login.cshtml");
        }

        // POST: Logout
        [HttpPost]
        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login");
        }

    }
}
