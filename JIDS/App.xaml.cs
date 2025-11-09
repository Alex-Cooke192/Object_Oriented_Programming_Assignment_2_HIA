using System;
using System.IO;
using System.Windows;
using JIDS.Data;
using JIDS.Repositories;
using Microsoft.EntityFrameworkCore;
using JIDS;

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
            var dir = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            // Configure EF Core SQLite
            var options = new DbContextOptionsBuilder<JetDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            // Create DB context and database file if missing
            var dbContext = new JetDbContext(options);
            dbContext.Database.EnsureCreated();

            // Register lightweight application services in Application resources
            var navigationService = new JIDS.Services.NavigationService();
            var userSession = new JIDS.Services.UserSessionService();

            Application.Current.Resources["NavigationService"] = navigationService;
            Application.Current.Resources["UserSessionService"] = userSession;
            Application.Current.Resources["DbContext"] = dbContext;

            // Register auth repository (stateless; doesn't require a logged-in user)
            var authRepository = new AuthRepository(dbContext);
            Application.Current.Resources["AuthRepository"] = authRepository;

            // Show MainWindow as the root window..
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
