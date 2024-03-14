namespace PertPlan.WebUI.Models.ViewModels
{
    /// <summary>
    /// Klasa reprezentująca dane podane przez użykownika przy definiowaniu zadań.
    /// </summary>
    public class ProjectTask
    {
        /// <summary>
        /// Identyfikator zadania.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nazwa zadania.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Czas pozytywny zakończenia zadania.
        /// </summary>
        public double PositiveFinishTime { get; set; }

        /// <summary>
        /// Średni czas zakończenia zadania.
        /// </summary>
        public double AverageFinishTime { get; set; }

        /// <summary>
        /// Czas negatywny zakończenia zadania.
        /// </summary>
        public double NegativeFinishTime { get; set; }

        /// <summary>
        /// Łańcuch zawierający identyfikatory zadań, od których to zadanie zależy.
        /// </summary>
        public string? DependOnTasks { get; set; }
    }
}
