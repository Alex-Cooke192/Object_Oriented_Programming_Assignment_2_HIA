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
                    // Deserialize type-specific settings into a dynamic object
                    Settings = string.IsNullOrWhiteSpace(ic.ComponentSettings?.SettingsJson)
                        ? null
                        : JsonSerializer.Deserialize<JsonElement>(ic.ComponentSettings.SettingsJson, _options)
                }).ToList()
            };

            layouts[config.ConfigId] = layout;
        }

        return layouts;
    }

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
                ComponentSettings = new ComponentSettings
                {
                    ComponentId = c.ComponentId,
                    SettingsJson = c.Settings?.ToString() ?? "{}"
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
                    ComponentSettings = new ComponentSettings
                    {
                        ComponentId = c.ComponentId,
                        SettingsJson = c.Settings?.ToString() ?? "{}"
                    }
                }).ToList()
            };

            await _db.JetConfigurations.AddAsync(newConfig);
        }

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SaveAllAsync(Dictionary<Guid, JetLayout> configs)
    {
        foreach (var (id, json) in configs)
            await SaveConfigAsync(id, JsonSerializer.Serialize(json, _options));
        return true;
    }
}