using Microsoft.AspNetCore.Mvc;
using PertPlan.WebUI.Logics;
using PertPlan.WebUI.Models;
using PertPlan.WebUI.Models.ViewModels;
using System.Diagnostics;
using System.Reflection;

namespace PertPlan.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Actions()
        {
            var actions = new List<ActionPERT>();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Tasks() 
        {
            var projectTasks = new List<ProjectTask>();
            return View(projectTasks);
        }

        [HttpPost]
        public IActionResult Tasks(List<ProjectTask> projectTasks)
        {
            var logic = new HomeLogic();

            if (ModelState.IsValid)
            {
                var actions = logic.MapToActionsPERT(projectTasks);
                return RedirectToAction(nameof(Actions));
            }

            // Dane nie przeszły walidacji, zwróć widok z błędami
            return View(projectTasks);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Test()
        {
            // Przykładowe dane dla diagramu PERT
            var tasks = new[]
            {
            new { Id = "A", Name = "Task A", Dependencies = new[] { "B" } },
            new { Id = "B", Name = "Task B", Dependencies = new[] { "C" } },
            new { Id = "C", Name = "Task C", Dependencies = new[] { "D" } },
            new { Id = "D", Name = "Task D", Dependencies = Array.Empty<string>() }
        };

            return View(tasks);
        }
    }
}