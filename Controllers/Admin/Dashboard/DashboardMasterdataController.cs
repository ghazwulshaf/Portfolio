using GhazwulShaf.Models;
using GhazwulShaf.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GhazwulShaf.Controllers.Admin.Dashboard
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Dashboard/Masterdata")]
    public class DashboardMasterdataController : Controller
    {
        private readonly MasterdataService _masterdataService;

        public DashboardMasterdataController(MasterdataService masterdataService)
        {
            _masterdataService = masterdataService;
        }

        // GET: Dashboard Masterdata Page
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var masterdata = await _masterdataService.GetAsync();

            return View("/Views/Admin/Dashboard/Masterdata/Index.cshtml", masterdata);
        }

        // POST: Update Masterdata
        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> Update(Masterdata masterdata)
        {
            await _masterdataService.UpdateAsync(masterdata);

            return RedirectToAction(nameof(Index));
        }

        // GET: Project Types New Item Partial
        [HttpGet]
        [Route("Types/Add")]
        public IActionResult AddTypesItem(int typeId)
        {
            ViewBag.TypeId = typeId;

            return PartialView("/Views/Admin/Dashboard/Masterdata/_ProjectType.cshtml", new Masterdata());
        }
    }
}
