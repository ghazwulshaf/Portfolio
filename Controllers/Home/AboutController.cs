using GhazwulShaf.Models;
using GhazwulShaf.Services;
using Microsoft.AspNetCore.Mvc;

namespace GhazwulShaf.Controllers.Home
{
    [Route("/about")]
    public class AboutController : Controller
    {
        private readonly AboutProfileService _profileService;
        private readonly AboutSectionService _sectionService;

        public AboutController(AboutProfileService profileService, AboutSectionService sectionService)
        {
            _profileService = profileService;
            _sectionService = sectionService;
        }

        // GET: AboutController
        public async Task<IActionResult> Index()
        {
            return View("/Views/Home/About/Index.cshtml",
                new About {
                    Profile = await _profileService.GetAsync(),
                    Sections = await _sectionService.GetAllAsync()
                }    
            );
        }
    }
}
