public class InteriorComponent
{
    public Guid Id { get; set; }
    public Guid ConfigId { get; set; }
    public string Name { get; set; }
    public string Type { get; set; } // e.g. "seat", "screen"
    public string position { get; set; }
    public string Material { get; set; }
    public float Weight { get; set; }
    public string Color { get; set; }
    public string Tier { get; set; }
    public string Accessibility { get; set; }

    public object Properties { get; set; } // Holds subtype-specific data
}