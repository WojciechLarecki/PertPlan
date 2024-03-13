using System.Runtime.Serialization;

namespace PertPlan.WebUI.Models.ViewModels
{
    public class ProjectTask : ISerializable
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            info.AddValue(nameof(Id), Id);
            info.AddValue(nameof(Name), Name);
            info.AddValue(nameof(PositiveFinishTime), PositiveFinishTime);
            info.AddValue(nameof(AverageFinishTime), AverageFinishTime);
            info.AddValue(nameof(NegativeFinishTime), NegativeFinishTime);
            info.AddValue(nameof(DependOnTasks), DependOnTasks);
        }
    }
}
