using System;
using Microsoft.AspNetCore.Mvc;
using PertPlan.WebUI.Logics;
using PertPlan.WebUI.Models.Helpers;
using PertPlan.WebUI.Models.ViewModels;

namespace PertPlan.WebUI.Controllers
{
    public class TasksController : Controller
    {
        // endpoint for task creation
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Index(List<ProjectTask> projectTasks)
        {
            if (ModelState.IsValid)
            {
                if (projectTasks is null)
                    throw new ArgumentNullException("ProjectTasks are null");

                if (projectTasks.Count == 0)
                    throw new ArgumentException("There aren't any project tasks");

                for (int i = 0; i < projectTasks.Count; i++)
                {
                    projectTasks[i].Id = i;
                }

                var projectTasksCopy = projectTasks.Select(pt => pt.Copy()).ToList();
                var actionsCopy = Mapper.MapToActionsPERT(projectTasksCopy);
                var logic = new HomeLogic();
                var viewModel = logic.GetTaskPostVM(projectTasks);
                viewModel.TableVM = new TaskPostTableVM(actionsCopy);
                return View("Actions", viewModel);
            }

            // Data is invalid, return empty view
            return View(projectTasks);
        }

        [HttpPost]
        public IActionResult UploadFile(IFormFile csvFile)
        {
            var projectTasks = new List<ProjectTask>();

            if (csvFile == null || csvFile.Length == 0)
            {
                // ModelState.AddModelError("csvFile", "Nie wybrano pliku lub plik jest pusty.");
                TempData["ErrorMessage"] = "Nie wybrano pliku lub plik jest pusty.";
                return RedirectToAction("Index", "Home");
            }

            using (var reader = new StreamReader(csvFile.OpenReadStream()))
            {
                string? line;
                _ = reader.ReadLine(); //headers line
                var lineIndex = 1;
                try
                {
                    var random = new Random();
                    do
                    {
                        line = reader.ReadLine();
                        lineIndex++;

                        if (line is null) break;

                        var properties = line.Split(';');

                        int taskNumber;
                        double taskPositiveFinishTime;
                        double taskAverageFinishTime;
                        double taskNegativeFinishTime;

                        if (properties.Length != 6)
                        {
                            throw new Exception($"Line {lineIndex}: File has wrong structure.");
                        }
                        else if (!int.TryParse(properties[0], out taskNumber))
                        {
                            throw new Exception($"Line {lineIndex}: Task number is not a number.");
                        }
                        else if (!double.TryParse(properties[2], out taskPositiveFinishTime))
                        {
                            throw new Exception($"Line {lineIndex}: Positive time is not a number.");
                        }
                        else if (!double.TryParse(properties[3], out taskAverageFinishTime))
                        {
                            throw new Exception($"Line {lineIndex}: Average time is not a number.");
                        }
                        else if (!double.TryParse(properties[4], out taskNegativeFinishTime))
                        {
                            throw new Exception($"Line {lineIndex}: Negative time is not a number.");
                        }

                        try
                        {
                            Validator.ValidateTaskName(properties[1]);
                            Validator.ValidateTaskAverageTime(taskPositiveFinishTime, taskAverageFinishTime, taskNegativeFinishTime);
                            Validator.ValidateNegativeTimeInput(taskAverageFinishTime, taskNegativeFinishTime);
                        }
                        catch(ArgumentException e)
                        {
                            throw new Exception($"Line {lineIndex}: {e.Message}");
                        }

                        var task = new ProjectTask
                        {
                            Id = random.Next(0, 1000000),
                            Name = properties[1],
                            PositiveFinishTime = taskPositiveFinishTime,
                            AverageFinishTime = taskAverageFinishTime,
                            NegativeFinishTime = taskNegativeFinishTime
                        };

                        if (!string.IsNullOrEmpty(properties[5]))
                        {
                            Validator.ValidateDependentTasksInput(properties[5], taskNumber);
                            task.DependOnTasks = properties[5];
                        }

                        projectTasks.Add(task);
                    } while (line != null);
                }
                catch (Exception e)
                {
                    TempData["ErrorMessage"] = e.Message;
                    return RedirectToAction("Index", "Home");
                }
                
            }

            return View("Index", projectTasks);
        }
    }
}
