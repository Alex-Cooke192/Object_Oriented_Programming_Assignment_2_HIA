using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JetInteriorApp.Data;

namespace JetInteriorApp.Tests
{
    public class MainTest
    {
        public static async Task RunAllAsync()
        {
            string dbPath = "Data/jetconfigs.db";

            Console.WriteLine("Starting Database Integrity Tests...");

            // 1. Check if database file exists
            if (!File.Exists(dbPath))
            {
                Console.WriteLine($"Database file not found at '{dbPath}'.");
                Console.WriteLine("Please ensure it exists or run the main program to initialize it first.");
                return;
            }

            // 2. Configure EF Core with SQLite
            var options = new DbContextOptionsBuilder<JetDbContext>()
                .UseSqlite($"Data Source={dbPath}") // fixed interpolation
                .Options;

            // 3. Create context
            using var db = new JetDbContext(options);

            // 4. Run table & relationship integrity tests
            var tester = new DatabaseTester(db);
            await tester.RunTestsAsync();

            // 5. Configuration Manager Tests
            /*
            var configurationManagerTests = new ConfigurationManagerTests();

            configurationManagerTests.GetConfiguration_ReturnsCorrectConfig();
            configurationManagerTests.CreateConfiguration_AddsNewConfig();
            configurationManagerTests.CloneConfiguration_CreatesCopyWithModifiedName();
            configurationManagerTests.DeleteConfiguration_RemovesConfig();
            configurationManagerTests.SaveAllChanges_ReturnsTrue();
            */

            Console.WriteLine("\nAll tests completed successfully.");
        }
    }
}
