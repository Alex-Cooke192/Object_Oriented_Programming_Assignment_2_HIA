namespace JIDS.Models
{
    public class LightingProperties : ComponentPropertiesBase
    {
        public int BrightnessLevel { get; set; }
        public string ColorTemperature { get; set; }
        public bool Dimmable { get; set; }
    }
}