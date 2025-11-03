using System.Text.Json;
using JetInteriorApp.Models;
using JetInteriorApp.Interfaces;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;

namespace JetInteriorApp.Services.Configuration
{
    /// <summary>
    /// Service responsible for managing in-memory configurations
    /// and synchronizing with the persistent repository layer.
    /// </summary>
    public class ConfigurationManager : IConfigurationServiceReader, IConfigurationServiceWriter
    {
        private readonly IConfigurationRepository _repository;
        private readonly ConcurrentDictionary<Guid, JetConfiguration> _inMemoryConfigs;
        private readonly Guid _userId;

        public ConfigurationManager(IConfigurationRepository repository, Guid userId)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _userId = userId;
            _inMemoryConfigs = new ConcurrentDictionary<Guid, JetConfiguration>();
        }

        /// <summary>
        /// Loads all configurations from the repository into memory.
        /// </summary>
        public async Task InitializeAsync()
        {
            var configs = await _repository.LoadAllAsync();
            _inMemoryConfigs.Clear();

            foreach (var config in configs)
            {
                _inMemoryConfigs[config.ConfigID] = config;
            }
        }

        /// <summary>
        /// Retrieves a configuration from memory by ID.
        /// </summary>
        public JetConfiguration? GetConfiguration(Guid id)
        {
            _inMemoryConfigs.TryGetValue(id, out var config);
            return config;
        }

        /// <summary>
        /// Creates a new configuration and saves it to the repository.
        /// </summary>
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
                    ConfigID = Guid.Empty, // will be set on save
                    Name = c.Name,
                    Type = c.Type,
                    Tier = c.Tier,
                    Material = c.Material,
                    Position = c.Position,
                    CreatedAt = DateTime.UtcNow,
                    PropertiesJson = c.PropertiesJson
                }).ToList() ?? new List<InteriorComponent>()
            };

            var success = await _repository.SaveConfigAsync(newConfig);
            if (success)
            {
                _inMemoryConfigs[newConfig.ConfigID] = newConfig;
            }

            return newConfig;
        }

        /// <summary>
        /// Clones an existing configuration for the same user.
        /// </summary>
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
                InteriorComponents = original.InteriorComponents?.Select(c => new InteriorComponent
                {
                    ComponentID = Guid.NewGuid(),
                    ConfigID = Guid.Empty,
                    Name = c.Name,
                    Type = c.Type,
                    Tier = c.Tier,
                    Material = c.Material,
                    Position = c.Position,
                    CreatedAt = DateTime.UtcNow,
                    PropertiesJson = c.PropertiesJson
                }).ToList() ?? new List<InteriorComponent>()
            };

            var success = await _repository.SaveConfigAsync(clone);
            if (success)
            {
                _inMemoryConfigs[clone.ConfigID] = clone;
                return clone;
            }

            return null;
        }

        /// <summary>
        /// Deletes a configuration from memory and the repository.
        /// </summary>
        public async Task<bool> DeleteConfigurationAsync(Guid id)
        {
            if (!_inMemoryConfigs.TryRemove(id, out _))
                return false;

            // After removal, persist updated list
            return await SaveAllChangesAsync();
        }

        /// <summary>
        /// Persists all in-memory configurations to the repository.
        /// </summary>
        public async Task<bool> SaveAllChangesAsync()
        {
            try
            {
                var configs = _inMemoryConfigs.Values.ToList();
                await _repository.SaveAllAsync(configs);
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
