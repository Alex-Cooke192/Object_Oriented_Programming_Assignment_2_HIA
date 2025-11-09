namespace JIDS.Models
{
    public class KitchenProperties : ComponentPropertiesBase
    {
        public string ApplianceList { get; set; } // JSON stored as string
        public bool Refrigeration { get; set; }
        public bool FireSuppression { get; set; }
    }
}