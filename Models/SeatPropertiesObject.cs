namespace JetInteriorApp.Models
{
    public class SeatProperties : ComponentPropertiesBase
    {
        public Guid Id { get; set; }
        public Guid ComponentId { get; set; }
        public bool Recline { get; set; }
        public bool Lighting { get; set; }
        public bool Massage { get; set; }
        public string Accessibility { get; set; } // JSON stored as string
    }
}