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
        Console.WriteLine("Starting Jet Interior App...");

        // 1. Configure EF Core to use SQLite
        var options = new DbContextOptionsBuilder<JetDbContext>()
            .UseSqlite("Data Source=Data/jetconfigs.db")
            .Options;

        // 2. Create DB context
        using var db = new JetDbContext(options);

        // Optional: Delete & Recreate Database
        Console.WriteLine("Recreating database...");
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
        Console.WriteLine("Database recreated successfully!");

        // 3. Create database + tables if not already present
        Console.WriteLine("Ensuring database and tables are created...");
        await db.Database.EnsureCreatedAsync();

        // 4. Run integrity checks 
        var tester = new DatabaseTester(db);
        await tester.RunTestsAsync();

        // 5. Example current user
        Guid currentUserId = Guid.NewGuid();

        // If needed, seed a user so that repository operations work
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

            Console.WriteLine($"👤 Created test user: {user.Username} ({user.UserID})");
        }

        // 6. Initialize JSON repository
        var repo = new JsonConfigurationRepository(db, currentUserId);

        // 7. Example usage: Load all configurations for this user
        var configs = await repo.LoadAllAsync();

        Console.WriteLine("\n🛩 Loaded Jet Configurations:");
        foreach (var config in configs)
        {
            Console.WriteLine($" - {config.Name} (Seats: {config.SeatingCapacity})");
        }

        Console.WriteLine("\n✅ Program finished successfully.");
    }
}