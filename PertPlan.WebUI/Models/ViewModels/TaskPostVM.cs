using System.Reflection;
using System.Xml.Linq;

namespace PertPlan.WebUI.Models.ViewModels
{
    public class TaskPostVM
    {
        public Dictionary<int, PDMNode> Nodes { get; private set; }

        public TaskPostVM(Dictionary<int, PDMNode> nodes)
        {
            Nodes = nodes;
        }

        public List<int>? CriticalPathIds { get; set; }
        public double CriticalPathVariation { get; set; }
        public string CSV { get; set; }

        public override string ToString()
        {
            var chars = "ABCDEFGHIJKLMNOPERSTUWXYZ";
            var output = "graph TD\n";

            foreach (var node in Nodes)
            {
                if (node.Value.NextNode == null) continue;

                foreach (var nextNode in node.Value.NextNode)
                {
                    output += chars[node.Value.Id] + $"[{node.Value.ToHtmlString()}]" + " --> " + chars[nextNode.Id] + $"[{nextNode.ToHtmlString()}]\n";
                }
            }

            return output;
        }
    }
}
