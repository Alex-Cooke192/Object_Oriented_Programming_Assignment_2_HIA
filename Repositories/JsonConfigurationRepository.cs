using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using JetInteriorApp.Models;
using JetInteriorApp.Interfaces;
using System.ComponentModel;

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
        // Your implementation here
    }
    public async Task<bool> SaveEditedComponentsAsync(Guid configId, string configJson)
    {
        // 1. Load JetConfig and hydrate components
        var config = await _db.JetConfigs
            .Include(c => c.InteriorComponents)
            .FirstOrDefaultAsync(c => c.Id == configId);

        if (config == null) return false;

        // 2. Deserialize layout map to get component IDs
        var layoutMap = JsonSerializer.Deserialize<List<LayoutCell>>(configJson, _options);
        if (layoutMap == null || !layoutMap.Any()) return false;

        var componentIds = layoutMap.Select(c => c.ComponentId).Distinct().ToList();

        // 3. Query matching components from DB
        var dbComponents = await _db.InteriorComponents
            .Where(c => componentIds.Contains(c.ComponentId))
            .ToListAsync();

        // 4. Update each DB component using edited version from JetConfig
        foreach (var dbComponent in dbComponents)
        {
            // Get co-ordinates of component
            var layoutCell = layoutMap.FirstOrDefault(c => c.ComponentId == dbComponent.ComponentId);
            if (layoutCell == null) continue;

            // Pull edited component from current JetConfig
            var editedComponent = config.InteriorComponents
                .FirstOrDefault(c => c.ComponentId == dbComponent.ComponentId);

            if (editedComponent == null) continue;

            dbComponent.X = layoutCell.X;
            dbComponent.Y = layoutCell.Y;

            switch (dbComponent)
            {
                case SeatComponentDB dbSeat when editedComponent is SeatComponentDB editedSeat:
                    dbSeat.Recline = editedSeat.Recline;
                    dbSeat.Massage = editedSeat.Massage;
                    dbSeat.Accessibility = editedSeat.Accessibility;
                    dbSeat.Lighting = editedSeat.Lighting;
                    break;

                case KitchenComponentDB dbKitchen when editedComponent is KitchenComponentDB editedKitchen:
                    dbKitchen.ApplianceList = editedKitchen.ApplianceList;
                    dbKitchen.Refrigeration = editedKitchen.Refrigeration;
                    dbKitchen.FireSuppression = editedKitchen.FireSuppression;
                    break;

                case LightingComponentDB dbLight when editedComponent is LightingComponentDB editedLight:
                    dbLight.BrightnessLevel = editedLight.BrightnessLevel;
                    dbLight.ColorTemperature = editedLight.ColorTemperature;
                    dbLight.Dimmable = editedLight.Dimmable;
                    break;

                case TableComponentDB dbTable when editedComponent is TableComponentDB editedTable:
                    dbTable.SurfaceMaterial = editedTable.SurfaceMaterial;
                    dbTable.Foldable = editedTable.Foldable;
                    dbTable.SeatCount = editedTable.SeatCount;
                    break;

                case ScreenComponentDB dbScreen when editedComponent is ScreenComponentDB editedScreen:
                    dbScreen.ContentFilters = editedScreen.ContentFilters;
                    dbScreen.Resolution = editedScreen.Resolution;
                    dbScreen.TouchEnabled = editedScreen.TouchEnabled;
                    break;

                case StorageCabinetComponentDB dbCabinet when editedComponent is StorageCabinetComponentDB editedCabinet:
                    dbCabinet.CapacityLitres = editedCabinet.CapacityLitres;
                    dbCabinet.Lockable = editedCabinet.Lockable;
                    dbCabinet.ShelfCount = editedCabinet.ShelfCount;
                    break;

                case EmergencyExitComponentDB dbExit when editedComponent is EmergencyExitComponentDB editedExit:
                    dbExit.ClearanceRadius = editedExit.ClearanceRadius;
                    dbExit.SignageType = editedExit.SignageType;
                    dbExit.AccessibilityFeatures = editedExit.AccessibilityFeatures;
                    break;
            }
        }

        // 5. Save changes
        await _db.SaveChangesAsync();
        return true;
    }

}
