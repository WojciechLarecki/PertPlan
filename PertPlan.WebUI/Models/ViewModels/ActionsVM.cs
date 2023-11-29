namespace PertPlan.WebUI.Models.ViewModels
{
    public class ActionsVM
    {
        public List<TaskPertVM>? RootActions { get; set; }
        public List<int>? CriticalPathIds { get; set; }
        public double CriticalPathVariation { get; set; }
    }

    public class TaskPertVM
    {
        public TaskPertVM(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public double EarlyStart { get; set; }
        public double EstimatedEndTime { get; set; }
        public double EarlyEnd { get; set; }

        public double LateStart { get; set; }
        public double SlackTime { get; set; }
        public double LateEnd { get; set; }
    }
}
