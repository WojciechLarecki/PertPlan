using PertPlan.WebUI.Models;
using PertPlan.WebUI.Models.ViewModels;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text;

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

        //public ActionsVM GetActionsVM(IEnumerable<ActionPERT> actions)
        //{
        //    var viewModel = new ActionsVM();
        //    viewModel.RootActions = actions.Where(a => a.PreviousActions == null).ToList();
        //    return viewModel;
        //}

        internal TaskPostVM GetTaskPostVM(List<ProjectTask> projectTasks)
        {
            if (projectTasks is null)
                throw new ArgumentNullException("ProjectTasks are null");

            if (projectTasks.Count == 0)
                throw new ArgumentException("There aren't any project tasks");

            var actions = MapToActionsPERT(projectTasks);

            // Tworzenie słownika, gdzie kluczem jest ID zadania, a wartością jest odpowiadający mu obiekt PDMNode
            Dictionary<int, PDMNode> pdmNodesDictionary = new Dictionary<int, PDMNode>();

            // Tworzenie obiektów PDMNode na podstawie ProjectTask
            foreach (var task in actions)
            {
                PDMNode pdmNode = new PDMNode(task.Id)
                {
                    Name = task.Name,
                    EstimatedTaskEndTime = task.Estimated
                };

                pdmNodesDictionary.Add(task.Id, pdmNode);
            }

            // set next and previous nodes
            foreach (var action in actions) 
            {
                if (action.PreviousActions != null) 
                {
                    pdmNodesDictionary[action.Id].PreviousNode = new List<PDMNode>();

                    foreach (var previousAction in action.PreviousActions)
                    {
                        pdmNodesDictionary[action.Id].PreviousNode.Add(pdmNodesDictionary[previousAction.Id]);
                    }
                }

                if (action.NextActions != null)
                {
                    pdmNodesDictionary[action.Id].NextNode = new List<PDMNode>();

                    foreach (var nextAction in action.NextActions)
                    {
                        pdmNodesDictionary[action.Id].NextNode.Add(pdmNodesDictionary[nextAction.Id]);
                    }
                }
            }

            foreach(var nodeKVP in pdmNodesDictionary)
            {
                var node = pdmNodesDictionary[nodeKVP.Key];
                if (node.PreviousNode == null) node.EarlyStart = 0;
                else node.EarlyStart = node.PreviousNode.Max(prevNode => prevNode.EarlyEnd);
            }

            for (int i = pdmNodesDictionary.Count - 1; i >= 0; --i)
            {
                var node = pdmNodesDictionary[i];
                if (node.NextNode == null) node.LateEnd = node.EarlyEnd;
                else node.LateEnd = node.NextNode.Min(prevNode => prevNode.LateStart);
            }

            //// Ustawianie relacji między zadaniami (NextNode)
            //foreach (var action in actions)
            //{
            //    if (action.NextActions != null)
            //    {
            //        foreach (var nextAction in action.NextActions)
            //        {
            //            if (pdmNodesDictionary[action.Id].NextNode == null)
            //                pdmNodesDictionary[action.Id].NextNode = new List<PDMNode>();

            //            pdmNodesDictionary[action.Id].NextNode.Append(pdmNodesDictionary[nextAction.Id]);
            //        }
            //    }


            //    if (action.PreviousActions != null) 
            //    {
            //        foreach (var previousAction in action.PreviousActions)
            //        {
            //            if (pdmNodesDictionary[action.Id].PreviousNode == null)
            //                pdmNodesDictionary[action.Id].PreviousNode = new List<PDMNode>();

            //            pdmNodesDictionary[action.Id].PreviousNode.Append(pdmNodesDictionary[previousAction.Id]);
            //        }
            //    }
            //}



            //foreach (var pdm in pdmNodesDictionary) 
            //{
            //    // sets 0 for starting tasks
            //    if (pdm.Value.PreviousNode == null)
            //    {
            //        pdmNodesDictionary[pdm.Key].EarlyStart = 0;
            //    }
            //    //sets early start for other nodes
            //    else
            //    {
            //        var max = pdmNodesDictionary[pdm.Key].PreviousNode.Max(node => node.EarlyEnd);

            //        if (max == null) throw new ArgumentException("Sum is null");

            //        pdmNodesDictionary[pdm.Key].EarlyStart = max;
            //    }
            //}





            //// Znalezienie i ustawienie EarlyStart, EarlyEnd, LateStart, SlackTime dla każdego węzła
            //SetNodeTiming(rootNode: pdmNodesDictionary[0], currentTime: 0);

            //// Znalezienie ścieżki krytycznej
            //var criticalPathIds = FindCriticalPath(rootNode: pdmNodesDictionary[0]);

            //// Obliczenie CriticalPathVariation
            //double criticalPathVariation = pdmNodesDictionary[criticalPathIds.Last()].LateEnd - pdmNodesDictionary[criticalPathIds.First()].EarlyEnd;
            var test = pdmNodesDictionary[0].ToHtmlString().Trim();

            var viewModel = new TaskPostVM(pdmNodesDictionary);
            viewModel.CSV = GenerateCSVContent(projectTasks);
            return viewModel;
        }

        private string GenerateCSVContent(List<ProjectTask> projectTasks)
        {
            var strBuilder = new StringBuilder();
            strBuilder.AppendLine("Number;Name;Positive finish time;Average finish time;Negative finish time;Depends on");
            for (int i = 0; i < projectTasks.Count; i++)
            {
                strBuilder.AppendLine($"{i};{projectTasks[i].Name};{projectTasks[i].PositiveFinishTime};" +
                    $"{projectTasks[i].AverageFinishTime};{projectTasks[i].NegativeFinishTime};{projectTasks[i].DependOnTasks}");
            }

            return strBuilder.ToString().TrimEnd();
        }

        // Rekurencyjna funkcja do ustawiania czasów dla każdego węzła
        //private void SetNodeTiming(PDMNode rootNode, double currentTime)
        //{
        //    rootNode.EarlyStart = currentTime;

        //    if (rootNode.NextNode != null)
        //    {
        //        SetNodeTiming(rootNode.NextNode, currentTime + rootNode.EstimatedTaskEndTime);
        //    }

        //    rootNode.EarlyEnd = rootNode.EarlyStart + rootNode.EstimatedTaskEndTime;
        //}

        // Funkcja do znalezienia ścieżki krytycznej
        //private List<int> FindCriticalPath(PDMNode rootNode)
        //{
        //    List<int> criticalPathIds = new List<int>();
        //    FindCriticalPathRecursive(rootNode, criticalPathIds);
        //    return criticalPathIds;
        //}

        //// Funkcja pomocnicza dla znalezienia ścieżki krytycznej (rekurencyjna)
        //private void FindCriticalPathRecursive(PDMNode currentNode, List<int> criticalPathIds)
        //{
        //    criticalPathIds.Add(currentNode.Id);

        //    if (currentNode.NextNode != null)
        //    {
        //        FindCriticalPathRecursive(currentNode.NextNode, criticalPathIds);
        //    }
        //}
    }
}
