using Microsoft.AspNetCore.Mvc;

namespace GhazwulShaf.Controllers.Home
{
    public class ProjectsController : Controller
    {
        // GET: ProjectsController
        public ActionResult Index()
        {
            return View("/Views/Home/Projects/Index.cshtml");
        }

        // GET: Project Details
        public IActionResult Detail(int id)
        {
            ViewData["Id"] = id;
            return View("/Views/Home/Projects/Detail.cshtml");
        }

    }
}
