using PertPlan.WebUI.Models.ViewModels;

namespace PertPlan.WebUI.Models.Helpers
{
    /// <summary>
    /// Klasa pomocnicza zawierająca metody mapowania obiektów z jednego typu na drugi.
    /// </summary>
    public static class Mapper
    {
        /// <summary>
        /// Mapuje listę zadań projektowych na listę działań PERT.
        /// </summary>
        /// <param name="tasks">Lista zadań projektowych do zmapowania.</param>
        /// <returns>Lista działań PERT zmapowanych z zadań projektowych.</returns>
        public static List<ActionPERT> MapToActionsPERT(IEnumerable<ProjectTask> tasks)
        {
            var actions = tasks.Select(task => MapToActionPERT(task)).ToList();

            foreach (var action in actions)
            {
                var task = tasks.Where(x => x.Id == action.Id).First();

                if (task.DependOnTasks == "x") continue; //avoid start tasks for project

                var depedenceIds = task.DependOnTasks!
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

        /// <summary>
        /// Mapuje pojedyncze zadanie projektowe na działanie PERT.
        /// </summary>
        /// <param name="task">Zadanie projektowe do zmapowania.</param>
        /// <returns>Działanie PERT zmapowane z zadania projektowego.</returns>
        private static ActionPERT MapToActionPERT(ProjectTask task)
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
    }
}