using GhazwulShaf.Models;
using Microsoft.EntityFrameworkCore;

namespace GhazwulShaf.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<User> Users { get; set; } = default!;
}