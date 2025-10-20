using System.Windows;

public partial class LoginView : Window
{
    public LoginView()
    {
        InitializeComponent();
        DataContext = new LoginViewModel();
    }

    private void OnLoginClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel vm)
        {
            vm.Password = PasswordBox.Password;
        }
    }
}