using System;
using System.IO;
using System.Linq;
using System.Windows;
using JIDS.Data;
using JIDS.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JIDS
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

            // --- dev/test user seeding (insert after creating 'dbContext' and 'authRepository') ---
            // Decide whether to seed: DEBUG builds OR environment variable set to "1"
            bool shouldSeed = false;
#if DEBUG
            shouldSeed = true;
#endif
            var seedEnv = Environment.GetEnvironmentVariable("JIDS_SEED_USER");
            if (!string.IsNullOrEmpty(seedEnv) && seedEnv == "1")
                shouldSeed = true;

            if (shouldSeed)
            {
                try
                {
                    // Only seed when Users table is empty to avoid clobbering existing data
                    if (!dbContext.Users.Any())
                    {
                        const string devUser = "tester";
                        const string devEmail = "tester@example.com";
                        const string devPassword = "Test123!";

                        // Register synchronously here for startup convenience (development only)
                        authRepository.RegisterUserAsync(devUser, devEmail, devPassword).GetAwaiter().GetResult();

                        // Optional: write to console so tester sees it in VS Output / logs
                        Console.WriteLine($"[DEV] Seeded user: {devUser} / {devPassword}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[DEV] Seeding failed: " + ex);
                }
            }
            // --- end seeding ---

            // DEBUG: show current users in DB so you can confirm the seeded account exists
#if DEBUG
            try
            {
                var users = dbContext.Users.Select(u => new { u.Username, u.Email }).ToList();
                var msg = users.Any() ? $"Users in DB:\n{string.Join("\n", users.Select(u => $"{u.Username} ({u.Email})"))}" : "No users found in DB.";
                MessageBox.Show(msg, "DB Users (debug)", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to read users: {ex.Message}", "DB Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
#endif

            // Show MainWindow as the root window.
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}