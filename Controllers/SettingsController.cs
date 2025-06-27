using Microsoft.AspNetCore.Mvc;

namespace STD.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Settings/Index.cshtml");
        }

        [HttpGet]
        public IActionResult GetPage(string PageName)
        {
            return View($"~/Views/Settings/{PageName}/Index.cshtml");
        }
    }
}