using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GhazwulShaf.Controllers.Admin.Dashboard
{
    [Authorize(Roles = "Admin")]
    [Route("/Admin/Dashboard/")]
    public class DashboardController : Controller
    {
        // GET: DashboardController
        public ActionResult Index()
        {
            var user = HttpContext.User.Identity?.Name;
            ViewData["User"] = user;

            return View("/Views/Admin/Dashboard/Index.cshtml");
        }

    }
}
