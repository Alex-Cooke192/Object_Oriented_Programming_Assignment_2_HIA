using System;
using System.Linq;
using System.Threading.Tasks;
using JetInteriorApp.Models; 
using Microsoft.EntityFrameworkCore;
using JetInteriorApp.Data;
using System.ComponentModel;

public class DatabaseTester
{
    private readonly JetDbContext _db;

    public DatabaseTester(JetDbContext db)
    {
        _db = db;
    }

    public async Task RunTestsAsync()
    {
        Console.WriteLine("🔍 Testing database tables...\n");

        // Test the four tables
        await TestTableAsync("Users", _db.Users.CountAsync());
        await TestTableAsync("JetConfigurations", _db.JetConfigurations.CountAsync());
        await TestTableAsync("InteriorComponents", _db.InteriorComponents.CountAsync());
        await TestTableAsync("ComponentSettings", _db.ComponentSettings.CountAsync());

        // Verify foreign key relationships
        await CheckJetConfigurationUserLinksAsync();

        //Create a test user & add to database
        await CreateUserTestAsync(); 

        Console.WriteLine("\n All table checks complete.");
    }

    private async Task TestTableAsync(string tableName, Task<int> countTask)
    {
        try
        {
            int count = await countTask;
            Console.WriteLine($"PASS: {tableName}: {count} row(s)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {tableName}: Error - {ex.Message}");
        }
    }

    private async Task CheckJetConfigurationUserLinksAsync()
    {
        Console.WriteLine("\n Checking JetConfigurations → Users foreign key integrity...");

        var validUserIds = await _db.Users.Select(u => u.UserID).ToListAsync();
        var configs = await _db.JetConfigurations.ToListAsync();

        foreach (var config in configs)
        {
            if (!validUserIds.Contains(config.UserID))
            {
                Console.WriteLine($" JetConfiguration '{config.Name}' has invalid UserID: {config.UserID}");
            }
        }

        Console.WriteLine(" JetConfiguration user link check complete.");
    }

    public async Task CreateUserTestAsync()
    {
        try
        {
            // Create a new user
            var newUser = new User
            {
                Username = "testuser_" + Guid.NewGuid(), // unique name
                Email = "testuser@example.com",
                CreatedAt = DateTime.UtcNow
                // add other required fields here
            };

            //Set Password securely
            newUser.SetPassword("SecurePassword123!"); 

            //Convert to UserDB format so can be added to database
            var newUserDB = new UserDB
            {
                UserID = Guid.NewGuid(),              // generate a new ID for the database
                Username = newUser.Username,
                Email = newUser.Email,
                PasswordHash = newUser.PasswordHash,  // hashed password
                CreatedAt = newUser.CreatedAt
            };

            // Add user to the DbContext
            _db.Users.Add(newUserDB);

            // Commit to the database
            await _db.SaveChangesAsync();

            Console.WriteLine($"✅ User created successfully with ID: {newUser.UserID}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error creating user: {ex.Message}");
        }
    }
}