using Microsoft.AspNetCore.Mvc;

namespace PertPlan.WebUI.Controllers
{
    public class ExportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
