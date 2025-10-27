public class JetConfigDB
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ConfigJson { get; set; }
    public string Name { get; set; }
    public int Version { get; set; } = 1;
    public bool ValidationStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public UserDB User { get; set; }
}