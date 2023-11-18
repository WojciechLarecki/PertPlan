using System.ComponentModel.DataAnnotations;

namespace PertPlan.WebUI.Models.ViewModels
{
    public class ProjectTask
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa jest wymagana")]
        public string Name { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Wartość musi być liczbą nieujemną")]
        public double PositiveFinishTime { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Wartość musi być liczbą nieujemną")]
        public double AverageFinishTime { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Wartość musi być liczbą nieujemną")]
        public double NegativeFinishTime { get; set; }
        public string? DependOnTasks { get; set; }
    }
}
