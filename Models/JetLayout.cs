namespace JetInteriorApp.Models
{
    public class JetLayout
    {
        public Guid ConfigID { get; set; }
        public List<LayoutCell> Components { get; set; }
    }
}
