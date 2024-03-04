using PertPlan.WebUI.Models;
using PertPlan.WebUI.Models.Helpers;
using PertPlan.WebUI.Models.ViewModels;
using System.Text;

namespace PertPlan.WebUI.Logics
{
    public class HomeLogic
    {
        public TaskPostVM GetTaskPostVM(List<ProjectTask> projectTasks)
        {            
            List<ActionPERT> actions = Mapper.MapToActionsPERT(projectTasks);

            // Tworzenie słownika, gdzie kluczem jest ID zadania, a wartością jest odpowiadający mu obiekt PDMNode
            Dictionary<int, PDMNode> pdmNodesDictionary = new Dictionary<int, PDMNode>();
            
            // Tworzenie obiektów PDMNode na podstawie ProjectTask
            foreach (var task in actions)
            {
                PDMNode pdmNode = new PDMNode(task);

                pdmNodesDictionary.Add(pdmNode.Id, pdmNode);
            }

            // set next and previous nodes
            foreach (var action in actions) 
            {
                if (action.PreviousActions != null) 
                {
                    pdmNodesDictionary[action.Id].PreviousNodes = new List<PDMNode>();

                    foreach (var previousAction in action.PreviousActions)
                    {
                        pdmNodesDictionary[action.Id].PreviousNodes.Add(pdmNodesDictionary[previousAction.Id]);
                    }
                }

                if (action.NextActions != null)
                {
                    pdmNodesDictionary[action.Id].NextNodes = new List<PDMNode>();

                    foreach (var nextAction in action.NextActions)
                    {
                        pdmNodesDictionary[action.Id].NextNodes.Add(pdmNodesDictionary[nextAction.Id]);
                    }
                }
            }

            foreach(var nodeKVP in pdmNodesDictionary)
            {
                var node = pdmNodesDictionary[nodeKVP.Key];
                if (node.PreviousNodes == null) node.EarlyStart = 0;
                else node.EarlyStart = node.PreviousNodes.Max(prevNode => prevNode.EarlyEnd);
            }

            for (int i = pdmNodesDictionary.Count - 1; i >= 0; --i)
            {
                var node = pdmNodesDictionary[i];
                if (node.NextNodes == null) node.LateEnd = node.EarlyEnd;
                else node.LateEnd = node.NextNodes.Min(prevNode => prevNode.LateStart);
            }

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
    }
}
