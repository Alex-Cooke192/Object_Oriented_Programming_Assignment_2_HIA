using System.Text.Json;
using JetInteriorApp.Models;
using JetInteriorApp.Interfaces; 

namespace JetInteriorApp.Services
{
    class JsonComponentPorter : IComponentRepositoryPorter
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
    }
}