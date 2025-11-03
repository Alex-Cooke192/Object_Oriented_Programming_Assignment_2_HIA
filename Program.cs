using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JetInteriorApp.Data;
using JetInteriorApp.Tests; 

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting Jet Interior Design System...");

        // Configure EF Core with SQLite
        var options = new DbContextOptionsBuilder<JetDbContext>()
            .UseSqlite("Data Source=Data/jetconfigs.db")
            .Options;

        using var db = new JetDbContext(options);

        // Ensure database exists (or recreate if needed)
        Console.WriteLine("Recreating database...");
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
        Console.WriteLine("Database recreated successfully!");

        // Run all tests via MainTest
        try
        {
            var MainTester = new TestMain(); 
            await MainTester.RunAllAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during tests: {ex.Message}");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}