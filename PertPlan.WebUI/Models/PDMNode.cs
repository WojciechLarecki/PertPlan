namespace PertPlan.WebUI.Models
{
    public class PDMNode
    {
        public PDMNode(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }
        public string? Name { get; set; }
        public double? EarlyEnd { get => _earlyEnd; private set => _earlyEnd = value; }

        public double? SlackTime { get => _slackTime; private set => _slackTime = value; }

        public List<PDMNode>? NextNode { get; set; }
        public List<PDMNode>? PreviousNode { get; set; }
        private double? _earlyStart;
        private double? _estimatedTaskEndTime;
        private double? _earlyEnd;
        private double? _lateStart;
        private double? _lateEnd;
        private double? _slackTime;

        public double? EarlyStart
        {
            get => _earlyStart;
            set
            {
                _earlyStart = value;

                if (EstimatedTaskEndTime != null)
                    EarlyEnd = _earlyStart + EstimatedTaskEndTime;

                if (LateStart != null)
                    SlackTime = LateStart - LateEnd;

            }
        }

        public double? EstimatedTaskEndTime 
        { 
            get => _estimatedTaskEndTime;
            set 
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

                if (EarlyStart != null)
                    SlackTime = _lateStart - EarlyStart;
            } 
        }

        public string ToHtmlString()
        {
            return $@"<div class=""grid-container"">" +
                    $@"<div class=""grid-item"">{EarlyStart?.ToString("F2")}</div>" +
                    $@"<div class=""grid-item"">{EstimatedTaskEndTime?.ToString("F2")}</div>" +
                    $@"<div class=""grid-item"">{EarlyEnd?.ToString("F2")}</div>" +
                    $@"<div class=""grid-item span-three-columns"">{Name}</div>" +
                    $@"<div class=""grid-item"">{LateStart?.ToString("F2")}</div>" +
                    $@"<div class=""grid-item"">{SlackTime?.ToString("F2")}</div>" +
                    $@"<div class=""grid-item"">{LateEnd?.ToString("F2")}</div>" +
                "</div>";
        }
    }
}
