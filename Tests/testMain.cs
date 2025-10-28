using Microsoft.EntityFrameworkCore;

namespace JetInteriorApp.Tests
{
    public class MainTest
    {
        public static async Task RunAllAsync()
        {
            string dbPath = "Data/jetconfigs.db";

            // Check if the database file exists
            if (!File.Exists(dbPath))
            {
                Console.WriteLine($"❌ Database file not found at '{dbPath}'. Please ensure it exists before running tests.");
                return;
            }

            var options = new DbContextOptionsBuilder<JetDbContext>()
                .UseSqlite("Data Source={dbPath}")
                .Options;

            using var db = new JetDbContext(options);

            var tester = new DatabaseTester(db);
            await tester.RunTestsAsync();
        }
    }
}