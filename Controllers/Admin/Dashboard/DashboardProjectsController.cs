using GhazwulShaf.Models;
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

        [HttpGet]
        [Route("Add")]
        public IActionResult Add()
        {
            return View("/Views/Admin/Dashboard/Projects/Project.cshtml");
        }

        [HttpGet]
        [Route("Tags/Add")]
        public IActionResult AddTag(int itemId, string itemName)
        {
            ViewBag.ItemId = itemId;
            ViewBag.ItemName = itemName;

            return PartialView("/Views/Admin/Dashboard/Projects/_Tag.cshtml", new Project());
        }
    }
}
