using GhazwulShaf.Models;
using GhazwulShaf.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GhazwulShaf.Controllers.Admin.Dashboard
{
    [Authorize(Roles = "Admin")]
    [Route("/Admin/Dashboard/Contact")]
    public class DashboardContactController : Controller
    {
        private readonly ContactService _contactService;

        public DashboardContactController(ContactService contactService)
        {
            _contactService = contactService;
        }

        // GET: DashboardContactController
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var contact = await _contactService.GetAsync();

            return View("/Views/Admin/Dashboard/Contact/Index.cshtml", contact);
        }

        // POST: Update
        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> Update(Contact contact)
        {
            await _contactService.UpdateAsync(contact);
            return RedirectToAction(nameof(Index));
        }

        // GET: Render Links Item
        [HttpGet]
        [Route("Links/Add")]
        public ActionResult AddLinksItem(int itemId)
        {
            ViewBag.ItemId = itemId;
            return PartialView("/Views/Admin/Dashboard/Contact/_LinksItem.cshtml", new Contact());
        }
    }
}
