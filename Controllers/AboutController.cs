using Microsoft.AspNetCore.Mvc;

namespace GhazwulShaf.Controllers
{
    public class AboutController : Controller
    {
        // GET: AboutController
        public IActionResult Index()
        {
            return View("/Views/Home/About/Index.cshtml");
        }

    }
}
