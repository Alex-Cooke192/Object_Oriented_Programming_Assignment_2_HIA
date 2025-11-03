namespace JetInteriorApp.Models
{
    public class SeatProperties : ComponentPropertiesBase
    {
        public bool Recline { get; set; }
        public bool Lighting { get; set; }
        public bool Massage { get; set; }
        public string Accessibility { get; set; }
    }
}