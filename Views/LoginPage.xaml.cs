using System.Windows;

namespace MyApp.Views
{
    public partial class LoginPage : Window
    {
        private LoginViewModel ViewModel => (LoginViewModel)DataContext;

        public LoginPage()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
        }

        private void OnLoginButtonClick(object sender, RoutedEventArgs e)
        {
            ViewModel.Password = PasswordBox.Password;
        }
    }
}