using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GhazwulShaf.Controllers.Admin.Dashboard
{
    [Route("/Admin/Dashboard/")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        // GET: DashboardController
        public ActionResult Index()
        {
            return View("/Views/Admin/Dashboard/Index.cshtml");
        }

    }
}
