using JetInteriorApp.Models;
using JetInteriorApp.Interfaces;
using JetInteriorApp.Data; 
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

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

    // Matches interface exactly
    public async Task<List<JetConfiguration>> LoadAllAsync()
    {
        return await _db.JetConfigurations
            .Where(c => c.UserID == _currentUserId)
            .Include(c => c.Components)
            .ToListAsync();
    }

    // Matches interface exactly
    public async Task<bool> SaveConfigAsync(JetConfiguration config)
    {
        if (config == null) return false;

        var existing = await _db.JetConfigurations
            .Include(c => c.Components)
            .FirstOrDefaultAsync(c => c.ConfigID == config.ConfigID && c.UserID == _currentUserId);

        if (existing != null)
        {
            _db.InteriorComponents.RemoveRange(existing.Components);

            existing.Components = config.Components?.Select(c => new InteriorComponent
            {
                ConfigID = existing.ConfigID,
                Name = c.Name,
                Type = c.Type,
                Tier = c.Tier,
                Material = c.Material,
                Width = c.Width,
                Height = c.Height,
                Depth = c.Depth,
                Cost = c.Cost,
                PropertiesJson = c.PropertiesJson
            }).ToList();

            existing.ModelName = config.ModelName;
            existing.SeatingCapacity = config.SeatingCapacity;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            var newConfig = new JetConfiguration
            {
                ConfigID = config.ConfigID,
                UserID = _currentUserId,
                ModelName = config.ModelName,
                SeatingCapacity = config.SeatingCapacity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Components = config.Components?.Select(c => new InteriorComponent
                {
                    ConfigID = config.ConfigID,
                    Name = c.Name,
                    Type = c.Type,
                    Tier = c.Tier,
                    Material = c.Material,
                    Width = c.Width,
                    Height = c.Height,
                    Depth = c.Depth,
                    Cost = c.Cost,
                    PropertiesJson = c.PropertiesJson
                }).ToList()
            };

            await _db.JetConfigurations.AddAsync(newConfig);
        }

        await _db.SaveChangesAsync();
        return true;
    }

    // Matches interface exactly
    public async Task<bool> SaveAllAsync(List<JetConfiguration> configs)
    {
        foreach (var config in configs)
        {
            await SaveConfigAsync(config);
        }
        return true;
    }
}