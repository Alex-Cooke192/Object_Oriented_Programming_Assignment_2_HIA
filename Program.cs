using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JetInteriorApp.Data;
using JetInteriorApp.Models;
using JetInteriorApp.Interfaces;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting Jet Interior Design System...");

        // 1. Configure EF Core to use SQLite (persistent local DB)
        var options = new DbContextOptionsBuilder<JetDbContext>()
            .UseSqlite("Data Source=Data/jetconfigs.db")
            .Options;

        // 2. Create DB context
        using var db = new JetDbContext(options);

        // Optional: Recreate DB
        Console.WriteLine("Recreating database...");
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
        Console.WriteLine("Database recreated successfully!");

        // 3. Ensure DB and tables exist
        await db.Database.EnsureCreatedAsync();

        // 4. Run integrity checks
        var tester = new DatabaseTester(db);
        await tester.RunTestsAsync();

        // 5. Example current user
        Guid currentUserId = Guid.NewGuid();

        // 6. Seed a test user if not exists
        var existingUser = await db.Users.FirstOrDefaultAsync(u => u.UserID == currentUserId);
        if (existingUser == null)
        {
            var user = new UserDB
            {
                UserID = currentUserId,
                Username = "TestUser",
                Email = "test@example.com",
                CreatedAt = DateTime.UtcNow
            };
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();

            Console.WriteLine($"Created test user \nUsername:{user.Username} \nUser ID:{user.UserID}");
        }

        // 7. Initialize repository interface
        var repo = new JsonConfigurationRepository(db, currentUserId);

        // 8. Load configurations for this user
        var configs = await repo.LoadAllAsync();

        Console.WriteLine("\n🛩 Loaded Jet Configurations:");
        if (configs.Count == 0)
            Console.WriteLine("   (No configurations found)");
        else
            foreach (var config in configs)
                Console.WriteLine($" - {config.Name} (Seats: {config.SeatingCapacity})");

        // 9. Run repository tests at the end
        Console.WriteLine("\nRunning repository interface tests...");
        var repoTests = new JsonConfigurationRepositoryTests();
        await repoTests.RunTestsAsync();

        Console.WriteLine("\nAll tests completed successfully.");
    }
}