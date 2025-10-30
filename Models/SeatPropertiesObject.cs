namespace JetInteriorApp.Models
{
    public class SeatProperties
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public bool Recline { get; set; }
        public bool Lighting { get; set; }
        public bool Massage { get; set; }
        public string Accessibility { get; set; } // JSON stored as string
    }
}