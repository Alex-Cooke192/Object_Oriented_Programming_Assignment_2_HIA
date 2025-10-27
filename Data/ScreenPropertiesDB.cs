public class ScreenPropertiesDB
{
    public int Id { get; set; }
    public int ComponentId { get; set; }
    public string Resolution { get; set; }
    public string ContentFilters { get; set; } // JSON
    public bool TouchEnabled { get; set; }

    public InteriorComponentDB Component { get; set; }
}