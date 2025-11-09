using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JIDS.Data;
using JIDS.Models;
using JIDS.Interfaces;

public class JsonConfigurationRepositoryTests
{
    private JetDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<JetDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // isolated per test
            .Options;
        return new JetDbContext(options);
    }

    [Fact]
    public async Task RunTestsAsync()
    {
        Console.WriteLine("\n===== Running JsonConfigurationRepository Tests =====");

        await RunTestAsync(nameof(SaveConfigAsync_Should_Add_New_Config), SaveConfigAsync_Should_Add_New_Config);
        await RunTestAsync(nameof(SaveConfigAsync_Should_Update_Existing_Config), SaveConfigAsync_Should_Update_Existing_Config);
        await RunTestAsync(nameof(LoadAllAsync_Should_Return_User_Configs), LoadAllAsync_Should_Return_User_Configs);
        await RunTestAsync(nameof(SaveAllAsync_Should_Remove_Deleted_Configs), SaveAllAsync_Should_Remove_Deleted_Configs);

        Console.WriteLine("\n===== All JsonConfigurationRepository Tests Executed =====");
    }

    private async Task RunTestAsync(string testName, Func<Task> testFunc)
    {
        Console.WriteLine($"\n--- Running {testName} ---");
        try
        {
            await testFunc();
            Console.WriteLine($"[{testName}] PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{testName}] FAILED");
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    [Fact]
    public async Task SaveConfigAsync_Should_Add_New_Config()
    {
        using var db = GetInMemoryDbContext();
        var userId = Guid.NewGuid();
        var repo = new JsonConfigurationRepository(db, userId);

        var newConfig = new JetConfiguration
        {
            ConfigID = Guid.NewGuid(),
            UserID = userId,
            Name = "Test Jet Config",
            CabinDimensions = "10x3x2.5m",
            SeatingCapacity = 8,
            Version = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            InteriorComponents = new List<InteriorComponent>
            {
                new InteriorComponent
                {
                    ComponentID = Guid.NewGuid(),
                    ConfigID = Guid.NewGuid(),
                    Name = "Seat A1",
                    Type = "Seat",
                    Tier = "Business",
                    Material = "Leather",
                    Position = "{\"row\":1,\"col\":1}",
                    CreatedAt = DateTime.UtcNow,
                    PropertiesJson = "{\"recline\": true}"
                }
            }
        };

        var result = await repo.SaveConfigAsync(newConfig);
        Assert.True(result);

        var saved = await db.JetConfigurations
            .Include(c => c.InteriorComponents)
            .FirstOrDefaultAsync();

        Assert.NotNull(saved);
        Assert.Equal(newConfig.Name, saved.Name);
        Assert.Single(saved.InteriorComponents);

        Console.WriteLine($"Config added: {saved.Name}, Components: {saved.InteriorComponents.Count}");
    }

    [Fact]
    public async Task SaveConfigAsync_Should_Update_Existing_Config()
    {
        using var db = GetInMemoryDbContext();
        var userId = Guid.NewGuid();
        var repo = new JsonConfigurationRepository(db, userId);

        var configId = Guid.NewGuid();
        var existingDb = new JetConfigurationDB
        {
            ConfigID = configId,
            UserID = userId,
            Name = "Original Name",
            CabinDimensions = "Original Dimensions",
            SeatingCapacity = 6,
            Version = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            InteriorComponents = new List<InteriorComponentDB>()
        };
        db.JetConfigurations.Add(existingDb);
        await db.SaveChangesAsync();

        var updatedConfig = new JetConfiguration
        {
            ConfigID = configId,
            UserID = userId,
            Name = "Updated Name",
            CabinDimensions = "Updated Dimensions",
            SeatingCapacity = 10,
            Version = 3,
            CreatedAt = existingDb.CreatedAt,
            UpdatedAt = DateTime.UtcNow,
            InteriorComponents = new List<InteriorComponent>()
        };

        var result = await repo.SaveConfigAsync(updatedConfig);
        Assert.True(result);

        var retrieved = await db.JetConfigurations.FirstAsync();
        Assert.Equal("Updated Name", retrieved.Name);
        Assert.Equal(10, retrieved.SeatingCapacity);

        Console.WriteLine($"Config updated successfully: {retrieved.Name}, Seats: {retrieved.SeatingCapacity}");
    }

    [Fact]
    public async Task LoadAllAsync_Should_Return_User_Configs()
    {
        using var db = GetInMemoryDbContext();
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        db.JetConfigurations.AddRange(
            new JetConfigurationDB
            {
                ConfigID = Guid.NewGuid(),
                UserID = userId,
                Name = "Jet 1",
                CabinDimensions = "10x3x2.5m",
                SeatingCapacity = 6,
                Version = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                InteriorComponents = new List<InteriorComponentDB>()
            },
            new JetConfigurationDB
            {
                ConfigID = Guid.NewGuid(),
                UserID = otherUserId,
                Name = "Other User Jet",
                CabinDimensions = "1.5x4x8",
                SeatingCapacity = 6,
                Version = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                InteriorComponents = new List<InteriorComponentDB>()
            }
        );
        await db.SaveChangesAsync();

        var repo = new JsonConfigurationRepository(db, userId);
        var configs = await repo.LoadAllAsync();

        Assert.Single(configs);
        Assert.Equal("Jet 1", configs[0].Name);

        Console.WriteLine($"Loaded {configs.Count} config(s) for user {userId}.");
        Console.WriteLine($"Config name: {configs[0].Name}");
    }

    [Fact]
    public async Task SaveAllAsync_Should_Remove_Deleted_Configs()
    {
        using var db = GetInMemoryDbContext();
        var userId = Guid.NewGuid();
        var repo = new JsonConfigurationRepository(db, userId);

        var config1 = new JetConfigurationDB
        {
            ConfigID = Guid.NewGuid(),
            UserID = userId,
            Name = "Config 1",
            CabinDimensions = "5x3x2.5m",
            SeatingCapacity = 8,
            Version = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            InteriorComponents = new List<InteriorComponentDB>()
        };
        var config2 = new JetConfigurationDB
        {
            ConfigID = Guid.NewGuid(),
            UserID = userId,
            Name = "Config 2",
            CabinDimensions = "6x4x3m",
            SeatingCapacity = 10,
            Version = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            InteriorComponents = new List<InteriorComponentDB>()
        };
        db.JetConfigurations.AddRange(config1, config2);
        await db.SaveChangesAsync();

        var updatedList = new List<JetConfiguration>
        {
            new JetConfiguration
            {
                ConfigID = config1.ConfigID,
                UserID = userId,
                Name = "Config 1",
                CabinDimensions = "4x4x4m",
                SeatingCapacity = 8,
                Version = 1,
                CreatedAt = config1.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                InteriorComponents = new List<InteriorComponent>()
            }
        };

        await repo.SaveAllAsync(updatedList);

        var remaining = await db.JetConfigurations.ToListAsync();
        Assert.Single(remaining);
        Assert.Equal("Config 1", remaining[0].Name);

        Console.WriteLine($"Remaining config count: {remaining.Count}");
        Console.WriteLine($"Remaining config: {remaining[0].Name}");
    }
}