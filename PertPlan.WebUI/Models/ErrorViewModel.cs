namespace PertPlan.WebUI.Models
{
    /// <summary>
    /// Model b��du.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Identyfikator ��dania.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Okre�la, czy identyfikator ��dania powinien by� wy�wietlany.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}