using Microsoft.EntityFrameworkCore;

namespace JetInteriorApp.Tests
{
    public class MainTest
    {
        public static async Task RunAllAsync()
        {
            var options = new DbContextOptionsBuilder<JetDbContext>()
                .UseSqlite("Data Source=Object_Oriented_Programming_Assignment_2_HIA-HAI_29-add-c-database/Data/jetconfigs.db")
                .Options;

            using var db = new JetDbContext(options);
            db.Database.EnsureCreated();

            var tester = new DatabaseTester(db);
            await tester.RunTestsAsync();
        }
    }
}