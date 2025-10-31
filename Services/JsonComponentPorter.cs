using System.Text.Json;
using JetInteriorApp.Models;
using JetInteriorApp.Interfaces;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace JetInteriorApp.Services
{
    public class JsonComponentPorter : IComponentRepositoryPorter
    {
        private readonly JetDbContext _db; 
        public async Task<List<InteriorComponent>> GetFullComponentsForUser(Guid userId)
        {
        // Step 1: Get all config IDs for the user
        var configIds = await _db.JetConfigs
            .Where(cfg => cfg.UserId == userId)
            .Select(cfg => cfg.Id)
            .ToListAsync();

        // Step 2: Get all components linked to those configs
        var components = await _db.InteriorComponents
            .Where(comp => configIds.Contains(comp.ConfigID))
            .ToListAsync();

            var ComponentList = new List<InteriorComponent>();

            foreach (var comp in components)
            {
                var full = new InteriorComponent
                {
                    Id = comp.Id,
                    ConfigId = comp.ConfigId,
                    Name = comp.Name,
                    Type = comp.Type,
                    position = comp.Position, 
                    Material = comp.Material,
                    Weight = comp.Weight,
                    Color = comp.Color,
                };

                // Load subtype properties based on type
                full.Properties = await LoadPropertiesByType(comp.Id, comp.Type);
                ComponentList.Add(full);
            }
            return ComponentList;
        }
        private async Task<object> LoadPropertiesByType(Guid componentId, string type)
        {
            return type.ToLower() switch
            {
                "seat" => await _db.SeatProperties.FirstOrDefaultAsync(p => p.ComponentId == componentId),
                "screen" => await _db.ScreenProperties.FirstOrDefaultAsync(p => p.ComponentId == componentId),
                "table" => await _db.TableProperties.FirstOrDefaultAsync(p => p.ComponentId == componentId),
                "kitchen" => await _db.KitchenProperties.FirstOrDefaultAsync(p => p.ComponentId == componentId),
                "storage" => await _db.StorageCabinetProperties.FirstOrDefaultAsync(p => p.ComponentId == componentId),
                "toilet" => await _db.ToiletProperties.FirstOrDefaultAsync(p => p.ComponentId == componentId),
                "lighting" => await _db.LightingProperties.FirstOrDefaultAsync(p => p.ComponentId == componentId),
                "emergency_exit" => await _db.EmergencyExitProperties.FirstOrDefaultAsync(p => p.ComponentId == componentId),
                _ => null
            };
        }
    }
}