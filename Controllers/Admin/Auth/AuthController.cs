using System.Security.Claims;
using GhazwulShaf.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GhazwulShaf.Controllers.Admin.Auth
{
    [Route("/Admin/Auth/")]
    public class AuthController : Controller
    {
        public readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
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
            if (username == "admin" && password == "admin123")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "Dashboard");
            }

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
