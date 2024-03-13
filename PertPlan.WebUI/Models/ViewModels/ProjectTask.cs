using System.Runtime.Serialization;

namespace PertPlan.WebUI.Models.ViewModels
{
    public class ProjectTask
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public double PositiveFinishTime { get; set; }

        public double AverageFinishTime { get; set; }

        public double NegativeFinishTime { get; set; }

        public string? DependOnTasks { get; set; }
    }
}
