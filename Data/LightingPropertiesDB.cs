public class LightingPropertiesDB
{
    public Guid Id { get; set; }
    public Guid ComponentId { get; set; }
    public int BrightnessLevel { get; set; }
    public string ColorTemperature { get; set; }
    public bool Dimmable { get; set; }

    public InteriorComponentDB Component { get; set; }
}