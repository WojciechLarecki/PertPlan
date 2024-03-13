using Microsoft.AspNetCore.Mvc.Localization;
using System.Text;

namespace PertPlan.WebUI.Models.ViewModels
{
    public class TaskPostTableVM
    {
        private List<byte[]> _combinations;
        private List<ActionPERT> _nodes;

        public List<ActionPERT> Nodes { get => _nodes; }
        public List<CombinationRow> Rows { get; private set; }
        public Dictionary<double, double> RowsForTable2 { get; private set; }
        public TaskPostTableVM(IEnumerable<ActionPERT> actions)
        {
            Rows = new List<CombinationRow>();
            RowsForTable2 = new Dictionary<double, double>();
            _nodes = actions.ToList();
            _combinations = new List<byte[]>();
            GenerateCombinationsHelper(_nodes.Count, new byte[_nodes.Count]);
            var falseBegin = new ActionPERT();
            var falseEnd = new ActionPERT();

            for (int i = 0; i < _combinations.Count; i++)
            {
                byte[] bytes = new byte[_nodes.Count + 2];
                Array.Copy(_combinations[i], 0, bytes, 1, _nodes.Count);

                _combinations[i] = bytes;
                var row = new CombinationRow();
                row.Sequence = bytes;
                Rows.Add(row);
            }

            var startActions = actions.Where(x => x.PreviousActions == null).ToList();
            var endActions = actions.Where(x => x.NextActions == null).ToList();
            falseBegin.NextActions = startActions;
            falseEnd.PreviousActions = endActions;
            startActions.ForEach(x => { x.PreviousActions = new List<ActionPERT> { falseBegin }; });
            endActions.ForEach(x => { x.NextActions = new List<ActionPERT> { falseEnd }; });
            _nodes.Insert(0, falseBegin);
            _nodes.Add(falseEnd);
            for (int i = 0; i < _nodes.Count; i++) 
            {
                _nodes[i].Id = i;
            }

            foreach (var row in Rows)
            {
                var sol = FindLongestPath(falseBegin, falseEnd, row.Sequence!);
                row.Time = sol.Item1;
                row.Probability = sol.Item2;

                if (RowsForTable2.ContainsKey(row.Time))
                {
                    RowsForTable2[row.Time] += row.Probability;
                }
                else
                {
                    RowsForTable2.Add(row.Time, row.Probability);
                }
            }
        }


        public (double, double) FindLongestPath(ActionPERT start, ActionPERT target, byte[] sequence)
        {
            Dictionary<ActionPERT, double> distance = new Dictionary<ActionPERT, double>();
            double chance = GetNodesChance(sequence);

            foreach (var node in _nodes)
            {
                distance[node] = double.MinValue;
            }
            distance[start] = 0;
            
            Queue<ActionPERT> queue = new Queue<ActionPERT>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current == target)
                {
                    return (distance[current], chance);
                }

                var neighbors = current.NextActions ?? Enumerable.Empty<ActionPERT>();
                foreach (var neighbor in neighbors)
                {
                    var newDistance = GetDistance(neighbor, sequence);
                    var newLength = distance[current] + newDistance;
                    if (newLength > distance[neighbor])
                    {
                        distance[neighbor] = newLength;
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return (-1d, -1d); // Jeśli nie istnieje ścieżka od A do B
        }

        private double GetDistance(ActionPERT neighbor, byte[] sequence)
        {
            var number = sequence[neighbor.Id];

            if (neighbor.Id == 0 || neighbor.Id == sequence.Length - 1)
                return 0;

            if (number == 1)
            {
                return neighbor.Positive;
            }
            else if (number == 2)
            {
                return neighbor.Average;
            }
            else
            {
                return neighbor.Negative;
            }
        }

        private double GetNodesChance(byte[] sequence)
        {
            var chance = 1d;

            foreach (var number in sequence)
            {
                if (number == 2)
                {
                    chance *= 4;
                } 
            }

            chance /= Math.Pow(6, _nodes.Count - 2);
            return chance;
        }
        


        /*
            Time complexity: O(3^n)
            Memory complexity: O(3^n)
        */

        private void GenerateCombinationsHelper(int n, byte[] sequence)
        {
            if (n == 0)
            {
                byte[] copy = new byte[sequence.Length];
                Array.Copy(sequence, copy, sequence.Length);
                _combinations.Add(copy);
            }
            else
            {
                for (byte i = 1; i <= 3; i++)
                {
                    sequence[sequence.Length - n] = i;
                    GenerateCombinationsHelper(n - 1, sequence);
                }
            }
        }

        public string ToBarChart(string chartTitle)
        {
            double maxValue = RowsForTable2.Values.Max();
            double roundedValue = Math.Ceiling(maxValue * 10) / 10;
            StringBuilder strBuilder = new StringBuilder();

            List<string> keysToDisaply = RowsForTable2.Keys.Select(k => k.ToString(System.Globalization.CultureInfo.InvariantCulture)).ToList();
            List<string> valuesToDisaply = RowsForTable2.Values.Select(k => k.ToString(System.Globalization.CultureInfo.InvariantCulture)).ToList();

            strBuilder.AppendLine("xychart-beta");
            strBuilder.AppendLine($"title \"{chartTitle}\"");
            strBuilder.Append("x-axis [");
            strBuilder.AppendJoin(", ", keysToDisaply);
            strBuilder.AppendLine("]");
            strBuilder.AppendLine($"y-axis 0 --> {roundedValue}");
            strBuilder.Append("bar [");
            strBuilder.AppendJoin(", ", valuesToDisaply);
            strBuilder.AppendLine("]");
            strBuilder.Append("line [");
            strBuilder.AppendJoin(", ", valuesToDisaply);
            strBuilder.AppendLine("]");

            return strBuilder.ToString();
        }
    }

    public class CombinationRow
    {
        public byte[]? Sequence { get; set; }
        public double Time { get; set; }
        public double Probability { get; set; }
    }
}
