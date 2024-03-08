namespace PertPlan.WebUI.Models.ViewModels
{
    public class TaskPostTableVM
    {
        private List<byte[]> _combinations;
        private List<ActionPERT> nodes;

        public List<ActionPERT> Nodes { get => nodes; }
        public List<Table1VM> Rows { get; private set; }
        public Dictionary<double, double> RowsForTable2 { get; private set; }
        public TaskPostTableVM(IEnumerable<ActionPERT> actions)
        {
            Rows = new List<Table1VM>();
            RowsForTable2 = new Dictionary<double, double>();

            int n = actions.Count();
            _combinations = new List<byte[]>();
            GenerateCombinationsHelper(n, new byte[n]);
            nodes = actions.ToList();
            var falseBegin = new ActionPERT();
            var falseEnd = new ActionPERT();

            for (int i = 0; i < _combinations.Count; i++)
            {
                byte[] bytes = new byte[n + 2];
                Array.Copy(_combinations[i], 0, bytes, 1, n);

                _combinations[i] = bytes;
                var row = new Table1VM();
                row.Sequence = bytes;
                Rows.Add(row);
            }

            var startActions = actions.Where(x => x.PreviousActions == null).ToList();
            var endActions = actions.Where(x => x.NextActions == null).ToList();
            falseBegin.NextActions = startActions;
            falseEnd.PreviousActions = endActions;
            startActions.ForEach(x => { x.PreviousActions = new List<ActionPERT> { falseBegin }; });
            endActions.ForEach(x => { x.NextActions = new List<ActionPERT> { falseEnd }; });
            nodes.Insert(0, falseBegin);
            nodes.Add(falseEnd);
            for (int i = 0; i < nodes.Count; i++) 
            {
                nodes[i].Id = i;
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

            foreach (var node in nodes)
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

            chance /= Math.Pow(6, nodes.Count - 2);
            return chance;
        }
        


        /*
            Złożoność czasowa: O(3^n)
            Złożoność pamięciowa: O(3^n)
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

        public string ToBarChart()
        {
            double maxValue = RowsForTable2.Values.Max();
            double roundedValue = Math.Ceiling(maxValue * 10) / 10;

            var str = "xychart-beta\n" +
                        "title \"Rozkład prawdopodobieństwa\"\n" +
                        $"x-axis [{string.Join(", ", RowsForTable2.Keys)}]\n" +
                        $"y-axis 0 --> {roundedValue}\n" +
                        $"bar [{string.Join(", ", RowsForTable2.Values)}]\n" +
                        $"line [{string.Join(", ", RowsForTable2.Values)}]\n";
            return str;
        }
    }

    public class Table1VM
    {
        public byte[]? Sequence { get; set; }
        public double Time { get; set; }
        public double Probability { get; set; }
    }
}
