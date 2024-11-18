using Microsoft.AspNetCore.Mvc;

namespace GhazwulShaf.Controllers.Admin.Auth
{
    [Route("/Auth/Login/")]
    public class LoginController : Controller
    {
        // GET: LoginController
        public ActionResult Index()
        {
            return View("/Views/Admin/Auth/Login.cshtml");
        }

    }
}
