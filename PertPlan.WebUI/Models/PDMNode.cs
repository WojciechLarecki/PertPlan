namespace PertPlan.WebUI.Models
{
    public class PDMNode
    {
        public PDMNode(ActionPERT task)
        {
            _task = task;
            EstimatedTaskEndTime = task.Estimated;
        }

        public int Id => _task.Id;

        public string Name => _task.Name!;

        public double? EarlyEnd { get => _earlyEnd; private set => _earlyEnd = value; }

        public double? SlackTime { get => _slackTime; private set => _slackTime = value; }

        public double StandardDeviation => (_task.Negative + _task.Positive) / 6;

        public double Variation => Math.Pow(StandardDeviation, 2);

        public List<PDMNode>? NextNodes { get; set; }

        public List<PDMNode>? PreviousNodes { get; set; }
        private double? _earlyStart;
        private double? _estimatedTaskEndTime;
        private double? _earlyEnd;
        private double? _lateStart;
        private double? _lateEnd;
        private double? _slackTime;
        private readonly ActionPERT _task;

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

        public string ToHtmlString()
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

        public bool IsCritical { get => SlackTime == 0; }

        const string gridContainerStyle = "display: grid; grid-template-columns: 1fr 1fr 1fr; grid-template-rows: auto; background-color: transparent; gap: 2px";

        const string gridItemStyle = "background-color: white; padding: 10px; text-align: center;";

        const string gridItem3ColumnsStyle = "grid-column: span 3;";
    }
}
