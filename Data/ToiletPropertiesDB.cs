public class ToiletPropertiesDB
{
    public int Id { get; set; }
    public int ComponentId { get; set; }
    public bool IsAccessible { get; set; }
    public bool HasBabyChanging { get; set; }
    public string VentilationType { get; set; }

    public InteriorComponentDB Component { get; set; }
}