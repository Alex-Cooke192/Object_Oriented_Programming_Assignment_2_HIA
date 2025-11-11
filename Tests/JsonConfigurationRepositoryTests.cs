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
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new JetDbContext(options);
    }

    // ----------------------------------------------------
    // Manual Test Runner
    // ----------------------------------------------------
    public async Task RunTestsAsync()
    {
        Console.WriteLine("Running JsonConfigurationRepository tests...");

        await RunTest(nameof(SaveConfigAsync_Should_Add_New_Config), SaveConfigAsync_Should_Add_New_Config);
        await RunTest(nameof(SaveConfigAsync_Should_Update_Existing_Config), SaveConfigAsync_Should_Update_Existing_Config);
        await RunTest(nameof(LoadAllAsync_Should_Return_User_Configs), LoadAllAsync_Should_Return_User_Configs);
        await RunTest(nameof(SaveAllAsync_Should_Remove_Deleted_Configs), SaveAllAsync_Should_Remove_Deleted_Configs);

        Console.WriteLine("JsonConfigurationRepository tests completed.");
    }

    private async Task RunTest(string testName, Func<Task> testFunc)
    {
        Console.WriteLine($"Running: {testName}");

        try
        {
            await testFunc();
            Console.WriteLine($"PASS: {testName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FAIL: {testName} - {ex.Message}");
            throw;
        }
    }

    // ----------------------------------------------------
    // Internal Test Logic
    // ----------------------------------------------------

    private async Task SaveConfigAsync_Should_Add_New_Config()
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
        if (!result) throw new Exception("SaveConfigAsync returned false");

        var saved = await db.JetConfigurations
            .Include(c => c.InteriorComponents)
            .FirstOrDefaultAsync();

        if (saved == null) throw new Exception("Config not saved");
        if (saved.InteriorComponents.Count != 1) throw new Exception("Components not saved correctly");
    }

    private async Task SaveConfigAsync_Should_Update_Existing_Config()
    {
        using var db = GetInMemoryDbContext();
        var userId = Guid.NewGuid();
        var repo = new JsonConfigurationRepository(db, userId);

        var configId = Guid.NewGuid();

        db.JetConfigurations.Add(new JetConfigurationDB
        {
            ConfigID = configId,
            UserID = userId,
            Name = "Original Name",
            CabinDimensions = "Original",
            SeatingCapacity = 6,
            Version = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var updatedConfig = new JetConfiguration
        {
            ConfigID = configId,
            UserID = userId,
            Name = "Updated Name",
            CabinDimensions = "Updated Dim",
            SeatingCapacity = 10,
            Version = 2,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            InteriorComponents = new List<InteriorComponent>()
        };

        var result = await repo.SaveConfigAsync(updatedConfig);
        if (!result) throw new Exception("SaveConfigAsync update returned false");

        var retrieved = await db.JetConfigurations.FirstAsync();
        if (retrieved.Name != "Updated Name") throw new Exception("Config name was not updated");
    }

    private async Task LoadAllAsync_Should_Return_User_Configs()
    {
        using var db = GetInMemoryDbContext();
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        db.JetConfigurations.AddRange(
            new JetConfigurationDB
            {
                ConfigID = Guid.NewGuid(),
                UserID = userId,
                Name = "User Jet",
                CabinDimensions = "10x3",
                SeatingCapacity = 6,
                Version = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new JetConfigurationDB
            {
                ConfigID = Guid.NewGuid(),
                UserID = otherUserId,
                Name = "Other User Jet",
                CabinDimensions = "5x2",
                SeatingCapacity = 2,
                Version = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );

        await db.SaveChangesAsync();

        var repo = new JsonConfigurationRepository(db, userId);
        var configs = await repo.LoadAllAsync();

        if (configs.Count != 1) throw new Exception("LoadAllAsync returned incorrect count");
        if (configs[0].Name != "User Jet") throw new Exception("Returned wrong configuration");
    }

    private async Task SaveAllAsync_Should_Remove_Deleted_Configs()
    {
        using var db = GetInMemoryDbContext();
        var userId = Guid.NewGuid();
        var repo = new JsonConfigurationRepository(db, userId);

        var config1 = new JetConfigurationDB
        {
            ConfigID = Guid.NewGuid(),
            UserID = userId,
            Name = "Config1",
            CabinDimensions = "5x3",
            SeatingCapacity = 8,
            Version = 1
        };

        var config2 = new JetConfigurationDB
        {
            ConfigID = Guid.NewGuid(),
            UserID = userId,
            Name = "Config2",
            CabinDimensions = "6x4",
            SeatingCapacity = 10,
            Version = 1
        };

        db.JetConfigurations.AddRange(config1, config2);
        await db.SaveChangesAsync();

        var updatedList = new List<JetConfiguration>
        {
            new JetConfiguration
            {
                ConfigID = config1.ConfigID,
                UserID = userId,
                Name = "Config1",
                CabinDimensions = "4x4",
                SeatingCapacity = 8,
                Version = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                InteriorComponents = new List<InteriorComponent>()
            }
        };

        await repo.SaveAllAsync(updatedList);

        var remaining = await db.JetConfigurations.ToListAsync();
        if (remaining.Count != 1) throw new Exception("Deleted config was not removed");
        if (remaining[0].Name != "Config1") throw new Exception("Wrong config remaining");
    }

    // ----------------------------------------------------
    // xUnit Fact Test Wrappers
    // ----------------------------------------------------

    [Fact] public async Task SaveConfigAsync_Add_Fact() => await SaveConfigAsync_Should_Add_New_Config();
    [Fact] public async Task SaveConfigAsync_Update_Fact() => await SaveConfigAsync_Should_Update_Existing_Config();
    [Fact] public async Task LoadAllAsync_Fact() => await LoadAllAsync_Should_Return_User_Configs();
    [Fact] public async Task SaveAllAsync_Fact() => await SaveAllAsync_Should_Remove_Deleted_Configs();
}
