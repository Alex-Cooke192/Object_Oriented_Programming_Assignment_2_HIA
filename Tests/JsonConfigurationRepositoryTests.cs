using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JetInteriorApp.Data;
using JetInteriorApp.Models;
using JetInteriorApp.Interfaces;

public class JsonConfigurationRepositoryTests
{
    private JetDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<JetDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // new isolated DB each run
            .Options;
        return new JetDbContext(options);
    }

    public async Task RunTestsAsync()
    {
        Console.WriteLine("\nRunning JsonConfigurationRepository tests");

        await SaveConfigAsync_Should_Add_New_Config();
        await SaveConfigAsync_Should_Update_Existing_Config();
        await LoadAllAsync_Should_Return_User_Configs();
        await SaveAllAsync_Should_Remove_Deleted_Configs();

        Console.WriteLine("\nJsonConfigurationRepository: All tests executed successfully.");

    }

    [Fact]
    public async Task SaveConfigAsync_Should_Add_New_Config()
    {
        // Arrange
        using var db = GetInMemoryDbContext();
        var userId = Guid.NewGuid();
        var repo = new JsonConfigurationRepository(db, userId);

        var newConfig = new JetConfiguration
        {
            ConfigID = Guid.NewGuid(),
            Name = "Test Jet Config",
            CabinDimensions = "10x3x2.5m", 
            SeatingCapacity = 8,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            InteriorComponents = new List<InteriorComponent>
            {
                new InteriorComponent
                {
                    ComponentID = Guid.NewGuid(),
                    Name = "Seat A1",
                    Type = "Seat",
                    Tier = "Business",
                    Material = "Leather",
                    Width = 0.5f,
                    Height = 1.0f,
                    Depth = 0.6f,
                    Cost = 1000f,
                    PropertiesJson = "{\"recline\": true}"
                }
            }
        };

        // Act
        var result = await repo.SaveConfigAsync(newConfig);

        // Assert
        Assert.True(result);

        var saved = await db.JetConfigurations
            .Include(c => c.InteriorComponents)
            .FirstOrDefaultAsync();

        Assert.NotNull(saved);
        Assert.Equal(newConfig.Name, saved.Name);
        Assert.Single(saved.InteriorComponents);
    }

    [Fact]
    public async Task SaveConfigAsync_Should_Update_Existing_Config()
    {
        using var db = GetInMemoryDbContext();
        var userId = Guid.NewGuid();
        var repo = new JsonConfigurationRepository(db, userId);

        // Seed existing config
        var configId = Guid.NewGuid();
        var existingDb = new JetConfigurationDB
        {
            ConfigID = configId,
            UserID = userId,
            Name = "Original Name",
            CabinDimensions = "Original Dimensions", 
            SeatingCapacity = 6,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            InteriorComponents = new List<InteriorComponentDB>()
        };
        db.JetConfigurations.Add(existingDb);
        await db.SaveChangesAsync();

        // Modify and update
        var updatedConfig = new JetConfiguration
        {
            ConfigID = configId,
            Name = "Updated Name",
            CabinDimensions = "Updated Dimensions",
            SeatingCapacity = 10,
            CreatedAt = existingDb.CreatedAt,
            UpdatedAt = DateTime.UtcNow,
            InteriorComponents = new List<InteriorComponent>()
        };

        var result = await repo.SaveConfigAsync(updatedConfig);

        // Assert
        Assert.True(result);

        var retrieved = await db.JetConfigurations.FirstAsync();
        Assert.Equal("Updated Name", retrieved.Name);
        Assert.Equal(10, retrieved.SeatingCapacity);
    }

    [Fact]
    public async Task LoadAllAsync_Should_Return_User_Configs()
    {
        using var db = GetInMemoryDbContext();
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        // Seed two configs for different users
        db.JetConfigurations.AddRange(
            new JetConfigurationDB
            {
                ConfigID = Guid.NewGuid(),
                UserID = userId,
                Name = "My Jet 1",
                CabinDimensions = "3x2.5x7m",
                SeatingCapacity = 4, 
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new JetConfigurationDB
            {
                ConfigID = Guid.NewGuid(),
                UserID = otherUserId,
                Name = "Other User Jet",
                CabinDimensions = "1.5x4x8",
                SeatingCapacity = 6, 
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            }
        );
        await db.SaveChangesAsync();

        var repo = new JsonConfigurationRepository(db, userId);

        // Act
        var configs = await repo.LoadAllAsync();

        // Assert
        Assert.Single(configs);
        Assert.Equal("My Jet 1", configs[0].Name);
    }

    [Fact]
    public async Task SaveAllAsync_Should_Remove_Deleted_Configs()
    {
        using var db = GetInMemoryDbContext();
        var userId = Guid.NewGuid();
        var repo = new JsonConfigurationRepository(db, userId);

        // Seed two configs
        var config1 = new JetConfigurationDB
        {
            ConfigID = Guid.NewGuid(),
            UserID = userId,
            Name = "Config 1",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            InteriorComponents = new List<InteriorComponentDB>()
        };
        var config2 = new JetConfigurationDB
        {
            ConfigID = Guid.NewGuid(),
            UserID = userId,
            Name = "Config 2",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            InteriorComponents = new List<InteriorComponentDB>()
        };
        db.JetConfigurations.AddRange(config1, config2);
        await db.SaveChangesAsync();

        // Act — only keep one in memory
        var updatedList = new List<JetConfiguration>
        {
            new JetConfiguration
            {
                ConfigID = config1.ConfigID,
                Name = "Config 1",
                CabinDimensions = "4x4x4m", 
                SeatingCapacity = 8,
                CreatedAt = config1.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                InteriorComponents = new List<InteriorComponent>()
            }
        };

        await repo.SaveAllAsync(updatedList);

        // Assert
        var remaining = await db.JetConfigurations.ToListAsync();
        Assert.Single(remaining);
        Assert.Equal("Config 1", remaining[0].Name);
    }
}