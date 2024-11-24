using Microsoft.AspNetCore.Mvc;

namespace GhazwulShaf.Controllers.Home
{
    public class ContactController : Controller
    {
        // GET: ContactController
        public ActionResult Index()
        {
            return View("/Views/Home/Contact/Index.cshtml");
        }

    }
}
