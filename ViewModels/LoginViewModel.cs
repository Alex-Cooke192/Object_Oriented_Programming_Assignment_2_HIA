using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using YourApp.Models;

namespace YourApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly Authenticator _authenticator;

        public LoginViewModel()
        {
            _authenticator = new Authenticator();
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
        }

        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        private bool CanExecuteLogin(object parameter) => true;

        private void ExecuteLogin(object parameter)
        {
            var user = _authenticator.Authenticate(Username, Password);

            if (user != null && _authenticator.Authorise(user))
                StatusMessage = "Login successful!";
            else
                StatusMessage = "Invalid username or password.";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}