using System.Threading.Tasks;

public class AuthService
{
    public async Task<bool> LoginAsync(string username, string password)
    {
        // Simulate API call delay
        await Task.Delay(1000);

        // Simple check (in real life, call an API here)
        return username == "admin" && password == "password";
    }
}