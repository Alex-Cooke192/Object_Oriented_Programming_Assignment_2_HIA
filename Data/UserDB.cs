public class UserDB
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public ICollection<JetConfigDB> JetConfigs { get; set; } = new List<JetConfigDB>();
}
