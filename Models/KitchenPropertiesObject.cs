namespace JetInteriorObject.Models
{
    public class KitchenProperties
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public string ApplianceList { get; set; } // JSON stored as string
        public bool Refrigeration { get; set; }
        public bool FireSuppression { get; set; }
    }
}