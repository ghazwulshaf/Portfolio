using GhazwulShaf.Models;
using Microsoft.AspNetCore.Identity;

namespace GhazwulShaf.Data;

public static class DbSeeder
{
    public static void Seed(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Pastikan database telah dibuat
        db.Database.EnsureCreated();

        // Seed data jika belum ada
        if (!db.Users.Any())
        {
            db.Users.Add(
                new User { Username = "admin", Password = HashPassword("password123"), Role = "Admin" }
            );

            db.SaveChanges();
        }
    }

    public static string HashPassword(string password)
    {
        var hasher = new PasswordHasher<User>();
        return hasher.HashPassword(new User(), password);
    }
}