namespace PertPlan.WebUI.Models
{
    /// <summary>
    /// Klasa reprezentująca węzeł diagramu sieciowego PERT.
    /// </summary>
    public class PDMNode
    {
        /// <summary>
        /// Inicjalizuje nową instancję węzła diagramu sieciowego PERT.
        /// </summary>
        /// <param name="task">Działanie PERT przypisane do węzła.</param>
        public PDMNode(ActionPERT task)
        {
            _task = task;
            EstimatedTaskEndTime = task.Estimated;
        }

        /// <summary>
        /// Identyfikator węzła.
        /// </summary>
        public int Id => _task.Id;

        /// <summary>
        /// Nazwa węzła.
        /// </summary>
        public string Name => _task.Name!;

        /// <summary>
        /// Najwcześniejszy moment, kiedy zadanie powinno się zakończyć.
        /// </summary>
        public double? EarlyEnd { get => _earlyEnd; private set => _earlyEnd = value; }

        /// <summary>
        /// Czas w którym zadanie może nie zostać zaczęte.
        /// </summary>
        public double? SlackTime { get => _slackTime; private set => _slackTime = value; }

        /// <summary>
        /// Odchylenie standardowe.
        /// </summary>
        public double StandardDeviation => (_task.Negative + _task.Positive) / 6;

        /// <summary>
        /// Wariacja.
        /// </summary>
        public double Variation => Math.Pow(StandardDeviation, 2);

        /// <summary>
        /// Wcześniejsze węzły.
        /// </summary>
        public List<PDMNode>? NextNodes { get; set; }

        /// <summary>
        /// Następne węzły.
        /// </summary>
        public List<PDMNode>? PreviousNodes { get; set; }

        private double? _earlyStart;
        private double? _estimatedTaskEndTime;
        private double? _earlyEnd;
        private double? _lateStart;
        private double? _lateEnd;
        private double? _slackTime;
        private readonly ActionPERT _task;

        /// <summary>
        /// Najwcześniejszy moment, kiedy zadanie powinno się rozpocząć.
        /// </summary>
        public double? EarlyStart
        {
            get => _earlyStart;
            set
            {
                _earlyStart = value;

                if (EstimatedTaskEndTime != null)
                    EarlyEnd = _earlyStart + EstimatedTaskEndTime;

                if (LateStart != null && LateEnd != null)
                    SlackTime = Math.Round(LateStart.Value - LateEnd.Value, 8);

            }
        }


        /// <summary>
        /// Szacowany czas zakończenia zadania.
        /// </summary>
        public double? EstimatedTaskEndTime
        {
            get => _estimatedTaskEndTime;
            private set
            {
                _estimatedTaskEndTime = value;

                if (EarlyStart != null)
                    EarlyEnd = _earlyStart + _estimatedTaskEndTime;

                if (LateEnd != null)
                    LateStart = LateEnd - _estimatedTaskEndTime;
            }
        }

        /// <summary>
        /// Najpóźniejszy moment, kiedy zadanie powinno się zakończyć.
        /// </summary>
        public double? LateEnd
        {
            get => _lateEnd;
            set
            {
                _lateEnd = value;

                if (EstimatedTaskEndTime != null)
                    LateStart = _lateEnd - EstimatedTaskEndTime;
            }
        }

        /// <summary>
        /// Najpóźniejszy moment, kiedy zadanie powinno się rozpocząć.
        /// </summary>
        public double? LateStart
        {
            get => _lateStart;
            private set
            {
                _lateStart = value;

                if (EarlyStart != null && LateStart != null)
                    SlackTime = Math.Round(LateStart.Value - EarlyStart.Value, 8);
            }
        }

        /// <summary>
        /// Tworzy reprezentację węzła w formie kodu HTML w notacji PDM.
        /// </summary>
        /// <returns>Reprezentacja węzła w postaci kodu HTML.</returns>
        public string ToPDMNotation()
        {
            return $@"<div class=""grid-container"" style=""{gridContainerStyle}"">" +
                    $@"<div class=""grid-item"" style=""{gridItemStyle}"">{EarlyStart?.ToString("F2")}</div>" +
                    $@"<div class=""grid-item"" style=""{gridItemStyle}"">{EstimatedTaskEndTime?.ToString("F2")}</div>" +
                    $@"<div class=""grid-item"" style=""{gridItemStyle}"">{EarlyEnd?.ToString("F2")}</div>" +
                    $@"<div class=""grid-item span-three-columns"" style=""{gridItemStyle} {gridItem3ColumnsStyle}"">{Name}</div>" +
                    $@"<div class=""grid-item"" style=""{gridItemStyle}"">{LateStart?.ToString("F2")}</div>" +
                    $@"<div class=""grid-item"" style=""{gridItemStyle}"">{SlackTime?.ToString("F2")}</div>" +
                    $@"<div class=""grid-item"" style=""{gridItemStyle}"">{LateEnd?.ToString("F2")}</div>" +
                "</div>";
        }

        /// <summary>
        /// Tworzy reprezentację węzła w formie kodu HTML w notacji PDM.
        /// </summary>
        /// <returns>Reprezentacja węzła w postaci kodu HTML.</returns>
        public string ToCustomNotation()
        {
            return $@"<div class=""grid-container"" style=""{gridContainerStyle}"">" +
                    $@"<div class=""grid-item"" style=""{gridItemStyle}"">{_task.Positive.ToString("F2")}</div>" +
                    $@"<div class=""grid-item"" style=""{gridItemStyle}"">{_task.Average.ToString("F2")}</div>" +
                    $@"<div class=""grid-item"" style=""{gridItemStyle}"">{_task.Negative.ToString("F2")}</div>" +
                    $@"<div class=""grid-item span-three-columns"" style=""{gridItemStyle} {gridItem3ColumnsStyle}"">{Name}</div>" +
                    $@"<div class=""grid-item span-three-columns"" style=""{gridItemStyle} {gridItem3ColumnsStyle}"">{_task.Estimated.ToString("F2")}</div>" +
                "</div>";
        }

        /// <summary>
        /// Określa, czy węzeł należy do ścieżki krytycznej.
        /// </summary>
        public bool IsCritical { get => SlackTime == 0; }

        const string gridContainerStyle = "display: grid; grid-template-columns: 1fr 1fr 1fr; grid-template-rows: auto; background-color: transparent; gap: 2px";

        const string gridItemStyle = "background-color: white; padding: 10px; text-align: center;";

        const string gridItem3ColumnsStyle = "grid-column: span 3;";
    }
}
