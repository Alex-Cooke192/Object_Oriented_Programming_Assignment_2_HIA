using Microsoft.EntityFrameworkCore;

namespace JetInteriorApp.Tests
{
    public class MainTest
    {
        public static async Task RunAllAsync()
        {
            var options = new DbContextOptionsBuilder<JetDbContext>()
                .UseSqlite("Data Source=Data/jetconfigs.db")
                .Options;

            using var db = new JetDbContext(options);
            db.Database.EnsureCreated();

            var tester = new DatabaseTester(db);
            await tester.RunTestsAsync();
        }
    }
}