namespace JetInteriorApp.Models
{
    public class ScreenProperties : ComponentProperties
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public string Resolution { get; set; }
        public string ContentFilters { get; set; } // JSON stored as string
        public bool TouchEnabled { get; set; }
    }

}