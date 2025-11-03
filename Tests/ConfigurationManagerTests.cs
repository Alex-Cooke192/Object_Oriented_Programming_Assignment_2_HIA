using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using JetInteriorApp.Models;
using JetInteriorApp.Interfaces;
using JetInteriorApp.Services.Configuration;
using Moq;
using Xunit;

public class ConfigurationManagerTests
{
    private readonly Mock<JsonConfigurationRepository> _mockRepo;
    private readonly ConfigurationManager _manager;
    private readonly Guid _userId;
    private readonly JetConfiguration _existingConfig;

    public ConfigurationManagerTests()
    {
        _userId = Guid.NewGuid();
        _mockRepo = new Mock<JsonConfigurationRepository>();

        // Setup an existing configuration
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
                    Position = "FrontLeft",
                    CreatedAt = DateTime.UtcNow,
                    PropertiesJson = "{}"
                }
            }
        };

        // Mock LoadAllAsync to return the existing config
        _mockRepo.Setup(r => r.LoadAllAsync())
            .ReturnsAsync(new List<JetConfiguration> { _existingConfig });

        // Mock SaveConfigAsync to always return true
        _mockRepo.Setup(r => r.SaveConfigAsync(It.IsAny<JetConfiguration>()))
            .ReturnsAsync(true);

        // Mock SaveAllAsync to always return true
        _mockRepo.Setup(r => r.SaveAllAsync(It.IsAny<List<JetConfiguration>>()))
            .ReturnsAsync(true);

        _manager = new ConfigurationManager(_mockRepo.Object, _userId);
        // Initialize the manager so it loads configs into memory
        _manager.GetConfigurationsForUserAsync().Wait();
    }

    [Fact]
    public void GetConfiguration_ReturnsCorrectConfig()
    {
        var config = _manager.GetConfiguration(_existingConfig.ConfigID);
        Assert.NotNull(config);
        Assert.Equal(_existingConfig.ConfigID, config.ConfigID);
        Assert.Equal(_existingConfig.Name, config.Name);
    }

    [Fact]
    public async Task CreateConfiguration_AddsNewConfig()
    {
        var newBase = new JetConfiguration
        {
            CabinDimensions = "20x20x20",
            SeatingCapacity = 6,
            Version = 0,
            InteriorComponents = new List<InteriorComponent>()
        };

        var created = await _manager.CreateConfigurationAsync("NewConfig", newBase);
        Assert.NotNull(created);
        Assert.Equal("NewConfig", created.Name);
        Assert.Equal(_userId, created.UserID);
        Assert.True(created.ConfigID != Guid.Empty);
    }

    [Fact]
    public async Task CloneConfiguration_CreatesCopyWithModifiedName()
    {
        var clone = await _manager.CloneConfigurationAsync(_existingConfig.ConfigID);
        Assert.NotNull(clone);
        Assert.Contains("Clone", clone.Name);
        Assert.Equal(_existingConfig.SeatingCapacity, clone.SeatingCapacity);
        Assert.Equal(_existingConfig.InteriorComponents.Count, clone.InteriorComponents.Count);
        Assert.NotEqual(_existingConfig.ConfigID, clone.ConfigID); // Ensure it's a new ID
    }

    [Fact]
    public async Task DeleteConfiguration_RemovesConfig()
    {
        var result = await _manager.DeleteConfigurationAsync(_existingConfig.ConfigID);
        Assert.True(result);
        var deleted = _manager.GetConfiguration(_existingConfig.ConfigID);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task SaveAllChanges_ReturnsTrue()
    {
        var result = await _manager.SaveAllChangesAsync();
        Assert.True(result);
    }

    [Fact]
    public void ExportConfigurationToJson_ReturnsValidJson()
    {
        var json = _manager.ExportConfigurationToJson(_existingConfig.ConfigID);
        Assert.False(string.IsNullOrEmpty(json));
        var deserialized = JsonSerializer.Deserialize<JetConfiguration>(json);
        Assert.NotNull(deserialized);
        Assert.Equal(_existingConfig.ConfigID, deserialized.ConfigID);
    }

    [Fact]
    public async Task ImportConfigurationFromJsonAsync_AddsConfigToMemory()
    {
        var json = JsonSerializer.Serialize(_existingConfig);
        var imported = await _manager.ImportConfigurationFromJsonAsync(json);

        Assert.NotNull(imported);
        Assert.Equal(_userId, imported.UserID);
        Assert.NotEqual(_existingConfig.ConfigID, imported.ConfigID); // New ID for imported
        var inMemory = _manager.GetConfiguration(imported.ConfigID);
        Assert.Equal(imported.ConfigID, inMemory.ConfigID);
    }
}
