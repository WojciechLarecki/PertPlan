using System.Text;

namespace PertPlan.WebUI.Models.ViewModels
{
    /// <summary>
    /// Widok modelu reprezentujący diagram PERT oraz tabele danych.
    /// </summary>
    public class TaskPostVM
    {
        /// <summary>
        /// Widok modelu tabeli.
        /// </summary>
        public TaskPostTableVM TableVM { get; private set; }

        /// <summary>
        /// Słownik zadań diagramu PERT.
        /// </summary>
        public Dictionary<int, PDMNode> Nodes { get; private set; }

        /// <summary>
        /// Inicjalizuje nową instancję widoku modelu.
        /// </summary>
        /// <param name="actions">Dostępne zdania.</param>
        public TaskPostVM(List<ActionPERT> actions)
        {
            Nodes = new Dictionary<int, PDMNode>();

            // convert actions to pdm nodes
            foreach (var task in actions)
            {
                PDMNode pdmNode = new PDMNode(task);

                Nodes.Add(pdmNode.Id, pdmNode);
            }

            foreach (var action in actions)
            {
                // set previous nodes
                if (action.PreviousActions != null)
                {
                    Nodes[action.Id].PreviousNodes = new List<PDMNode>();

                    foreach (var previousAction in action.PreviousActions)
                    {
                        Nodes[action.Id].PreviousNodes!.Add(Nodes[previousAction.Id]);
                    }
                }

                // set next nodes
                if (action.NextActions != null)
                {
                    Nodes[action.Id].NextNodes = new List<PDMNode>();

                    foreach (var nextAction in action.NextActions)
                    {
                        Nodes[action.Id].NextNodes!.Add(Nodes[nextAction.Id]);
                    }
                }
            }

            // set early start for all nodes
            foreach (var nodeKVP in Nodes)
            {
                var node = Nodes[nodeKVP.Key];
                if (node.PreviousNodes == null) node.EarlyStart = 0;
                else node.EarlyStart = node.PreviousNodes.Max(prevNode => prevNode.EarlyEnd);
            }

            // set late end for last nodes
            var lastNodes = Nodes.Values.Where(n => n.NextNodes == null).ToList();
            var max = lastNodes.Select(n => n.EarlyEnd).Max();
            foreach (var node in lastNodes)
            {
                node.LateEnd = max;
            }

            // set late end for rest of nodes
            for (int i = Nodes.Count - 1; i >= 0; --i)
            {
                var node = Nodes[i];
                if (node.NextNodes == null) continue;
                else node.LateEnd = node.NextNodes.Min(prevNode => prevNode.LateStart);
            }

            CSV = GenerateCSVContent(actions);
            TableVM = new TaskPostTableVM(actions);
        }

        /// <summary>
        /// Zawartość CSV generowana na podstawie dostępnych działań.
        /// </summary>
        public string CSV { get; set; }

