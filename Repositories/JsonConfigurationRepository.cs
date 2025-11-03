using JetInteriorApp.Models;
using JetInteriorApp.Interfaces;
using JetInteriorApp.Data; 
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.ComponentModel.Design;

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
    var dbConfigs = await _db.JetConfigurations
        .Where(c => c.UserID == _currentUserId)
        .Include(c => c.InteriorComponents)
        .ToListAsync();

    var domainConfigs = dbConfigs.Select(db => new JetConfiguration
    {
        ConfigID = db.ConfigID,
        UserID = db.UserID,
        Name = db.Name,
        SeatingCapacity = db.SeatingCapacity,
        CreatedAt = db.CreatedAt,
        UpdatedAt = db.UpdatedAt,
        InteriorComponents = db.InteriorComponents?.Select(ic => new InteriorComponent
        {
            ComponentID = ic.ComponentID,
            ConfigID = ic.ConfigID,
            Name = ic.Name,
            Type = ic.Type,
            Tier = ic.Tier,
            Material = ic.Material,
            Width = ic.Width,
            Height = ic.Height,
            Depth = ic.Depth,
            Cost = ic.Cost,
            PropertiesJson = ic.PropertiesJson
        }).ToList()
    }).ToList();

    return domainConfigs;
}

    // Matches interface exactly
    public async Task<bool> SaveConfigAsync(JetConfiguration config)
    {
        if (config == null) return false;

        // Convert to DB entity
        var configDb = new JetConfigurationDB
        {
            ConfigID = config.ConfigID,
            UserID = _currentUserId,
            Name = config.Name,
            SeatingCapacity = config.SeatingCapacity,
            CabinDimensions = config.CabinDimensions, 
            CreatedAt = config.CreatedAt,
            UpdatedAt = DateTime.UtcNow,
            InteriorComponents = config.InteriorComponents.Select(c => new InteriorComponentDB
            {
                ComponentID = c.ComponentID,
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
        // Try find if this config exists currently in the database
        var existing = await _db.JetConfigurations
            .Include(c => c.InteriorComponents)
            .FirstOrDefaultAsync(c => c.ConfigID == configDb.ConfigID && c.UserID == _currentUserId);
        // If the configuration already exists, update relevant fields
        if (existing != null)
        {
            _db.InteriorComponents.RemoveRange(existing.InteriorComponents);

            existing.InteriorComponents = configDb.InteriorComponents;
            existing.Name = configDb.Name;
            existing.SeatingCapacity = configDb.SeatingCapacity;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            // No existing config was found, so create new one
            await _db.JetConfigurations.AddAsync(configDb);
        }
        // Commit changes
        await _db.SaveChangesAsync();
        return true;
    }


    // Matches interface exactly
    public async Task<bool> SaveAllAsync(List<JetConfiguration> configs)
    {
        // Get all configs currently in the DB for this user
        var existingConfigs = await _db.JetConfigurations
            .Where(c => c.UserID == _currentUserId)
            .Include(c => c.InteriorComponents)
            .ToListAsync();

        // Find configs that no longer exist in memory
        var toRemove = existingConfigs
            .Where(dbConfig => !configs.Any(c => c.ConfigID == dbConfig.ConfigID))
            .ToList();

        if (toRemove.Any())
        {
            // Remove related interior components first (if cascade delete not configured)
            foreach (var config in toRemove)
            {
                _db.InteriorComponents.RemoveRange(config.InteriorComponents);
            }
            // Remove the deleted configuration
            _db.JetConfigurations.RemoveRange(toRemove);
        }

        // Save or update the rest
        foreach (var config in configs)
        {
            await SaveConfigAsync(config);
        }

        await _db.SaveChangesAsync();
        return true;
    }
}