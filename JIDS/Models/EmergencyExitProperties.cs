namespace JetInteriorApp.Models
{
    public class EmergencyExitProperties : ComponentPropertiesBase
    {
        public float ClearanceRadius { get; set; }
        public string SignageType { get; set; }
        public string AccessibilityFeatures { get; set; } // JSON stored as string
    }
}