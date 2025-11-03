using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetInteriorApp.Data;
using JetInteriorApp.Models;
using JetInteriorApp.Services.Configuration;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class ConfigurationIntegrationTests : IAsyncLifetime
{
    private readonly Guid _userId = Guid.NewGuid();
    private JetDbContext _db;
    private JsonConfigurationRepository _repository;
    private ConfigurationManager _manager;

    public async Task InitializeAsync()
    {
        Console.WriteLine("Initializing in-memory database...");

        var connection = new SqliteConnection("DataSource=:memory:"); // in-memory DB
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<JetDbContext>()
            .UseSqlite(connection)
            .Options;

        _db = new JetDbContext(options);
        await _db.Database.EnsureCreatedAsync();

        _repository = new JsonConfigurationRepository(_db, _userId);
        _manager = new ConfigurationManager(_repository, _userId);

        // Seed user
        _db.Users.Add(new UserDB
        {
            UserID = _userId,
            Username = "IntegrationTestUser",
            Email = "integration@test.com",
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        Console.WriteLine("Database initialized and test user seeded.");
    }

    public async Task DisposeAsync()
    {
        Console.WriteLine("Disposing database...");
        await _db.DisposeAsync();
        Console.WriteLine("Database disposed.");
    }

    [Fact]
    public async Task Configuration_CRUD_FullIntegration_Works()
    {
        Console.WriteLine("Starting Configuration_CRUD_FullIntegration_Works test...");

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

        Console.WriteLine("\nCreating new configuration via ConfigurationManager...");
        var newConfig = await _manager.CreateConfigurationAsync("TestConfig", baseLayout);
        Assert.NotNull(newConfig);
        Console.WriteLine($"\nConfiguration created: {newConfig.Name} ({newConfig.ConfigID})");

        // Reload from DB
        Console.WriteLine("\nLoading configurations from repository...");
        var loadedConfigs = await _repository.LoadAllAsync();
        Assert.Single(loadedConfigs);
        var loaded = loadedConfigs.First();
        Console.WriteLine($"\nLoaded configuration: {loaded.Name} with {loaded.InteriorComponents.Count} components");

        // Clone configuration
        Console.WriteLine("\nCloning configuration...");
        await _manager.InitializeAsync();
        var cloned = await _manager.CloneConfigurationAsync(newConfig.ConfigID);
        Assert.NotNull(cloned);
        Console.WriteLine($"\nCloned configuration: {cloned.Name} ({cloned.ConfigID})");

        // Save all
        Console.WriteLine("\nSaving all in-memory configurations...");
        var saveAllResult = await _manager.SaveAllChangesAsync();
        Assert.True(saveAllResult);
        Console.WriteLine("\nAll configurations saved successfully.");

        // Delete original
        Console.WriteLine($"\nDeleting original configuration: {newConfig.Name}...");
        var deleteResult = await _manager.DeleteConfigurationAsync(newConfig.ConfigID);
        Assert.True(deleteResult);
        Console.WriteLine("\nOriginal configuration deleted.");

        // Verify remaining
        var remainingConfigs = await _repository.LoadAllAsync();
        Assert.Single(remainingConfigs);
        Assert.Equal(cloned.ConfigID, remainingConfigs.First().ConfigID);
        Console.WriteLine($"\nRemaining configuration: {remainingConfigs.First().Name} ({remainingConfigs.First().ConfigID})");

        Console.WriteLine("\nConfiguration_CRUD_FullIntegration_Works test completed successfully.");
    }
}
