using GhazwulShaf.Services;
using Microsoft.AspNetCore.Mvc;

namespace GhazwulShaf.Controllers.Home
{
    public class ContactController : Controller
    {
        private readonly ContactService _contactService;

        public ContactController(ContactService contactService)
        {
            _contactService = contactService;
        }

        // GET: ContactController
        public async Task<IActionResult> Index()
        {
            var contact = await _contactService.GetAsync();
            return View("/Views/Home/Contact/Index.cshtml", contact);
        }

    }
}
