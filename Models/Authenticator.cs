public class Authenticator
{
    private readonly List<User> _users;

    public Authenticator(List<User> users)
    {
        _users = users;
    }

    public bool Authenticate(string username, string password)
    {
        var user = _users.FirstOrDefault(u => u.Username == username);
        return user != null && user.ValidatePassword(password);
    }
}