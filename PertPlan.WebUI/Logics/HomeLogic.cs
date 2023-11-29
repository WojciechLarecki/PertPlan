using PertPlan.WebUI.Models;
using PertPlan.WebUI.Models.ViewModels;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Linq;

namespace PertPlan.WebUI.Logics
{
    public class HomeLogic
    {
        private ActionPERT MapToActionPERT(ProjectTask task)
        {
            return new ActionPERT()
            {
                Id = task.Id,
                Name = task.Name,
                Negative = task.NegativeFinishTime,
                Average = task.AverageFinishTime,
                Positive = task.PositiveFinishTime
            };
        }

        public List<ActionPERT> MapToActionsPERT(IEnumerable<ProjectTask> tasks) 
        {
            var actions = tasks.Select(task => MapToActionPERT(task)).ToList();
            
            foreach (var action in actions)
            {
                var task = tasks.Where(x => x.Id == action.Id).First();

                if (string.IsNullOrWhiteSpace(task.DependOnTasks)) continue; //avoid start tasks for project

                var depedenceIds = task.DependOnTasks
                .Split(',')
                .Select(str => int.TryParse(str, out var num) ? num : -1)
                .ToList();

                var previousActions = actions.Where(a => depedenceIds.Contains(a.Id)).ToList();
                action.PreviousActions = previousActions;

                foreach (var previousAction in action.PreviousActions)
                {
                    if (previousAction.NextActions == null) previousAction.NextActions = new List<ActionPERT>();

                    previousAction.NextActions.Add(action);
                }
            }

            return actions;
        }

        public ActionsVM GetActionsVM(IEnumerable<ActionPERT> actions)
        {
            var viewModel = new ActionsVM();
            viewModel.RootActions = actions.Where(a => a.PreviousActions == null).ToList();
            return viewModel;
        }
    }
}
