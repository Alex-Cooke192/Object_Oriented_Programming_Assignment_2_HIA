using System.Text.Json;
using JetInteriorApp.Models;
using JetInteriorApp.Interfaces;
using JetInteriorApp.Services.Configuration; 
using Xunit; 
using Moq; 
/*
public class ConfigurationManagerTests
{
    private readonly Mock<IConfigurationRepository> _mockRepo;
    private readonly ConfigurationManager _manager;

    public ConfigurationManagerTests()
    {
        _mockRepo = new Mock<IConfigurationRepository>();

        var layout = new JetLayout
        {
            ConfigID = Guid.NewGuid(),
            Components = new List<LayoutCell>
            {
                new LayoutCell { X = 0, Y = 0, ComponentId = Guid.NewGuid() }
            }
        };

        var configId = Guid.NewGuid();
        var json = JsonSerializer.Serialize(layout);

        var configs = new Dictionary<Guid, JetLayout> { { configId, layout } };

        _mockRepo.Setup(r => r.LoadAllAsync())
                .Returns(Task.FromResult<IDictionary<Guid, JetLayout>>(configs));
        _mockRepo.Setup(r => r.SaveAllAsync(It.IsAny<Dictionary<Guid, JetLayout>>()))
                .ReturnsAsync(true);

        _manager = new ConfigurationManager(_mockRepo.Object);
    }

    [Fact]
    public void GetConfiguration_ReturnsCorrectConfig()
    {
        var config = _manager.GetConfiguration(layout.ConfigID);
        Assert.NotNull(config);
        Assert.Equal(layout.ConfigID, config.ConfigID);
    }

    [Fact]
    public void CreateConfiguration_AddsNewConfig()
    {
        var newLayout = new JetLayout
        {
            ConfigID = Guid.NewGuid(),
            Components = new List<LayoutCell>()
        };

        var created = _manager.CreateConfiguration(newLayout.ConfigID, newLayout);
        Assert.NotNull(created);
        Assert.Equal(newLayout.ConfigID, created.ConfigID);
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
*/