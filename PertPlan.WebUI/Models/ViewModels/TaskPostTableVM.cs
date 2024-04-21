using System.Text;

namespace PertPlan.WebUI.Models.ViewModels
{
    /// <summary>
    /// Widok modelu reprezentujący tabele kombinacji i prawdopodobieństw.
    /// </summary>
    public class TaskPostTableVM
    {

        private List<byte[]> _combinations;
        private List<ActionPERT> _nodes;
        
        /// <summary>
        /// Lista zadań PERT.
        /// </summary>
        public List<ActionPERT> Nodes { get => _nodes; }
        
        /// <summary>
        /// Lista wierszy tabeli kombinacji.
        /// </summary>
        public List<CombinationRow> Rows { get; private set; }

        /// <summary>
        /// Słownik wierszy tabeli prawdopodobieństw.
        /// </summary>
        public Dictionary<double, double> SummaryTableRows { get; private set; }

        /// <summary>
        /// Inicjalizuje nową instancję obiektu tabeli zadań PERT.
        /// </summary>
        /// <param name="actions">Dostępne zadania.</param>
        public TaskPostTableVM(IEnumerable<ActionPERT> actions)
        {
            Rows = new List<CombinationRow>();
            SummaryTableRows = new Dictionary<double, double>();
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

                if (SummaryTableRows.ContainsKey(row.Time))
                {
                    SummaryTableRows[row.Time] += row.Probability;
                }
                else
                {
                    SummaryTableRows.Add(row.Time, row.Probability);
                }
            }
        }

        /// <summary>
        /// Wyszukuje najdłuższą ścieżkę między dwoma zadaniami PERT.
        /// </summary>
        /// <param name="start">Zadanie startowe.</param>
        /// <param name="target">Zadanie docelowe.</param>
        /// <param name="sequence">Sekwencja określająca czasy poszczególnych zadań.</param>
        /// <returns>Krotka zawierająca czas trwania ścieżki i prawdopodobieństwo.</returns>
        public Tuple<double, double> FindLongestPath(ActionPERT start, ActionPERT target, byte[] sequence)
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
                    return new Tuple<double, double>(distance[current], chance);
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

            return new Tuple<double,double>(-1d, -1d); // If the path does not exist
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
        /// <summary>
        /// Generuje kombinacje sekwencji zadań z określonymi czasami PERT.
        /// </summary>
        /// <param name="n">Liczba zadań PERT.</param>
        /// <param name="sequence">Sekwencja rozpoczynająca generowanie, powinna zawierać same zera.</param>
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
        
        /// <summary>
        /// Generuje wykres słupkowy na podstawie danych z tabeli prawdopodobieństw.
        /// </summary>
        /// <param name="chartTitle">Tytuł wykresu.</param>
        /// <returns>Tekst reprezentujący wykres słupkowy.</returns>
        public string ToBarChart(string chartTitle)
        {
            double maxValue = SummaryTableRows.Values.Max();
            double roundedValue = Math.Ceiling(maxValue * 10) / 10;
            StringBuilder strBuilder = new StringBuilder();

            List<string> keysToDisaply = SummaryTableRows.Keys.Select(k => k.ToString(System.Globalization.CultureInfo.InvariantCulture)).ToList();
            List<string> valuesToDisaply = SummaryTableRows.Values.Select(k => k.ToString(System.Globalization.CultureInfo.InvariantCulture)).ToList();

            strBuilder.AppendLine("xychart-beta");
            strBuilder.AppendLine($"title \"{chartTitle}\"");
            strBuilder.Append("x-axis [");
            strBuilder.AppendJoin(", ", keysToDisaply);
            strBuilder.AppendLine("]");
            strBuilder.AppendLine($"y-axis 0 --> {roundedValue.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            strBuilder.Append("bar [");
            strBuilder.AppendJoin(", ", valuesToDisaply);
            strBuilder.AppendLine("]");
            string str = strBuilder.ToString();
            return str;
        }
    }
    
    /// <summary>
    /// Klasa reprezentująca wiersz tabeli kombinacji.
    /// </summary>
    public class CombinationRow
    {
        /// <summary>
        /// Sekwencja określająca kombinację działań.
        /// </summary>
        public byte[]? Sequence { get; set; }
        
        /// <summary>
        /// Czas trwania kombinacji działań.
        /// </summary>
        public double Time { get; set; }

        /// <summary>
        /// Prawdopodobieństwo kombinacji działań.
        /// </summary>
        public double Probability { get; set; }
    }
}
