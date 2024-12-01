using Microsoft.AspNetCore.Mvc;

namespace GhazwulShaf.Controllers
{
    public class ViewController : Controller
    {
        // GET: ViewController
        public IActionResult LoadPartialView(string viewName, dynamic data)
        {
            return PartialView("/Views/Shared/" + viewName, new { Data = data});
        }

        public IActionResult LoadPartialScript(string scriptName)
        {
            return File("/Views/Shared/" + scriptName, "application/javascript");
        }
    }
}
