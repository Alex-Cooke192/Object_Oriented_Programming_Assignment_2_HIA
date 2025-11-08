using System;
using System.IO;
using System.Windows;
using JetInteriorApp.Data;
using JetInteriorApp.Repositories;
using JetInteriorApp.ViewModels;
using JetInteriorApp.Views;
using Microsoft.EntityFrameworkCore;

namespace JetInteriorApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set up database path
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string dbPath = Path.Combine(baseDirectory, "Data", "jetconfigs.db");

            // Ensure Data folder exists
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath));

            // Configure EF Core SQLite
            var options = new DbContextOptionsBuilder<JetDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            // Create DB context and database file if missing
            var dbContext = new JetDbContext(options);
            dbContext.Database.EnsureCreated();

            // Inject repository and ViewModel
            var authRepository = new AuthRepository(dbContext);
            var loginViewModel = new LoginViewModel(authRepository);

            // Launch Login Window
            var loginView = new LoginView
            {
                DataContext = loginViewModel
            };

            loginView.Show();
        }
    }
}

