using System;
using System.IO;
using System.Windows;
using JetInteriorApp.Data;
using JetInteriorApp.Repositories;
using JetInteriorApp.Interfaces;
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

            // Resolve database path
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string dbPath = Path.Combine(baseDirectory, "Data", "jetconfigs.db");

            // Ensure Data folder exists
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath));

            // Build DbContext options
            var options = new DbContextOptionsBuilder<JetDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            // Create context and ensure database
            var dbContext = new JetDbContext(options);
            dbContext.Database.EnsureCreated();

            // Inject repository into ViewModel
            IAuthRepository authRepository = new AuthRepository(dbContext);
            var loginVM = new LoginViewModel(authRepository);

            // Launch LoginView
            var loginView = new LoginView
            {
                DataContext = loginVM
            };
            loginView.Show();
        }
    }
}

