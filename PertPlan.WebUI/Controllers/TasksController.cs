using Microsoft.AspNetCore.Mvc;
using PertPlan.WebUI.Logics;
using PertPlan.WebUI.Models.ViewModels;

namespace PertPlan.WebUI.Controllers
{
    public class TasksController : Controller
    {
        // endpoint for task creation
        [HttpGet]
        public IActionResult Index()
        {
            var projectTasks = new List<ProjectTask>();
            return View(projectTasks);
        }


        [HttpPost]
        public IActionResult Tasks(List<ProjectTask> projectTasks)
        {

            if (ModelState.IsValid)
            {
                var logic = new HomeLogic();
                var viewModel = logic.GetTaskPostVM(projectTasks);
                return View("Actions", viewModel);
            }

            // Data is invalid, return empty view
            return View(projectTasks);
        }
    }
}
