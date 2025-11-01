namespace JetInteriorApp.Models
{
    public class ToiletProperties : ComponentPropertiesBase
    {
        public Guid Id { get; set; }
        public Guid ComponentId { get; set; }
        public bool IsAccessible { get; set; }
        public bool HasBabyChanging { get; set; }
        public string VentilationType { get; set; }
    }
}