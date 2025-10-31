namespace JetInteriorApp.Models
{
    public class LightingProperties : ComponentProperties
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public int BrightnessLevel { get; set; }
        public string ColorTemperature { get; set; }
        public bool Dimmable { get; set; }
    }
}
