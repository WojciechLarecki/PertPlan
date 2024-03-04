using System.Reflection;
using System.Xml.Linq;

namespace PertPlan.WebUI.Models.ViewModels
{
    public class TaskPostVM
    {
        public TaskPostTableVM TableVM { get; set; }
        public Dictionary<int, PDMNode> Nodes { get; private set; }

        public TaskPostVM(Dictionary<int, PDMNode> nodes)
        {
            Nodes = nodes;
        }

        public string CSV { get; set; }

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

        public override string ToString()
        {
            var chars = "ABCDEFGHIJKLMNOPERSTUWXYZ";
            var output = "graph TD\n";

            foreach (var node in Nodes)
            {
                if (node.Value.NextNodes == null && node.Value.IsCritical)
                {
                    output += chars[node.Value.Id] + $"[{node.Value.ToHtmlString()}]:::critical\n";
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
                        output += chars[node.Value.Id] + $"[{node.Value.ToHtmlString()}]:::critical" + " --> " + chars[nextNode.Id] + $"[{nextNode.ToHtmlString()}]:::critical\n";
                    }
                    else if (node.Value.IsCritical)
                    {
                        output += chars[node.Value.Id] + $"[{node.Value.ToHtmlString()}]:::critical" + " --> " + chars[nextNode.Id] + $"[{nextNode.ToHtmlString()}]\n";
                    }
                    else if (nextNode.IsCritical)
                    {
                        output += chars[node.Value.Id] + $"[{node.Value.ToHtmlString()}]" + " --> " + chars[nextNode.Id] + $"[{nextNode.ToHtmlString()}]:::critical\n";
                    }
                    else
                    {
                        output += chars[node.Value.Id] + $"[{node.Value.ToHtmlString()}]" + " --> " + chars[nextNode.Id] + $"[{nextNode.ToHtmlString()}]\n";
                    }
                    //output += chars[node.Value.Id] + " --> " + chars[nextNode.Id] + "\n";
                }
            }
            output += "classDef critical stroke:#f00,stroke-width: 3px\n";
            output += "classDef critical-disabled stroke:#9370DB\n";
            return output;
        }
    }
}
