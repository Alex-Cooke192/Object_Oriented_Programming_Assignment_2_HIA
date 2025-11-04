using JetInteriorApp.Data;
using JetInteriorApp.Repositories;
using JetInteriorApp.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace JetInteriorApp.Views
{
    public partial class LoginView : Window
    {
        // Parameterless constructor
        public LoginView()
        {
            InitializeComponent();
        }

        // This method will be called when the PasswordBox content changes
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                // Update the Password property in the ViewModel whenever the PasswordBox content changes
                vm.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}


