public class ScreenComponentDB : InteriorComponentDB
{
    public Guid Id { get; set; }
    public Guid ComponentId { get; set; }
    public string Resolution { get; set; }
    public string ContentFilters { get; set; } // JSON
    public bool TouchEnabled { get; set; }

    public InteriorComponentDB Component { get; set; }
}