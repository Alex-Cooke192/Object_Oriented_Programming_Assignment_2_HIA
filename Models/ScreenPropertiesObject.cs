namespace JetInteriorApp.Models
{
    public class ScreenProperties : ComponentPropertiesBase
    {
        public Guid Id { get; set; }
        public Guid ComponentId { get; set; }
        public string Resolution { get; set; }
        public string ContentFilters { get; set; } // JSON stored as string
        public bool TouchEnabled { get; set; }
    }

}