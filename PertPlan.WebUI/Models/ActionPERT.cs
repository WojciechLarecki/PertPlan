namespace PertPlan.WebUI.Models
{
    /// <summary>
    /// Klasa reprezentująca pojedyncze zadanie PERT.
    /// </summary>
    public class ActionPERT
    {
        /// <summary>
        /// Identyfikator zadania.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Poprzednie zadania.
        /// </summary>
        public List<ActionPERT>? PreviousActions { get; set; }

        /// <summary>
        /// Następne działania.
        /// </summary>
        public List<ActionPERT>? NextActions { get; set; }

        /// <summary>
        /// Nazwa działania.
        /// </summary>
        public string? Name { get; set; }
        
        /// <summary>
        /// Negatywny czas ukończenia zadania.
        /// </summary>
        public double Negative { get; set; }
        
        /// <summary>
        /// Średni czas ukończenia zadania.
        /// </summary>
        public double Average { get; set; }

        /// <summary>
        /// Pozytywny czas ukończenia zadania.
        /// </summary>
        public double Positive { get; set; }

        /// <summary>
        /// Szacowany czas trwania zadania.
        /// </summary>
        public double Estimated { get => Math.Round((Negative + 4 * Average + Positive) / 6, 8); }
    }
}
