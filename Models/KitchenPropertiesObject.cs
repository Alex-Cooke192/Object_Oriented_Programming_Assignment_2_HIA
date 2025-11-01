namespace JetInteriorApp.Models
{
    public class KitchenProperties : ComponentPropertiesBase
    {
        public Guid Id { get; set; }
        public Guid ComponentId { get; set; }
        public string ApplianceList { get; set; } // JSON stored as string
        public bool Refrigeration { get; set; }
        public bool FireSuppression { get; set; }
    }
}