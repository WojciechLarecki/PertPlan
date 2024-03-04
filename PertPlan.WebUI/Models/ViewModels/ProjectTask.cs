namespace PertPlan.WebUI.Models.ViewModels
{
    public class ProjectTask
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double PositiveFinishTime { get; set; }

        public double AverageFinishTime { get; set; }

        public double NegativeFinishTime { get; set; }

        public string? DependOnTasks { get; set; }

        public ProjectTask Copy()
        {
            return new ProjectTask 
            { 
                Id = Id, 
                Name = Name,
                PositiveFinishTime = PositiveFinishTime,
                AverageFinishTime = AverageFinishTime,
                NegativeFinishTime = NegativeFinishTime,
                DependOnTasks = DependOnTasks
            };   
        }
    }
}
