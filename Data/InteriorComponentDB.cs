public class InteriorComponentDB
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Position { get; set; } // JSON
    public string Dimensions { get; set; } // JSON
    public string Material { get; set; }
    public float Weight { get; set; }
    public string MountType { get; set; }
    public float Cost { get; set; }
    public string Tier { get; set; }
    public DateTime CreatedAt { get; set; }
}