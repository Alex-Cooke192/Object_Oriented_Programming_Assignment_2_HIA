public class StorageCabinetPropertiesDB
{
    public Guid Id { get; set; }
    public Guid ComponentId { get; set; }
    public float CapacityLitres { get; set; }
    public bool Lockable { get; set; }
    public int ShelfCount { get; set; }

    public InteriorComponentDB Component { get; set; }
}