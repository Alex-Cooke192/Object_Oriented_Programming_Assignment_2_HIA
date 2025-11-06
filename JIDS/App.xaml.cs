using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using JetInteriorApp.Data;
using JetInteriorApp.Repositories;
using JetInteriorApp.Interfaces;
using JetInteriorApp.ViewModels;
using JetInteriorApp.Views;
using Microsoft.EntityFrameworkCore;
using JetInteriorApp.Tests;

namespace JetInteriorApp
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

#if DEBUG
            try
            {
                Console.WriteLine("Running application startup test suite...");
                var testMain = new TestMain();
                await testMain.RunAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Startup tests failed: {ex.Message}");
            }
#endif

            // Always run this part in all builds
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string dbPath = Path.Combine(baseDirectory, "Data", "jetconfigs.db");

            Directory.CreateDirectory(Path.GetDirectoryName(dbPath));

            var options = new DbContextOptionsBuilder<JetDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            var dbContext = new JetDbContext(options);
            dbContext.Database.EnsureCreated();

            var authRepository = new AuthRepository(dbContext);
            var loginViewModel = new LoginViewModel(authRepository);
            var loginView = new LoginView();
            loginView.DataContext = loginViewModel;
            loginView.Show();
        }
    }
}


