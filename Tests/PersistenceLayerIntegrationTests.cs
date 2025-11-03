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
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"jet_integration_{Guid.NewGuid()}.db");
    private JetDbContext _db;
    private JsonConfigurationRepository _repository;
    private ConfigurationManager _manager;

    public async Task InitializeAsync()
    {
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
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();   // Just dispose, no file deletion needed
    }


    [Fact]
    public async Task Configuration_CRUD_FullIntegration_Works()
    {
        // Arrange: create a base layout configuration
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

        // Act 1: Create configuration via ConfigurationManager
        var newConfig = await _manager.CreateConfigurationAsync("TestConfig", baseLayout);
        Assert.NotNull(newConfig);
        Assert.Equal("TestConfig", newConfig.Name);

        // Act 2: Reload from DB using repository
        var loadedConfigs = await _repository.LoadAllAsync();
        Assert.Single(loadedConfigs);
        var loaded = loadedConfigs.First();
        Assert.Equal(newConfig.Name, loaded.Name);
        Assert.Single(loaded.InteriorComponents);

        // Act 3: Clone the configuration
        await _manager.InitializeAsync(); // load configs into memory
        var cloned = await _manager.CloneConfigurationAsync(newConfig.ConfigID);
        Assert.NotNull(cloned);
        Assert.NotEqual(newConfig.ConfigID, cloned.ConfigID);
        Assert.Equal($"{newConfig.Name} (Clone)", cloned.Name);

        // Act 4: Save all in-memory configs
        var saveAllResult = await _manager.SaveAllChangesAsync();
        Assert.True(saveAllResult);

        // Act 5: Delete one configuration
        var deleteResult = await _manager.DeleteConfigurationAsync(newConfig.ConfigID);
        Assert.True(deleteResult);

        // Verify that only one (the clone) remains
        var remainingConfigs = await _repository.LoadAllAsync();
        Assert.Single(remainingConfigs);
        Assert.Equal(cloned.ConfigID, remainingConfigs.First().ConfigID);
    }
}
