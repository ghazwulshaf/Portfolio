using Microsoft.AspNetCore.Mvc;

namespace GhazwulShaf.Controllers.Admin.Dashboard
{
    [Route("/Admin/Dashboard/")]
    public class DashboardController : Controller
    {
        // GET: DashboardController
        public ActionResult Index()
        {
            return View("/Views/Admin/Dashboard/Index.cshtml");
        }

    }
}
