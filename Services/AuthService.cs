using System.Security.Claims;
using GhazwulShaf.Data;
using GhazwulShaf.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace GhazwulShaf.Services;

public class AuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> LoginAsync(string username, string password, HttpContext httpContext)
    {
        // Ambil data user dari database
        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        if (user == null)
            return false;

        // Verifikasi password
        var passwordHasher = new PasswordHasher<User>();
        var result = passwordHasher.VerifyHashedPassword(user, user.Password, password);
        if (result != PasswordVerificationResult.Success)
            return false;
        
        // Tambahkan klaim
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true
        };

        await httpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

        // set auth user attributes
        AuthUser.Username = user.Username;
        AuthUser.Password = user.Password;
        AuthUser.Role = user.Role;

        return true;
    }
}