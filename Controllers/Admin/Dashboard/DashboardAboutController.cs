using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GhazwulShaf.Controllers.Admin.Dashboard
{
    [Route("/Admin/Dashboard/About")]
    [Authorize(Roles = "Admin")]
    public class DashboardAboutController : Controller
    {
        // GET: DashboardAboutController
        [HttpGet]
        public ActionResult Index()
        {
            return View("/Views/Admin/Dashboard/About/Index.cshtml");
        }

    }
}
