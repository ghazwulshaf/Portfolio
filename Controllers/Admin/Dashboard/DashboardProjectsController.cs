using Microsoft.AspNetCore.Mvc;

namespace GhazwulShaf.Controllers.Admin.Dashboard
{
    [Route("/Admin/Dashboard/Projects")]
    public class DashboardProjectsController : Controller
    {
        // GET: DashbaordProjectsController
        [HttpGet]
        public ActionResult Index()
        {
            return View("/Views/Admin/Dashboard/Projects/Index.cshtml");
        }

    }
}
