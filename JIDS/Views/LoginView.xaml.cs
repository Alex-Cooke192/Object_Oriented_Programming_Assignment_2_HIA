using JIDS.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace JIDS.Views
{
    public partial class LoginView : Page
    {
        public LoginView()
        {
            InitializeComponent();
        }

        // Update VM password on change (PasswordBox can't be bound securely)
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}