namespace JIDS.Models
{
    public class StorageCabinetProperties : ComponentPropertiesBase
    {
        public float CapacityLitres { get; set; }
        public bool Lockable { get; set; }
        public int ShelfCount { get; set; }
    }
}