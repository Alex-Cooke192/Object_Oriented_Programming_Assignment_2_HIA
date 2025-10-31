public class InteriorComponentDB
{
    public Guid ComponentId { get; set; }
    public Guid ConfigID { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Position { get; set; } // JSON
    public string Dimensions { get; set; } // JSON
    public string Material { get; set; }
    public float Weight { get; set; }
    public string Color { get; set; }
    public string Tier { get; set; }
    public DateTime CreatedAt { get; set; }

    //Navigation
    public JetConfigDB Config { get; set; }
    public object ComponentSettings { get; set; }
}