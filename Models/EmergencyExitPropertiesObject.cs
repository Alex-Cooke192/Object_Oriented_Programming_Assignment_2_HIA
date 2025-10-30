namespace JetInteriorApp.Models
{
    public class EmergencyExitProperties
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public float ClearanceRadius { get; set; }
        public string SignageType { get; set; }
        public string AccessibilityFeatures { get; set; } // JSON stored as string
    }
}