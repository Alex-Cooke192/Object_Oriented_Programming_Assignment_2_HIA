public class KitchenComponentDB : InteriorComponentDB
{
    public Guid Id { get; set; }
    public Guid ComponentId { get; set; }
    public string ApplianceList { get; set; } // JSON
    public bool Refrigeration { get; set; }
    public bool FireSuppression { get; set; }

    public InteriorComponentDB Component { get; set; }
}