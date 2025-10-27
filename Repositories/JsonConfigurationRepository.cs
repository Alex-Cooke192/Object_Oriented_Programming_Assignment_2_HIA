using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using JetInteriorApp.Models;

public class JsonConfigurationRepository : IConfigurationRepository
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
    public async Task<IDictionary<string, JetLayout>> LoadAllAsync()
    {
        var configs = await _db.JetConfigs
            .Where(c => c.UserId == _currentUserId)
            .AsNoTracking()
            .ToListAsync();

        var layouts = new Dictionary<string, JetLayout>();

        foreach (var config in configs)
        {
            if (!string.IsNullOrWhiteSpace(config.ConfigJson))
            {
                var layout = JsonSerializer.Deserialize<JetLayout>(config.ConfigJson, _options);
                if (layout != null)
                    layouts.Add(config.Name, layout); // ✅ Use a valid key
            }
        }

        return layouts;
    }
    // Save all layouts for the current user, overwriting existing configs
    public async Task SaveAllAsync(Dictionary<string, JetLayout> configs)
    {
        foreach (var layout in configs)
        {
            var json = JsonSerializer.Serialize(layout);
            var config = new JetConfigDB
            {
                UserId = _currentUserId,
                ConfigJson = json,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.JetConfigs.Add(config);
        }

        await _db.SaveChangesAsync();
    }
    public async Task SaveConfigAsync(JetLayout config)
    {
        // Intentionally does nothing
        await Task.CompletedTask;
    }
}
