using System.Text.Json;
using JetInteriorApp.Models;
using JetInteriorApp.Interfaces;
using System.ComponentModel;

namespace JetInteriorApp.Services
{
    public class JsonComponentPorter : IComponentRepositoryPorter
    {
        public async Task<List<InteriorComponent>> GetFullComponentsForUser(Guid userId)
        {
            var configs = await _db.JdcConfig
                .Where(c => c.UserId == userId)
                .Select(c => c.Id)
                .ToListAsync();

            var components = await _db.InteriorComponents
                .Where(fc => configs.Contains(fc.ConfigId))
                .ToListAsync();

            var ComponentList = new List<InteriorComponent>();

            foreach (var comp in components)
            {
                var full = new InteriorComponent
                {
                    Id = comp.Id,
                    ConfigId = comp.ConfigId,
                    Type = comp.Type,
                    Name = comp.Name,
                    Description = comp.Description,
                    Material = comp.Material,
                    Weight = comp.Weight,
                    Cost = comp.Cost,
                    Size = comp.Size,
                    Color = comp.Color,
                    Accessibility = comp.Accessibility
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