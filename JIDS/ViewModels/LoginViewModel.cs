using JetInteriorApp.Helpers;
using JetInteriorApp.Interfaces;
using JetInteriorApp.Repositories;
using JetInteriorApp.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace JetInteriorApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IAuthRepository _authRepository;

        public LoginViewModel(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
            LoginCommand = new RelayCommand(async _ => await LoginAsync());
            RegisterCommand = new RelayCommand(async _ => await RegisterAsync());
        }

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        private async Task LoginAsync()
        {
            bool success = await _authRepository.ValidateUserAsync(Username, Password);

            if (success)
            {
                StatusMessage = $"Welcome, {Username}!";

                // Open the main window
                var mainWindow = new MainWindow();
                mainWindow.Show();

                // Close the login window
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is LoginView)
                    {
                        window.Close();
                        break;
                    }
                }
            }
            else
            {
                StatusMessage = "Invalid username or password.";
            }
        }


        private async Task RegisterAsync()
        {
            if (string.IsNullOrEmpty(Email))
            {
                StatusMessage = "Email is required for registration.";
                return;
            }

            bool success = await _authRepository.RegisterUserAsync(Username, Email, Password);
            StatusMessage = success ? $"User {Username} registered successfully." : "Username already exists.";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
