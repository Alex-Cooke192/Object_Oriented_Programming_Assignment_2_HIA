public class TablePropertiesDB
{
    public int Id { get; set; }
    public int ComponentId { get; set; }
    public bool Foldable { get; set; }
    public string SurfaceMaterial { get; set; }
    public int SeatCount { get; set; }

    public InteriorComponentDB Component { get; set; }
}