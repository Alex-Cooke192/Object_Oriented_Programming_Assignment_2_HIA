using System.Windows;
using System.Windows.Controls;
using JetInteriorApp.ViewModels;
using JetInteriorApp.Repositories;
using JetInteriorApp.Interfaces;
using JetInteriorApp.Data;
using Microsoft.EntityFrameworkCore;

namespace JetInteriorApp.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();

            // Build DbContext options 
            var options = new DbContextOptionsBuilder<JetDbContext>()
                .UseSqlite("DB_FILE_PATH")
                .Options;

            // Create context using options
            var dbContext = new JetDbContext(options);

            // Create database if it doesn't exist yet
            dbContext.Database.EnsureCreated();

            // Create repository and inject it into the ViewModel
            IAuthRepository authRepository = new AuthRepository(dbContext);
            DataContext = new LoginViewModel(authRepository);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}

