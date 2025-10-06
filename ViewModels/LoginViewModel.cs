using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Threading.Tasks;

public class LoginViewModel : INotifyPropertyChanged
{
    private string _username;
    private string _password;
    private string _statusMessage;
    private bool _isBusy;

    private readonly AuthService _authService;

    public LoginViewModel()
    {
        _authService = new AuthService();
        LoginCommand = new AsyncRelayCommand(LoginAsync, () => !IsBusy);
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

    public bool IsBusy
    {
        get => _isBusy;
        set 
        { 
            _isBusy = value; 
            OnPropertyChanged();
            ((AsyncRelayCommand)LoginCommand).RaiseCanExecuteChanged();
        }
    }

    public ICommand LoginCommand { get; }

    private async Task LoginAsync()
    {
        IsBusy = true;
        StatusMessage = "Logging in...";

        bool success = await _authService.LoginAsync(Username, Password);

        if (success)
        {
            StatusMessage = "Login successful!";
            // Navigate to another view or raise event here
        }
        else
        {
            StatusMessage = "Invalid username or password.";
        }

        IsBusy = false;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}