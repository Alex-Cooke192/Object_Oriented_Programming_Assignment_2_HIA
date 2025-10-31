using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using JetInteriorApp.Models;
using JetInteriorApp.Interfaces; 

public class JsonConfigurationRepository : IConfigurationRepository
{
    private readonly JetDbContext _db;
    private readonly Guid _currentUserId;
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public JsonConfigurationRepository(JetDbContext db, int currentUserId)
    {
        _db = db;
        _currentUserId = currentUserId;
    }

    // Load all configs for the current user and deserialize them
    public async Task<IDictionary<Guid, JetLayout>> LoadAllAsync()
{
    var configs = await _db.JetConfigs
        .Where(c => c.UserId == _currentUserId)
        .AsNoTracking()
        .ToListAsync();

    var layouts = new Dictionary<Guid, JetLayout>();

    foreach (var config in configs)
    {
        if (!string.IsNullOrWhiteSpace(config.ConfigJson))
        {
            var layout = JsonSerializer.Deserialize<JetLayout>(config.ConfigJson, _options);
            if (layout != null) 
            {
                layouts.Add(layout.ConfigID, layout);
            }
        }
    }

    // Extract components from layouts belonging to the current user
    var userComponents = layouts
        .SelectMany(kvp => kvp.Value.Components.Select(c => new
        {
            LayoutId = kvp.Key,
            ComponentId = c.ComponentId,
            Coordinate = (c.X, c.Y)
        }))
        .ToList();

    Console.WriteLine($"🔍 Found {userComponents.Count} components for user {_currentUserId}.");

    return layouts;
}
    // Save all layouts for the current user, overwriting existing configs
    public async Task<bool> SaveAllAsync(Dictionary<Guid, JetLayout> configs)
{
    foreach (var layoutEntry in configs)
    {
        var layout = layoutEntry.Value;
        var configId = layoutEntry.Key;

        var config = new JetConfigDB
        {
            Id = configId,
            UserId = _currentUserId,
            Name = layout.Name,
            Version = layout.Version,
            ValidationStatus = layout.ValidationStatus,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ConfigJson = JsonSerializer.Serialize(layout, _options),
            InteriorComponents = layout.Components.Select(c => new InteriorComponentDB
            {
                Id = Guid.NewGuid(),
                JetConfigId = configId,
                Type = c.Type,
                X = c.X,
                Y = c.Y,
                Width = c.Width,
                Height = c.Height,

                // Subtype navigation will be added below
                KitchenProperties = c.Type == "Kitchen" ? new KitchenPropertiesDB
                {
                    ApplianceType = c.Kitchen.ApplianceType,
                    HasIsland = c.Kitchen.HasIsland
                } : null,

                ChairProperties = c.Type == "Chair" ? new ChairPropertiesDB
                {
                    Material = c.Chair.Material,
                    HasArmrest = c.Chair.HasArmrest
                } : null

                // Add other subtypes here as needed
            }).ToList()
        };

        _db.JetConfigs.Add(config);
    }

    await _db.SaveChangesAsync();
    return true;
}
    public async Task SaveConfigAsync(JetLayout config)
    {
        // Intentionally does nothing
        await Task.CompletedTask;
    }
}
