public class ConfigEntry
{
    public Guid ConfigID { get; set; } // Unique ID for this entry (could be same as ConfigID or separate)
    public Guid UserId { get; set; }  // Owner of the config
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? Name { get; set; }
    public object? JetLayout { get; set; } // The actual layout object
}