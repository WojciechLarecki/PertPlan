namespace PertPlan.WebUI.Models
{
    /// <summary>
    /// Model błędu.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Identyfikator zadania.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Określa, czy identyfikator zadania powinien być wyświetlany.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}