namespace JetInteriorApp.Models 
{
    public class TableProperties
    {
        public Guid Id { get; set; }
        public Guid ComponentId { get; set; }
        public bool Foldable { get; set; }
        public string SurfaceMaterial { get; set; }
        public int SeatCount { get; set; }
    }
}
