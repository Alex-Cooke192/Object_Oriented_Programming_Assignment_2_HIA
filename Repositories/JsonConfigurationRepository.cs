using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using JetInteriorApp.Models;
using JetInteriorApp.Interfaces; 

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
            if (layout != null && layout.UserId == _currentUserId) // ✅ Filter by user
            {
                layouts.Add(config.ID, layout);
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
    public async Task SaveAllAsync(Dictionary<Guid, JetLayout> configs)
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
