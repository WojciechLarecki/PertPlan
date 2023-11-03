namespace PertPlan.WebUI.Models
{
    public class ActionPERT
    {
        public List<ActionPERT>? PreviousActions { get; set; }
        public string? Name { get; set; }
        public int Negative { get; set; }
        public int Avrage { get; set; }
        public int Positive { get; set; }
    }
}
