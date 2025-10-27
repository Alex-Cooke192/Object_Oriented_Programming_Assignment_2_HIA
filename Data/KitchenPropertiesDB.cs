public class KitchenPropertiesDB
{
    public int Id { get; set; }
    public int ComponentId { get; set; }
    public string ApplianceList { get; set; } // JSON
    public bool Refrigeration { get; set; }
    public bool FireSuppression { get; set; }

    public InteriorComponentDB Component { get; set; }
}