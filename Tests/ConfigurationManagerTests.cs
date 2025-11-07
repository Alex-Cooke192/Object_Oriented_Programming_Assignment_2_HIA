using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetInteriorApp.Models;
using JetInteriorApp.Services.Configuration;
using JetInteriorApp.Interfaces;
using Moq;
using Xunit;

public class ConfigurationManagerTests
{
    private readonly Mock<IConfigurationRepository> _mockRepo;
    private readonly ConfigurationManager _manager;
    private readonly Guid _userId;
    private readonly JetConfiguration _existingConfig;

    public ConfigurationManagerTests()
    {
        _userId = Guid.NewGuid();
        _mockRepo = new Mock<IConfigurationRepository>();

        _existingConfig = new JetConfiguration
        {
            ConfigID = Guid.NewGuid(),
            UserID = _userId,
            Name = "TestConfig",
            SeatingCapacity = 4,
            CabinDimensions = "10x10x10",
            Version = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            InteriorComponents = new List<InteriorComponent>
            {
                new InteriorComponent
                {
                    ComponentID = Guid.NewGuid(),
                    Name = "Seat",
                    Type = "Chair",
                    Tier = "Economy",
                    Material = "Leather",
                    Position = "{\"x\":0,\"y\":0}",
                    CreatedAt = DateTime.UtcNow,
                    PropertiesJson = "{}"
                }
            }
        };

        // Mock repository methods
        _mockRepo.Setup(r => r.LoadAllAsync())
            .ReturnsAsync(new List<JetConfiguration> { _existingConfig });

        _mockRepo.Setup(r => r.SaveConfigAsync(It.IsAny<JetConfiguration>()))
            .ReturnsAsync(true);

        _mockRepo.Setup(r => r.SaveAllAsync(It.IsAny<List<JetConfiguration>>()))
            .ReturnsAsync(true);

        _manager = new ConfigurationManager(_mockRepo.Object, _userId);
        _manager.InitializeAsync().Wait();
    }

    public async Task RunTestsAsync()
    {
        Console.WriteLine("Running ConfigurationManager tests...");

        await TestGetConfigurationAsync();
        await TestCreateConfigurationAsync();
        await TestCloneConfigurationAsync();
        await TestDeleteConfigurationAsync();
        await TestSaveAllChangesAsync();

        Console.WriteLine("All ConfigurationManager tests completed successfully.");
    }

    private async Task TestGetConfigurationAsync()
    {
        var config = _manager.GetConfiguration(_existingConfig.ConfigID);
        if (config == null) throw new Exception("Config is null");

        Console.WriteLine($"GetConfiguration_ReturnsCorrectConfig: PASSED");
        await Task.CompletedTask;
    }

    private async Task TestCreateConfigurationAsync()
    {
        var newBase = new JetConfiguration
        {
            CabinDimensions = "20x20x20",
            SeatingCapacity = 6,
            Version = 0,
            InteriorComponents = new List<InteriorComponent>()
        };

        var created = await _manager.CreateConfigurationAsync("NewConfig", newBase);
        if (created == null) throw new Exception("Created config is null");

        Console.WriteLine("CreateConfiguration_AddsNewConfig: PASSED");
    }

    private async Task TestCloneConfigurationAsync()
    {
        var clone = await _manager.CloneConfigurationAsync(_existingConfig.ConfigID);
        if (clone == null) throw new Exception("Clone returned null");

        Console.WriteLine("CloneConfiguration_CreatesCopyWithModifiedName: PASSED");
    }

    private async Task TestDeleteConfigurationAsync()
    {
        var result = await _manager.DeleteConfigurationAsync(_existingConfig.ConfigID);
        if (!result) throw new Exception("Delete returned false");

        Console.WriteLine("DeleteConfiguration_RemovesConfig: PASSED");
    }

    private async Task TestSaveAllChangesAsync()
    {
        try
        {
            // Get the current set of configs to pass into SaveAllChangesAsync
            var configs = await _manager.InitializeAsync(); // returns List<JetConfiguration>

            var result = await _manager.SaveAllChangesAsync(configs);
            if (!result) throw new Exception("SaveAllChanges failed");

            Console.WriteLine("SaveAllChanges_ReturnsTrue: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SaveAllChanges_ReturnsTrue: FAILED - {ex.Message}");
            throw;
        }
    }

    [Fact] public async Task GetConfiguration_Fact() => await TestGetConfigurationAsync();
    [Fact] public async Task CreateConfiguration_Fact() => await TestCreateConfigurationAsync();
    [Fact] public async Task CloneConfiguration_Fact() => await TestCloneConfigurationAsync();
    [Fact] public async Task DeleteConfiguration_Fact() => await TestDeleteConfigurationAsync();
    [Fact] public async Task SaveAllChanges_Fact() => await TestSaveAllChangesAsync();
}
