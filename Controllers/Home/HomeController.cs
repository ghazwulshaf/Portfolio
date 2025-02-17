using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GhazwulShaf.Models;
using GhazwulShaf.Services;

namespace GhazwulShaf.Controllers.Home;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MasterdataService _masterdataService;
    private readonly AboutProfileService _profileService;
    private readonly ProjectsService _projectsService;

    public HomeController(
        ILogger<HomeController> logger,
        MasterdataService masterdataService,
        AboutProfileService profileService,
        ProjectsService projectsService
    ) {
        _logger = logger;
        _masterdataService = masterdataService;
        _profileService = profileService;
        _projectsService = projectsService;

        var dataDirecotry = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data");
        if (!Directory.Exists(dataDirecotry))
            Directory.CreateDirectory(dataDirecotry);
    }

    public async Task<IActionResult> Index()
    {
        var masterdata = await _masterdataService.GetAsync();
        var profile = await _profileService.GetAsync();
        var projects = await _projectsService.GetAllAsync();

        ViewBag.Welcome = masterdata.Welcome;
        ViewBag.AboutDescription = profile.Description;
        ViewBag.Projects = projects.OrderByDescending(p => p.Id)
            .Take(3)
            .ToList();

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
