using GhazwulShaf.Models;
using GhazwulShaf.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GhazwulShaf.Controllers.Admin.Dashboard
{
    [Authorize(Roles = "Admin")]
    [Route("admin/dashboard/masterdata")]
    public class DashboardMasterdataController : Controller
    {
        private readonly MasterdataService _masterdataService;

        public DashboardMasterdataController(MasterdataService masterdataService)
        {
            _masterdataService = masterdataService;
        }

        // GET: Dashboard Masterdata Page
        [HttpGet, Route("")]
        public async Task<IActionResult> Index()
        {
            var masterdata = await _masterdataService.GetAsync();

            return View("/Views/Admin/Dashboard/Masterdata/Index.cshtml", masterdata);
        }

        // POST: Update Welcome Text
        [HttpPost, Route("welcome/update")]
        public async Task<IActionResult> UpdateWelcome(Masterdata masterdata)
        {
            var welcome = masterdata.Welcome;
            await _masterdataService.UpdateWelcomeAsync(welcome);
            return RedirectToAction(nameof(Index));
        }

        // POST: Update Masterdata
        [HttpPost, Route("types/update")]
        public async Task<IActionResult> UpdateTypes(Masterdata masterdata)
        {
            var types = masterdata.Types;
            await _masterdataService.UpdateTypesAsync(types);
            return RedirectToAction(nameof(Index));
        }

        // GET: Project Types New Item Partial
        [HttpGet, Route("types/add")]
        public IActionResult AddTypesItem(int typeId)
        {
            ViewBag.TypeId = typeId;

            return PartialView("/Views/Admin/Dashboard/Masterdata/_ProjectType.cshtml", new Masterdata());
        }
    }
}
