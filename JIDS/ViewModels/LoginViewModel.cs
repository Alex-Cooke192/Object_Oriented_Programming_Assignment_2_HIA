using JetInteriorApp.Helpers;
using JetInteriorApp.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using JetInteriorApp.Data;
using JetInteriorApp.Models;
using JetInteriorApp.Services.Configuration;
using System.Windows;

namespace JetInteriorApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IAuthRepository _authRepository;
        private readonly IUserSessionService _userSession;
        private readonly INavigationService _navigationService;

        public LoginViewModel(IAuthRepository authRepository, IUserSessionService userSession, INavigationService navigationService)
        {
            _authRepository = authRepository;
            _userSession = userSession;
            _navigationService = navigationService;

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
            var userDb = await _authRepository.ValidateUserAsync(Username, Password);

            if (userDb != null)
            {
                StatusMessage = $"Welcome, {userDb.Username}!";

                // Create domain User and set session
                var domainUser = User.FromExisting(userDb.UserID, userDb.Username, userDb.Email ?? string.Empty, userDb.PasswordHash ?? string.Empty, userDb.CreatedAt);
                _userSession.SetCurrentUser(domainUser);

                // Register user-scoped configuration repository/manager in Application resources
                if (Application.Current.Resources["DbContext"] is JetDbContext db)
                {
                    var repo = new JsonConfigurationRepository(db, userDb.UserID);
                    var manager = new ConfigurationManager(repo, userDb.UserID);

                    Application.Current.Resources["ConfigurationRepository"] = repo;
                    Application.Current.Resources["ConfigurationWriter"] = manager;
                }

                // Navigate to configuration list (MainWindow will host the view inside its frame)
                _navigationService.NavigateTo("ConfigurationList");
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