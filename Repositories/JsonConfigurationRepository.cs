using System.Text.Json;
using Microsoft.EntityFrameworkCore;

public class JsonConfigurationRepository
{
    private readonly JetDbContext _db;
    private readonly int _currentUserId;
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
    public List<JetLayout> LoadAll()
    {
        var configs = _db.JetConfigs
            .Where(c => c.UserId == _currentUserId)
            .AsNoTracking()
            .ToList();

        var layouts = new List<JetLayout>();

        foreach (var config in configs)
        {
            if (!string.IsNullOrWhiteSpace(config.ConfigJson))
            {
                var layout = JsonSerializer.Deserialize<JetLayout>(config.ConfigJson, _options);
                if (layout != null)
                    layouts.Add(layout);
            }
        }

        return layouts;
    }

    // Save all layouts for the current user, overwriting existing configs
    public void SaveAll(List<JetLayout> layouts)
    {
        var configs = _db.JetConfigs
            .Where(c => c.UserId == _currentUserId)
            .ToList();

        for (int i = 0; i < configs.Count && i < layouts.Count; i++)
        {
            configs[i].ConfigJson = JsonSerializer.Serialize(layouts[i], _options);
            configs[i].UpdatedAt = DateTime.UtcNow;
        }

        _db.SaveChanges();
    }
}