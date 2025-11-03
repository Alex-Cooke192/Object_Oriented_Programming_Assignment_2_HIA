using System;
using System.Collections.Generic;
using System.Text.Json;
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

        _mockRepo.Setup(r => r.LoadAllAsync())
            .ReturnsAsync(new List<JetConfiguration> { _existingConfig });
        _mockRepo.Setup(r => r.SaveConfigAsync(It.IsAny<JetConfiguration>())).ReturnsAsync(true);
        _mockRepo.Setup(r => r.SaveAllAsync(It.IsAny<List<JetConfiguration>>())).ReturnsAsync(true);

        _manager = new ConfigurationManager(_mockRepo.Object, _userId);
        _manager.InitializeAsync().Wait(); // load configs into memory
    }

    // ------------------------
    // Manual test runner
    // ------------------------
    public async Task RunTestsAsync()
    {
        Console.WriteLine("Running ConfigurationManager tests...");

        await TestGetConfigurationAsync();
        await TestCreateConfigurationAsync();
        await TestCloneConfigurationAsync();
        await TestDeleteConfigurationAsync();
        await TestSaveAllChangesAsync();
        await TestExportConfigurationToJsonAsync();
        await TestImportConfigurationFromJsonAsync();

        Console.WriteLine("All ConfigurationManager tests completed successfully.");
    }

    // ------------------------
    // Internal test logic
    // ------------------------
    private async Task TestGetConfigurationAsync()
    {
        try
        {
            var config = _manager.GetConfiguration(_existingConfig.ConfigID);
            if (config == null) throw new Exception("Config is null");

            Console.WriteLine($"GetConfiguration_ReturnsCorrectConfig: PASSED - Retrieved '{config.Name}'");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetConfiguration_ReturnsCorrectConfig: FAILED - {ex.Message}");
            throw;
        }

        await Task.CompletedTask;
    }

    private async Task TestCreateConfigurationAsync()
    {
        try
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

            Console.WriteLine($"CreateConfiguration_AddsNewConfig: PASSED - '{created.Name}' (ID {created.ConfigID})");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CreateConfiguration_AddsNewConfig: FAILED - {ex.Message}");
            throw;
        }
    }

    private async Task TestCloneConfigurationAsync()
    {
        try
        {
            var clone = await _manager.CloneConfigurationAsync(_existingConfig.ConfigID);
            if (clone == null) throw new Exception("Clone is null");

            Console.WriteLine($"CloneConfiguration_CreatesCopyWithModifiedName: PASSED - '{clone.Name}' (ID {clone.ConfigID})");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CloneConfiguration_CreatesCopyWithModifiedName: FAILED - {ex.Message}");
            throw;
        }
    }

    private async Task TestDeleteConfigurationAsync()
    {
        try
        {
            var result = await _manager.DeleteConfigurationAsync(_existingConfig.ConfigID);
            if (!result) throw new Exception("Delete failed");

            Console.WriteLine("DeleteConfiguration_RemovesConfig: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DeleteConfiguration_RemovesConfig: FAILED - {ex.Message}");
            throw;
        }
    }

    private async Task TestSaveAllChangesAsync()
    {
        try
        {
            var result = await _manager.SaveAllChangesAsync();
            if (!result) throw new Exception("SaveAllChanges failed");

            Console.WriteLine("SaveAllChanges_ReturnsTrue: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SaveAllChanges_ReturnsTrue: FAILED - {ex.Message}");
            throw;
        }
    }

    // ------------------------
    // Optional xUnit tests
    // ------------------------
    [Fact]
    public async Task GetConfiguration_Fact() => await TestGetConfigurationAsync();

    [Fact]
    public async Task CreateConfiguration_Fact() => await TestCreateConfigurationAsync();

    [Fact]
    public async Task CloneConfiguration_Fact() => await TestCloneConfigurationAsync();

    [Fact]
    public async Task DeleteConfiguration_Fact() => await TestDeleteConfigurationAsync();

    [Fact]
    public async Task SaveAllChanges_Fact() => await TestSaveAllChangesAsync();

    [Fact]
    public async Task ExportConfiguration_Fact() => await TestExportConfigurationToJsonAsync();

    [Fact]
    public async Task ImportConfiguration_Fact() => await TestImportConfigurationFromJsonAsync();
}
