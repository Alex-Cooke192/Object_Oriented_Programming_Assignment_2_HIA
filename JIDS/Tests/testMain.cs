using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JIDS.Data;

namespace JIDS.Tests
{
    public class TestMain
    {
        public async Task RunAllAsync()
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
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            // 3. Create context
            using var db = new JetDbContext(options);

            // 4. Run table & relationship integrity tests
            var tester = new DatabaseTester(db);
            await tester.RunTestsAsync();

            // 5. Run unit tests on JsonConfigurationRepository
            var jsonRepoUnitTester = new JsonConfigurationRepositoryTests();
            await jsonRepoUnitTester.RunTestsAsync(); 

            // 6. Run unit tests on ConfigurationManager
            var configurationManagerTests = new ConfigurationManagerTests();
            await configurationManagerTests.RunTestsAsync();

            // 7. Run persistence layer integration tests
            Console.WriteLine("\nRunning persistence layer integration tests...");
            try
            {
                var integrationTests = new ConfigurationIntegrationTests();
                await integrationTests.InitializeAsync(); // Set up in-memory DB & dependencies
                await integrationTests.Configuration_CRUD_FullIntegration_Works();
                await integrationTests.DisposeAsync();

                Console.WriteLine("Persistence layer integration tests completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Integration tests failed: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("\nAll tests completed.");
        }
    }
}