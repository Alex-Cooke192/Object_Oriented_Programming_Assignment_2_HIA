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
    public async Task<bool> SaveConfigAsync(Guid configId, string configJson)
{
    // 1. Find the existing config
    var config = await _db.JetConfigs
        .Include(c => c.InteriorComponents)
        .FirstOrDefaultAsync(c => c.Id == configId);

    if (config == null) return false;

    // 2. Update config metadata
    config.UpdatedAt = DateTime.UtcNow;
    config.ConfigJson = configJson;

    // 3. Deserialize layout map from configJson
    var layoutMap = JsonSerializer.Deserialize<List<LayoutCell>>(configJson, _options);
        if (layoutMap == null || !layoutMap.Any()) return false;

        // 3. Query all matching components and include their subtype properties
        var components = await _db.InteriorComponents
            .Where(c => componentIds.Contains(c.ComponentId))
            .Include(c => c.KitchenProperties)
            .Include(c => c.SeatProperties)
            .Include(c => c.LightingProperties)
            .Include(c => c.TableProperties)
            .Include(c => c.ScreenProperties)
            .Include(c => c.StorageCabinetProperties)
            .Include(c => c.EmergencyExitProperties)
            .ToListAsync();
        
        foreach (var entry in layoutMap)
        {
            // 4. Find the matching component
            var component = await _db.InteriorComponents
                .FirstOrDefaultAsync(c => c.ComponentId == entry.ComponentId);

            if (component == null) continue;

            // 5. Update position
            component.X = entry.X;
            component.Y = entry.Y;
            component.UpdatedAt = DateTime.UtcNow;

            // 6. Route to correct property table based on component.Type
            switch (component.Type)
            {
                case "Kitchen":
                    var kitchen = await _db.KitchenProperties.FirstOrDefaultAsync(p => p.ComponentId == component.Id);
                    if (kitchen != null)
                    {
                        kitchen.ApplianceList = NewComponent.ApplianceList;
                        kitchen.Refrigeration = NewComponent.Refrigeration;
                        kitchen.FireSuppression = NewComponent.FireSuppression;
                    }
                    break;

                case "Seat":
                    var chair = await _db.SeatProperties.FirstOrDefaultAsync(p => p.ComponentId == component.Id);
                    if (chair != null)
                    {
                        chair.IsAccessible = NewComponent.IsAccessible;
                        chair.Recline = NewComponent.Recline;
                        chair.Lighting = NewComponent.lighting;
                        chair.Massage = NewComponent.Massage;
                        chair.Accessibility = NewComponent.Accessibility;
                    }
                    break;

                case "Lighting":
                    var lighting = await _db.LightingProperties.FirstOrDefaultAsync(p => p.ComponentId == component.Id);
                    if (lighting != null)
                    {
                        lighting.BrightnessLevel = NewComponent.BrightnessLevel;
                        lighting.ColorTemperature = NewComponent.ColorTemperature;
                        lighting.Dimmable = NewComponent.Dimmable;
                    }
                    break;

                case "Table":
                    var table = await _db.TableProperties.FirstOrDefaultAsync(p => p.ComponentId == component.Id);
                    if (table != null)
                    {
                        table.SurfaceMaterial = NewComponent.SurfaceMaterial;
                        table.Foldable = NewComponent.Foldable;
                        table.SeatCount = NewComponent.SeatCount;
                    }
                    break;

                case "Screen":
                    var screen = await _db.ScreenProperties.FirstOrDefaultAsync(p => p.ComponentId == component.Id);
                    if (screen != null)
                    {
                        screen.ContentFilters = NewComponent.ContentFilters;
                        screen.Resolution = NewComponent.Resolution;
                        screen.TouchEnabled = NewComponent.TouchEnabled;
                    }
                    break;

                case "StorageCabinet":
                    var cabinet = await _db.StorageCabinetProperties.FirstOrDefaultAsync(p => p.ComponentId == component.Id);
                    if (cabinet != null)
                    {
                        cabinet.CapacityLitres = NewComponent.CapacityLitres;
                        cabinet.Lockable = NewComponent.Lockable;
                        cabinet.ShelfCount = NewComponent.ShelfCount;
                    }
                    break;

                case "EmergencyExit":
                    var exit = await _db.EmergencyExitProperties.FirstOrDefaultAsync(p => p.ComponentId == component.Id);
                    if (exit != null)
                    {
                        exit.ClearanceRadius = NewComponent.ClearanceRadius;
                        exit.SignageType = NewComponent.SignageType;
                        exit.AccessibilityFeatures = NewComponent.AccessibilityFeatures;
                    }
                    break;
            }
        }

    await _db.SaveChangesAsync();
    return true;
}


}
