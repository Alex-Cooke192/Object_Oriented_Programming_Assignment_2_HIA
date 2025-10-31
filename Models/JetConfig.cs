public class ConfigEntry
{
    public Guid ConfigID { get; set; } // Unique ID for this entry (could be same as ConfigID or separate)
    public Guid UserId { get; set; }  // Owner of the config
    public string Name { get; set; }
    public string Version { get; set; }
    public string configJson { get; set; }
    public bool ValidationStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}