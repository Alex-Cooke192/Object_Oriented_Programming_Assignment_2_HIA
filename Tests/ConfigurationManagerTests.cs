using System.Text.Json;
using JetInteriorApp.Models;
using JetInteriorApp.Interfaces;
using Xunit; 
using Moq; 

public class ConfigurationManagerTests
{
    private readonly Mock<IConfigurationRepository> _mockRepo;
    private readonly ConfigurationManager _manager;

    public ConfigurationManagerTests()
    {
        _mockRepo = new Mock<IConfigurationRepository>();

        var layout = new JetLayout
        {
            LayoutName = "TestLayout",
            UserId = Guid.NewGuid(),
            Components = new List<LayoutCell>
            {
                new LayoutCell { X = 0, Y = 0, ComponentId = Guid.NewGuid() }
            }
        };

        var configId = Guid.NewGuid();
        var json = JsonSerializer.Serialize(layout);
        var configs = new Dictionary<Guid, string> { { configId, json } };

        _mockRepo.Setup(r => r.LoadAll()).Returns(configs);
        _mockRepo.Setup(r => r.SaveAll(It.IsAny<Dictionary<Guid, string>>())).Returns(true);

        _manager = new ConfigurationManager(_mockRepo.Object);
    }

    [Fact]
    public void GetConfiguration_ReturnsCorrectConfig()
    {
        var config = _manager.GetConfiguration(_manager.GetConfigurationsForUser(layout.UserId)[0].ID);
        Assert.NotNull(config);
        Assert.Equal("TestLayout", config.LayoutName);
    }

    [Fact]
    public void CreateConfiguration_AddsNewConfig()
    {
        var newLayout = new JetLayout
        {
            LayoutName = "NewLayout",
            UserId = Guid.NewGuid(),
            Components = new List<LayoutCell>()
        };

        var created = _manager.CreateConfiguration(newLayout.UserId, newLayout.LayoutName, newLayout);
        Assert.NotNull(created);
        Assert.Equal("NewLayout", created.LayoutName);
    }

    [Fact]
    public void CloneConfiguration_CreatesCopyWithModifiedName()
    {
        var original = _manager.GetConfigurationsForUser(layout.UserId)[0];
        var clone = _manager.CloneConfiguration(original.ID);
        Assert.NotNull(clone);
        Assert.Contains("Clone", clone.LayoutName);
    }

    [Fact]
    public void DeleteConfiguration_RemovesConfig()
    {
        var config = _manager.GetConfigurationsForUser(layout.UserId)[0];
        var result = _manager.DeleteConfiguration(config.ID);
        Assert.True(result);
    }

    [Fact]
    public void SaveAllChanges_ReturnsTrue()
    {
        var result = _manager.SaveAllChanges();
        Assert.True(result);
    }

    // Helper property to access layout used in setup
    private JetLayout layout => JsonSerializer.Deserialize<JetLayout>(
        JsonSerializer.Serialize(new JetLayout
        {
            LayoutName = "TestLayout",
            UserId = Guid.NewGuid(),
            Components = new List<LayoutCell>
            {
                new LayoutCell { X = 0, Y = 0, ComponentId = Guid.NewGuid() }
            }
        }));
}