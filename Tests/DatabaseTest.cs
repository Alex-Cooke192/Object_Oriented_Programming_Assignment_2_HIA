using System;
using System.Linq;
using System.Threading.Tasks;
using JetInteriorApp.Models;
using JetInteriorApp.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class DatabaseTester
{
    private readonly JetDbContext _db;

    public DatabaseTester()
    {
        var options = new DbContextOptionsBuilder<JetDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new JetDbContext(options);
        _db.Database.EnsureCreated();
    }


    // --------------------------------------------------
    // Manual test runner
    // --------------------------------------------------
    public async Task RunTestsAsync()
    {
        Console.WriteLine("Running Database Integrity Tests...");

        await TestTablesAsync();
        await TestUserForeignKeysAsync();
        await TestCreateUserAsync();
        await TestCreateJetConfigurationAsync();
        await TestTablesAsync();

        Console.WriteLine("All database tests completed.");
    }

    // --------------------------------------------------
    // Test Logic
    // --------------------------------------------------

    private async Task TestTablesAsync()
    {
        await TestTableAsync("Users", _db.Users.CountAsync());
        await TestTableAsync("JetConfigurations", _db.JetConfigurations.CountAsync());
        await TestTableAsync("InteriorComponents", _db.InteriorComponents.CountAsync());
        await TestTableAsync("ComponentSettings", _db.ComponentSettings.CountAsync());
    }

    private async Task TestTableAsync(string table, Task<int> task)
    {
        try
        {
            int count = await task;
            Console.WriteLine($"PASS: {table} - {count} row(s)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FAIL: {table} - {ex.Message}");
            throw;
        }
    }

    private async Task TestUserForeignKeysAsync()
    {
        try
        {
            var validUserIds = await _db.Users.Select(u => u.UserID).ToListAsync();
            var configs = await _db.JetConfigurations.ToListAsync();

            foreach (var config in configs)
            {
                if (!validUserIds.Contains(config.UserID))
                    throw new Exception($"JetConfiguration '{config.Name}' has invalid UserID {config.UserID}");
            }

            Console.WriteLine("PASS: JetConfiguration to User foreign key integrity validated");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FAIL: JetConfiguration foreign key validation - {ex.Message}");
            throw;
        }
    }

    private async Task TestCreateUserAsync()
    {
        try
        {
            var newUser = User.CreateNew(
                username: "testuser_" + Guid.NewGuid(),
                email: "testuser@example.com",
                plainPassword: "SecurePassword123!"
            );

            var newUserDB = new UserDB
            {
                UserID = newUser.UserID,
                Username = newUser.Username,
                Email = newUser.Email,
                PasswordHash = newUser.PasswordHash,
                CreatedAt = newUser.CreatedAt
            };

            _db.Users.Add(newUserDB);
            await _db.SaveChangesAsync();

            Console.WriteLine($"PASS: Created user '{newUser.Username}' ({newUser.UserID})");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FAIL: CreateUser - {ex.Message}");
            throw;
        }
    }

    private async Task TestCreateJetConfigurationAsync()
    {
        try
        {
            var existingUser = await _db.Users.FirstOrDefaultAsync();
            if (existingUser == null)
                throw new Exception("No users exist to link JetConfiguration");

            var config = new JetConfiguration
            {
                ConfigID = Guid.NewGuid(),
                UserID = existingUser.UserID,
                Name = "Business Jet A",
                CabinDimensions = "10x3x2.5m",
                SeatingCapacity = 8,
                Version = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var configDB = new JetConfigurationDB
            {
                ConfigID = config.ConfigID,
                UserID = config.UserID,
                Name = config.Name,
                CabinDimensions = config.CabinDimensions,
                SeatingCapacity = config.SeatingCapacity,
                Version = config.Version ?? 0,
                CreatedAt = config.CreatedAt,
                UpdatedAt = config.UpdatedAt
            };

            _db.JetConfigurations.Add(configDB);
            await _db.SaveChangesAsync();

            var retrieved = await _db.JetConfigurations
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Name == "Business Jet A");

            Assert.NotNull(retrieved);
            Assert.Equal(existingUser.UserID, retrieved.UserID);

            Console.WriteLine("PASS: JetConfiguration created and foreign key validated");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FAIL: Create JetConfiguration - {ex.Message}");
            throw;
        }
    }

    // --------------------------------------------------
    // xUnit Facts (for dotnet test)
    // --------------------------------------------------

    [Fact]
    public async Task Tables_Exist_Fact() => await TestTablesAsync();

    [Fact]
    public async Task ForeignKeys_Valid_Fact() => await TestUserForeignKeysAsync();

    [Fact]
    public async Task CreateUser_Fact() => await TestCreateUserAsync();

    [Fact]
    public async Task CreateJetConfiguration_Fact() => await TestCreateJetConfigurationAsync();
}
