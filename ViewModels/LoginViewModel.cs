using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

public class LoginViewModel : INotifyPropertyChanged
{
    private string _username;
    private string _password;
    private string _statusMessage;
    private readonly Authenticator _authenticator;

    public LoginViewModel()
    {
        var users = new List<User>
        {
            new User { UserID = Guid.NewGuid(), Username = "alex", Password = "1234" },
            new User { UserID = Guid.NewGuid(), Username = "isobel", Password = "5678" },
            new User { UserID = Guid.NewGuid(), Username = "hannah", Password = "1357" },
        };
        _authenticator = new Authenticator(users);
        LoginCommand = new RelayCommand(Login);
    }

    public string Username
    {
        get => _username;
        set { _username = value; OnPropertyChanged(); }
    }

    public string Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged(); }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set { _statusMessage = value; OnPropertyChanged(); }
    }

    public ICommand LoginCommand { get; }

    private void Login()
    {
        bool success = _authenticator.Authenticate(Username, Password);
        if (Username == null)
        {
            StatusMessage = user?.IsLockedOut == true
           ? "Too many failed attempts. Access locked."
           : $"Login failed. Attempt {user?.FailedAttempts}/3.";
            return;
        }
        else
        {
            StatusMessage = "Login successful.";
        }
        StatusMessage = success ? "Login successful." : "Invalid credentials.";
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}