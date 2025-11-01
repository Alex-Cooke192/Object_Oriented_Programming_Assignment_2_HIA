namespace JetInteriorApp.Models
{
    public class JetLayout
    {
        public Guid ConfigID { get; set; }
        public string Name { get; set; } = string.Empty; 
        public List<LayoutCell> Components { get; set; }
    }
}
