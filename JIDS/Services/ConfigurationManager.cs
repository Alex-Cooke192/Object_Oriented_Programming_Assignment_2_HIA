using JetInteriorApp.Models;
using JetInteriorApp.Interfaces;
using System.Collections.Concurrent;

namespace JetInteriorApp.Services.Configuration
{
    public class ConfigurationManager : IConfigurationServiceReader, IConfigurationServiceWriter
    {
        private readonly IConfigurationRepository _repository;
        private ConcurrentDictionary<Guid, JetConfiguration> _inMemoryConfigs;
        private readonly Guid _userId;

        public ConfigurationManager(IConfigurationRepository repository, Guid userId)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _userId = userId;
            _inMemoryConfigs = new ConcurrentDictionary<Guid, JetConfiguration>();
        }

        public async Task<List<JetConfiguration>> InitializeAsync()
        {
            var configs = await _repository.LoadAllAsync();
            _inMemoryConfigs = new ConcurrentDictionary<Guid, JetConfiguration>(
                configs.ToDictionary(c => c.ConfigID)
            );
            return configs;
        }

        public JetConfiguration? GetConfiguration(Guid id)
        {
            _inMemoryConfigs.TryGetValue(id, out var config);
            return config;
        }

        public async Task<JetConfiguration> CreateConfigurationAsync(string name, JetConfiguration baseLayout)
        {
            var newConfig = new JetConfiguration
            {
                ConfigID = Guid.NewGuid(),
                UserID = _userId,
                Name = name,
                CabinDimensions = baseLayout.CabinDimensions,
                SeatingCapacity = baseLayout.SeatingCapacity,
                Version = (baseLayout.Version ?? 0) + 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                InteriorComponents = baseLayout.InteriorComponents?.Select(c => new InteriorComponent
                {
                    ComponentID = Guid.NewGuid(),
                    ConfigID = Guid.NewGuid(), // will be replaced after creation
                    Name = c.Name,
                    Type = c.Type,
                    Tier = c.Tier,
                    Material = c.Material,
                    Position = c.Position,
                    CreatedAt = DateTime.UtcNow,
                    PropertiesJson = c.PropertiesJson
                }).ToList() ?? new List<InteriorComponent>()
            };

            // Fix component ConfigID to the newly created config
            foreach (var ic in newConfig.InteriorComponents)
                ic.ConfigID = newConfig.ConfigID;

            var success = await _repository.SaveConfigAsync(newConfig);
            if (success) _inMemoryConfigs[newConfig.ConfigID] = newConfig;

            return newConfig;
        }

        public async Task<JetConfiguration?> CloneConfigurationAsync(Guid configId)
        {
            if (!_inMemoryConfigs.TryGetValue(configId, out var original))
                return null;

            var clone = new JetConfiguration
            {
                ConfigID = Guid.NewGuid(),
                UserID = original.UserID,
                Name = $"{original.Name} (Clone)",
                CabinDimensions = original.CabinDimensions,
                SeatingCapacity = original.SeatingCapacity,
                Version = (original.Version ?? 0) + 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                InteriorComponents = original.InteriorComponents?
                    .Select(c => new InteriorComponent
                    {
                        ComponentID = Guid.NewGuid(),
                        ConfigID = Guid.Empty, // fix after assignment
                        Name = c.Name,
                        Type = c.Type,
                        Tier = c.Tier,
                        Material = c.Material,
                        Position = c.Position,
                        CreatedAt = DateTime.UtcNow,
                        PropertiesJson = c.PropertiesJson
                    })
                    .ToList() ?? new List<InteriorComponent>()
            };

            foreach (var ic in clone.InteriorComponents)
                ic.ConfigID = clone.ConfigID;

            var success = await _repository.SaveConfigAsync(clone);

            if (success)
            {
                _inMemoryConfigs[clone.ConfigID] = clone;
                return clone;
            }

            return null;
        }

        public async Task<bool> DeleteConfigurationAsync(Guid id)
        {
            if (!_inMemoryConfigs.TryRemove(id, out _))
                return false;

            var updatedConfigs = _inMemoryConfigs.Values.ToList();
            return await SaveAllChangesAsync(updatedConfigs);
        }

        public async Task<bool> SaveAllChangesAsync(List<JetConfiguration> configs)
        {
            try
            {
                await _repository.SaveAllAsync(configs);

                _inMemoryConfigs = new ConcurrentDictionary<Guid, JetConfiguration>(
                    configs.ToDictionary(c => c.ConfigID)
                );

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SaveAllChangesAsync failed: {ex.Message}");
                return false;
            }
        }
    }
}