        /// <summary>
        /// Długość ścieżki krytycznej projektu.
        /// </summary>
        public double CriticalPathLenght
        {
            get
            {
                double result = 0;

                foreach (var nodeKye in Nodes)
                {
                    if (nodeKye.Value.IsCritical)
                    {
                        result += nodeKye.Value.EstimatedTaskEndTime ?? 0;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Odchylenie standardowe projektu.
        /// </summary>
        public double ProjectStandardDeviation
        {
            get
            {
                double result = 0;
                foreach (var node in Nodes.Values)
                {
                    if (node.IsCritical)
                    {
                        result += node.Variation;
                    }
                }

                return Math.Sqrt(result);
            }
        }

        /// <summary>
        /// Konwertuje obiekt na tekstową reprezentację diagramu w notacji PDM.
        /// </summary>
        /// <returns>Reprezentacja diagramu w postaci tekstu.</returns>
        public string ToPDMGraph()
        {
            var chars = "ABCDEFGHIJKLMNOPERSTUWXYZ";
            var output = "graph TD\n";

            foreach (var node in Nodes)
            {
                if (node.Value.NextNodes == null && node.Value.IsCritical)
                {
                    output += chars[node.Value.Id] + $"[{node.Value.ToPDMNotation()}]:::critical\n";
                    continue;
                }
                else if (node.Value.NextNodes == null)
                {
                    output += chars[node.Value.Id] + $"\n";
                    continue;
                }

                foreach (var nextNode in node.Value.NextNodes)
                {
                    if (node.Value.IsCritical && nextNode.IsCritical)
                    {
                        output += chars[node.Value.Id] + $"[{node.Value.ToPDMNotation()}]:::critical" + " --> " + chars[nextNode.Id] + $"[{nextNode.ToPDMNotation()}]:::critical\n";
                    }
                    else if (node.Value.IsCritical)
                    {
                        output += chars[node.Value.Id] + $"[{node.Value.ToPDMNotation()}]:::critical" + " --> " + chars[nextNode.Id] + $"[{nextNode.ToPDMNotation()}]\n";
                    }
                    else if (nextNode.IsCritical)
                    {
                        output += chars[node.Value.Id] + $"[{node.Value.ToPDMNotation()}]" + " --> " + chars[nextNode.Id] + $"[{nextNode.ToPDMNotation()}]:::critical\n";
                    }
                    else
                    {
                        output += chars[node.Value.Id] + $"[{node.Value.ToPDMNotation()}]" + " --> " + chars[nextNode.Id] + $"[{nextNode.ToPDMNotation()}]\n";
                    }
                }
            }
            output += "classDef critical stroke:#f00,stroke-width: 3px\n";
            output += "classDef critical-disabled stroke:#9370DB\n";
            return output;
        }

        /// <summary>
        /// Konwertuje obiekt na tekstową reprezentację diagramu w wygodnej notacji.
        /// </summary>
        /// <returns>Reprezentacja diagramu w postaci tekstu.</returns>
        public string ToCustomGraph()
        {
            var chars = "ABCDEFGHIJKLMNOPERSTUWXYZ";
            var output = "graph TD\n";

            foreach (var node in Nodes)
            {
                if (node.Value.NextNodes == null && node.Value.IsCritical)
                {
                    output += chars[node.Value.Id] + $"[{node.Value.ToCustomNotation()}]:::critical\n";
                    continue;
                }
                else if (node.Value.NextNodes == null)
                {
                    output += chars[node.Value.Id] + $"[{node.Value.ToCustomNotation()}]\n";
                    continue;
                }

                foreach (var nextNode in node.Value.NextNodes)
                {
                    if (node.Value.IsCritical && nextNode.IsCritical)
                    {
                        output += chars[node.Value.Id] + $"[{node.Value.ToCustomNotation()}]:::critical" + " --> " + chars[nextNode.Id] + $"[{nextNode.ToCustomNotation()}]:::critical\n";
                    }
                    else if (node.Value.IsCritical)
                    {
                        output += chars[node.Value.Id] + $"[{node.Value.ToCustomNotation()}]:::critical" + " --> " + chars[nextNode.Id] + $"[{nextNode.ToCustomNotation()}]\n";
                    }
                    else if (nextNode.IsCritical)
                    {
                        output += chars[node.Value.Id] + $"[{node.Value.ToCustomNotation()}]" + " --> " + chars[nextNode.Id] + $"[{nextNode.ToCustomNotation()}]:::critical\n";
                    }
                    else
                    {
                        output += chars[node.Value.Id] + $"[{node.Value.ToCustomNotation()}]" + " --> " + chars[nextNode.Id] + $"[{nextNode.ToCustomNotation()}]\n";
                    }
                }
            }
            output += "classDef critical stroke:#f00,stroke-width: 3px\n";
            output += "classDef critical-disabled stroke:#9370DB\n";
            return output;
        }

        /// <summary>
        /// Generuje zawartość CSV na podstawie dostępnych zadań projektowych.
        /// </summary>
        /// <param name="projectTasks">Dostępne zadania projektowe.</param>
        /// <returns>Reprezentacja zawartości CSV w postaci tekstu.</returns>
        private string GenerateCSVContent(List<ActionPERT> projectTasks)
        {
            var strBuilder = new StringBuilder();
            strBuilder.AppendLine("Number;Name;Positive finish time;Average finish time;Negative finish time;Depends on");
            for (int i = 0; i < projectTasks.Count; i++)
            {
                strBuilder.Append($"{i};{projectTasks[i].Name};{projectTasks[i].Positive};" +
                    $"{projectTasks[i].Average};{projectTasks[i].Negative};");
                
                if (projectTasks[i].PreviousActions is null)
                {
                    strBuilder.AppendLine("x");
                }
                else
                {
                    var preccesingTasksIds = projectTasks[i].PreviousActions!.Select(x => x.Id).ToList();
                    strBuilder.AppendJoin(", ", preccesingTasksIds);
                    strBuilder.AppendLine();
                }
            }

            return strBuilder.ToString().TrimEnd();
        }

    }
}
