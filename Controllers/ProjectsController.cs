using Microsoft.AspNetCore.Mvc;

namespace GhazwulShaf.Controllers
{
    public class ProjectsController : Controller
    {
        // GET: ProjectsController
        public ActionResult Index()
        {
            return View();
        }

        // GET: Project Details
        public IActionResult Detail(int id)
        {
            ViewData["Id"] = id;
            return View();
        }

    }
}
