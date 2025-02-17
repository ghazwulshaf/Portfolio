using GhazwulShaf.Data;
using GhazwulShaf.Services;
using Microsoft.EntityFrameworkCore;
using Slugify;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson();

// Add authentication and authorization service
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Admin/Auth/Login";
        options.LogoutPath = "/Admin/Auth/Logout";
    });
builder.Services.AddAuthorization();
builder.Services.AddScoped<SlugHelper>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AboutProfileService>();
builder.Services.AddScoped<AboutSectionService>();
builder.Services.AddScoped<MasterdataService>();
builder.Services.AddScoped<LearnService>();
builder.Services.AddScoped<ProjectsService>();
builder.Services.AddScoped<ContactService>();

// Configure EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Create user
DbSeeder.Seed(app.Services);

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
