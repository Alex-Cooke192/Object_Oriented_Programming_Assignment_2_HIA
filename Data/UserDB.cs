public class UserDB
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<JetConfig> JetConfigs { get; set; }
}