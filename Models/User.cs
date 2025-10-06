public class User
{
    public string UserID { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public List<JetConfiguration> JetConfigurations { get; set; }

    public bool Authenticate(string username, string password)
    {
        // Simple example, usually you'd hash & check passwords securely
        return this.Username == username && this.Password == password;
    }

    public bool Authorise()
    {
        // Placeholder for authorization logic (e.g., check roles/permissions)
        return true;
    }
}