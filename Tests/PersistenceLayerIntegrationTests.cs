using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JIDS.Data;
using JIDS.Models;
using JIDS.Services.Configuration;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class ConfigurationIntegrationTests : IAsyncLifetime
{
    private readonly Guid _userId = Guid.NewGuid();
    private JetDbContext _db;
    private JsonConfigurationRepository _repository;
    private ConfigurationManager _manager;

    // ----------------------------------------------------
    // Test Initialization / Cleanup
    // ----------------------------------------------------
    public async Task InitializeAsync()
    {
        Console.WriteLine("Initializing in-memory SQLite database...");

        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<JetDbContext>()
            .UseSqlite(connection)
            .Options;

        _db = new JetDbContext(options);
        await _db.Database.EnsureCreatedAsync();

        _repository = new JsonConfigurationRepository(_db, _userId);
        _manager = new ConfigurationManager(_repository, _userId);

        // Seed a user
        _db.Users.Add(new UserDB
        {
            UserID = _userId,
            Username = "IntegrationTestUser",
            Email = "integration@test.com",
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        Console.WriteLine("Database ready and user seeded.");
    }

    public async Task DisposeAsync()
    {
        Console.WriteLine("Disposing in-memory database...");

        await _db.DisposeAsync();

        Console.WriteLine("Database disposed.");
    }

    // ----------------------------------------------------
    // Manual Test Runner
    // ----------------------------------------------------
    public async Task RunTestsAsync()
    {
        Console.WriteLine("Running ConfigurationIntegrationTests...");

        await RunTest(nameof(Configuration_CRUD_FullIntegration_Works_Internal), Configuration_CRUD_FullIntegration_Works_Internal);

        Console.WriteLine("ConfigurationIntegrationTests completed.");
    }

    private async Task RunTest(string name, Func<Task> func)
    {
        Console.WriteLine($"Running: {name}");
        try
        {
            await func();
            Console.WriteLine($"PASS: {name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FAIL: {name} - {ex.Message}");
            throw;
        }
    }

    // ----------------------------------------------------
    // Internal Test Logic
    // ----------------------------------------------------
    private async Task Configuration_CRUD_FullIntegration_Works_Internal()
    {
        // Arrange
        var baseLayout = new JetConfiguration
        {
            ConfigID = Guid.NewGuid(),
            UserID = _userId,
            Name = "BaseLayout",
            CabinDimensions = "7x4.5x1.9m",
            SeatingCapacity = 6,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            InteriorComponents = new List<InteriorComponent>
            {
                new InteriorComponent
                {
                    ComponentID = Guid.NewGuid(),
                    ConfigID = Guid.Empty,
                    Name = "Seat A",
                    Type = "Seating",
                    Tier = "Premium",
                    Material = "Leather",
                    Position = "{x:10, y:20}",
                    CreatedAt = DateTime.UtcNow,
                    PropertiesJson = "{}"
                }
            }
        };

        // Create new configuration
        var newConfig = await _manager.CreateConfigurationAsync("TestConfig", baseLayout);
        if (newConfig == null) throw new Exception("CreateConfigurationAsync failed");

        // Reload from DB
        var loadedConfigs = await _repository.LoadAllAsync();
        if (loadedConfigs.Count != 1) throw new Exception("LoadAllAsync returned incorrect number of configs");

        var loaded = loadedConfigs.First();
        if (loaded.InteriorComponents.Count != 1)
            throw new Exception("Interior components not saved correctly");

        // Clone configuration
        await _manager.InitializeAsync();
        var cloned = await _manager.CloneConfigurationAsync(newConfig.ConfigID);
        if (cloned == null) throw new Exception("CloneConfigurationAsync failed");

        // Save all
        var configsToSave = _repository.LoadAllAsync().Result;
        var saved = await _manager.SaveAllChangesAsync(configsToSave);
        if (!saved) throw new Exception("SaveAllChangesAsync returned false");

        // Delete original config
        var deleteResult = await _manager.DeleteConfigurationAsync(newConfig.ConfigID);
        if (!deleteResult) throw new Exception("DeleteConfigurationAsync failed");

        // Verify only cloned remains
        var remaining = await _repository.LoadAllAsync();
        if (remaining.Count != 1) throw new Exception("Expected only 1 config remaining");

        if (remaining.First().ConfigID != cloned.ConfigID)
            throw new Exception("Cloned config not found after deletion");
    }

    // ----------------------------------------------------
    // xUnit Fact Wrapper
    // ----------------------------------------------------
    [Fact]
    public async Task Configuration_CRUD_FullIntegration_Works() =>
        await Configuration_CRUD_FullIntegration_Works_Internal();
}
