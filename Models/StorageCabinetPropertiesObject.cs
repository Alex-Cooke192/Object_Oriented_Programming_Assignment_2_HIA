namespace JetInteriorApp.Models
{
    public class StorageCabinetProperties : ComponentProperties
    {
        public int Id { get; set; }
        public int ComponentId { get; set; }
        public float CapacityLitres { get; set; }
        public bool Lockable { get; set; }
        public int ShelfCount { get; set; }
    }
}