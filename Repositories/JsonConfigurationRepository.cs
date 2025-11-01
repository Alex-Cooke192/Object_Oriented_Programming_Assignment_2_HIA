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

    public JsonConfigurationRepository(JetDbContext db, Guid currentUserId)
    {
        _db = db;
        _currentUserId = currentUserId;
    }

    /// <summary>
    /// Loads all configurations for the current user.
    /// </summary>
    public async Task<IDictionary<Guid, JetLayout>> LoadAllAsync()
    {
        var configs = await _db.JetConfigurations
            .Where(c => c.UserId == _currentUserId)
            .AsNoTracking()
            .Include(c => c.InteriorComponents)
                .ThenInclude(ic => ic.ComponentSettings)
            .ToListAsync();

        var layouts = new Dictionary<Guid, JetLayout>();

        foreach (var config in configs)
        {
            var layout = new JetLayout
            {
                ConfigID = config.ConfigId,
                Name = config.ModelName,
                SeatingCapacity = config.SeatingCapacity,
                Components = config.InteriorComponents.Select(ic => new LayoutComponent
                {
                    ComponentId = ic.ComponentId,
                    Name = ic.Name,
                    Type = ic.Type,
                    Tier = ic.Tier,
                    Material = ic.Material,
                    Color = ic.Color,
                    Position = JsonSerializer.Deserialize<Position>(ic.Position ?? "{}", _options),
                    Width = ic.Width,
                    Height = ic.Height,
                    Depth = ic.Depth,
                    Cost = ic.Cost,
                    Settings = ic.ComponentSettings == null ? null : new ComponentSettingsDTO
                    {
                        WifiAccess = ic.ComponentSettings.WifiAccess,
                        ScreenAccess = ic.ComponentSettings.ScreenAccess,
                        AccessibilitySettings = ic.ComponentSettings.AccessibilitySettings
                    }
                }).ToList()
            };

            layouts[config.ConfigId] = layout;
        }

        return layouts;
    }

    /// <summary>
    /// Saves all JetLayouts for the current user.
    /// </summary>
    public async Task<bool> SaveAllAsync(Dictionary<Guid, JetLayout> configs)
    {
        foreach (var (configId, layout) in configs)
        {
            var existingConfig = await _db.JetConfigurations
                .Include(c => c.InteriorComponents)
                    .ThenInclude(ic => ic.ComponentSettings)
                .FirstOrDefaultAsync(c => c.ConfigId == configId && c.UserId == _currentUserId);

            if (existingConfig != null)
            {
                existingConfig.ModelName = layout.Name;
                existingConfig.SeatingCapacity = layout.SeatingCapacity;
                existingConfig.UpdatedAt = DateTime.UtcNow;

                // Clear old components
                _db.InteriorComponents.RemoveRange(existingConfig.InteriorComponents);

                // Add new components
                existingConfig.InteriorComponents = layout.Components.Select(c => new InteriorComponent
                {
                    ComponentId = c.ComponentId,
                    ConfigId = configId,
                    Name = c.Name,
                    Type = c.Type,
                    Tier = c.Tier,
                    Material = c.Material,
                    Color = c.Color,
                    Position = JsonSerializer.Serialize(c.Position, _options),
                    Width = c.Width,
                    Height = c.Height,
                    Depth = c.Depth,
                    Cost = c.Cost,
                    CreatedAt = DateTime.UtcNow,
                    ComponentSettings = c.Settings == null ? null : new ComponentSettings
                    {
                        ComponentId = c.ComponentId,
                        WifiAccess = c.Settings.WifiAccess,
                        ScreenAccess = c.Settings.ScreenAccess,
                        AccessibilitySettings = c.Settings.AccessibilitySettings
                    }
                }).ToList();
            }
            else
            {
                var newConfig = new JetConfiguration
                {
                    ConfigId = configId,
                    UserId = _currentUserId,
                    ModelName = layout.Name,
                    SeatingCapacity = layout.SeatingCapacity,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    InteriorComponents = layout.Components.Select(c => new InteriorComponent
                    {
                        ComponentId = c.ComponentId,
                        ConfigId = configId,
                        Name = c.Name,
                        Type = c.Type,
                        Tier = c.Tier,
                        Material = c.Material,
                        Color = c.Color,
                        Position = JsonSerializer.Serialize(c.Position, _options),
                        Width = c.Width,
                        Height = c.Height,
                        Depth = c.Depth,
                        Cost = c.Cost,
                        CreatedAt = DateTime.UtcNow,
                        ComponentSettings = c.Settings == null ? null : new ComponentSettings
                        {
                            ComponentId = c.ComponentId,
                            WifiAccess = c.Settings.WifiAccess,
                            ScreenAccess = c.Settings.ScreenAccess,
                            AccessibilitySettings = c.Settings.AccessibilitySettings
                        }
                    }).ToList()
                };

                await _db.JetConfigurations.AddAsync(newConfig);
            }
        }

        await _db.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Saves a single JetLayout config from JSON string.
    /// </summary>
    public async Task<bool> SaveConfigAsync(Guid configId, string jetConfigJson)
    {
        var layout = JsonSerializer.Deserialize<JetLayout>(jetConfigJson, _options);
        if (layout == null) return false;

        var existing = await _db.JetConfigurations
            .Include(c => c.InteriorComponents)
                .ThenInclude(ic => ic.ComponentSettings)
            .FirstOrDefaultAsync(c => c.ConfigId == configId && c.UserId == _currentUserId);

        if (existing != null)
        {
            existing.ModelName = layout.Name;
            existing.SeatingCapacity = layout.SeatingCapacity;
            existing.UpdatedAt = DateTime.UtcNow;
            _db.InteriorComponents.RemoveRange(existing.InteriorComponents);

            existing.InteriorComponents = layout.Components.Select(c => new InteriorComponent
            {
                ComponentId = c.ComponentId,
                ConfigId = configId,
                Name = c.Name,
                Type = c.Type,
                Tier = c.Tier,
                Material = c.Material,
                Color = c.Color,
                Position = JsonSerializer.Serialize(c.Position, _options),
                Width = c.Width,
                Height = c.Height,
                Depth = c.Depth,
                Cost = c.Cost,
                CreatedAt = DateTime.UtcNow,
                ComponentSettings = c.Settings == null ? null : new ComponentSettings
                {
                    ComponentId = c.ComponentId,
                    WifiAccess = c.Settings.WifiAccess,
                    ScreenAccess = c.Settings.ScreenAccess,
                    AccessibilitySettings = c.Settings.AccessibilitySettings
                }
            }).ToList();
        }
        else
        {
            var newConfig = new JetConfiguration
            {
                ConfigId = configId,
                UserId = _currentUserId,
                ModelName = layout.Name,
                SeatingCapacity = layout.SeatingCapacity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                InteriorComponents = layout.Components.Select(c => new InteriorComponent
                {
                    ComponentId = c.ComponentId,
                    ConfigId = configId,
                    Name = c.Name,
                    Type = c.Type,
                    Tier = c.Tier,
                    Material = c.Material,
                    Color = c.Color,
                    Position = JsonSerializer.Serialize(c.Position, _options),
                    Width = c.Width,
                    Height = c.Height,
                    Depth = c.Depth,
                    Cost = c.Cost,
                    CreatedAt = DateTime.UtcNow,
                    ComponentSettings = c.Settings == null ? null : new ComponentSettings
                    {
                        ComponentId = c.ComponentId,
                        WifiAccess = c.Settings.WifiAccess,
                        ScreenAccess = c.Settings.ScreenAccess,
                        AccessibilitySettings = c.Settings.AccessibilitySettings
                    }
                }).ToList()
            };

            await _db.JetConfigurations.AddAsync(newConfig);
        }

        await _db.SaveChangesAsync();
        return true;
    }
}