namespace PertPlan.WebUI.Models
{
    public class ActionPERT
    {
        public int Id { get; set; }
        public List<ActionPERT>? PreviousActions { get; set; }
        public List<ActionPERT>? NextActions { get; set; }
        public string? Name { get; set; }
        public double Negative { get; set; }
        public double Average { get; set; }
        public double Positive { get; set; }
    }
}
