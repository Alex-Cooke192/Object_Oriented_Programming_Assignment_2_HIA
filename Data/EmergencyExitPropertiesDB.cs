public class EmergencyExitComponentDB : InteriorComponentDB
{
    public Guid Id { get; set; }
    public Guid ComponentId { get; set; }
    public float ClearanceRadius { get; set; }
    public string? SignageType { get; set; }
    public string? AccessibilityFeatures { get; set; } // JSON

    public InteriorComponentDB? Component { get; set; }
}
