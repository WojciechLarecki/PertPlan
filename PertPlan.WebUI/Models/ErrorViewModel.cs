namespace PertPlan.WebUI.Models
{
    /// <summary>
    /// Model b³êdu.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Identyfikator ¿¹dania.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Okreœla, czy identyfikator ¿¹dania powinien byæ wyœwietlany.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}