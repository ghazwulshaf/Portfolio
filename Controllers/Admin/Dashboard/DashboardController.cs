using GhazwulShaf.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GhazwulShaf.Controllers.Admin.Dashboard
{
    [Authorize(Roles = "Admin")]
    [Route("/Admin/Dashboard/")]
    public class DashboardController : Controller
    {
        // GET: DashboardController
        [HttpGet]
        public ActionResult Index()
        {
            return View("/Views/Admin/Dashboard/Index.cshtml");
        }
    }
}
