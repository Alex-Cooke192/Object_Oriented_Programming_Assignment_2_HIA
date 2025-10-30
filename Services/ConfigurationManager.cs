using System.Text.Json;
using JetInteriorApp.Models;
using JetInteriorApp.Interfaces; 

namespace JetInteriorApp.Services.Configuration
{
    public class ConfigurationManager : IConfigurationServiceReader, IConfigurationServiceWriter
    {
        private readonly IConfigurationRepository _repository;
        private readonly List<JetLayout> _inMemoryConfigs;

        public ConfigurationManager(IConfigurationRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _inMemoryConfigs = LoadAllFromRepository();
        }

        private List<JetLayout> LoadAllFromRepository()
        {
            var raw = _repository.LoadAll(); // Dictionary<Guid, string>
            var configs = new List<JetLayout>();

            foreach (var kvp in raw)
            {
                try
                {
                    var layout = JsonSerializer.Deserialize<JetLayout>(kvp.Value);
                    configs.Add(new JetLayout
                    {
                        ID = kvp.Key,
                        ConfigJson = kvp.Value,
                        LayoutName = layout?.LayoutName,
                        UserId = layout?.UserId ?? Guid.Empty
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Failed to deserialize layout {kvp.Key}: {ex.Message}");
                }
            }

            return configs;
        }

        public JetLayout GetConfiguration(Guid id)
        {
            return _inMemoryConfigs.FirstOrDefault(c => c.ID == id);
        }


        public JetLayout CreateConfiguration(Guid userId, string layoutName, JetLayout layout)
        {
            var newId = Guid.NewGuid();
            layout.UserId = userId;
            layout.LayoutName = layoutName;

            var json = JsonSerializer.Serialize(layout);
            var config = new JetLayout
            {
                ID = newId,
                UserId = userId,
                LayoutName = layoutName,
                ConfigJson = json
            };

            _inMemoryConfigs.Add(config);
            return config;
        }

        public JetLayout CloneConfiguration(Guid id, JetLayout config)
        {
            var original = GetConfiguration(id);
            if (original == null) return null;

            var layout = JsonSerializer.Deserialize<JetLayout>(original.ConfigJson);
            layout.LayoutName += " (Clone)";
            layout.UserId = original.UserId;

            return CreateConfiguration(original.UserId, layout.LayoutName, layout);
        }

        public bool DeleteConfiguration(Guid id)
        {
            var config = GetConfiguration(id);
            if (config == null) return false;

            return _inMemoryConfigs.Remove(config);
        }

        public bool SaveAllChanges()
        {
            try
            {
                var dict = _inMemoryConfigs.ToDictionary(c => c.ID, c => c.ConfigJson);
                return _repository.SaveAll(dict);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Save failed: {ex.Message}");
                return false;
            }
        }
    }
}