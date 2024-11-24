using GhazwulShaf.Models;
using GhazwulShaf.Services;
using Microsoft.AspNetCore.Mvc;

namespace GhazwulShaf.Controllers.Home
{
    public class AboutController : Controller
    {
        private readonly AboutProfileService _profileService;

        public AboutController(AboutProfileService profileService)
        {
            _profileService = profileService;
        }

        // GET: AboutController
        public async Task<IActionResult> Index()
        {
            return View("/Views/Home/About/Index.cshtml",
                new About {
                    Profile = await _profileService.GetAsync()
                }    
            );
        }

    }
}
