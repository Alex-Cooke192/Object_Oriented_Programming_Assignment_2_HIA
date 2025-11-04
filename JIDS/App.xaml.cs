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

            // Set up the database path
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string dbPath = Path.Combine(baseDirectory, "Data", "jetconfigs.db");

            // Ensure Data folder exists
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath));

            // Set up DbContext options
            var options = new DbContextOptionsBuilder<JetDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            // Create the DbContext and ensure database is created
            var dbContext = new JetDbContext(options);
            dbContext.Database.EnsureCreated();  // Creates the database if it doesn't exist

            // Create the AuthRepository and LoginViewModel, passing the dbContext
            var authRepository = new AuthRepository(dbContext);
            var loginViewModel = new LoginViewModel(authRepository);

            // Create the LoginView
            var loginView = new LoginView();  // Parameterless constructor
            loginView.DataContext = loginViewModel;  // Inject ViewModel into the view

            // Show the LoginView
            loginView.Show();
        }
    }
}

