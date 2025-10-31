public class JetConfigDB
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } //Stores foreign key 
    public string Name { get; set; }
    public int Version { get; set; } = 1;
    public bool ValidationStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation 
    public UserDB User { get; set; }
    public ICollection<InteriorComponentDB> InteriorComponents { get; set; }
}