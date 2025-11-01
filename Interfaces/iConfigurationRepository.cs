using JetInteriorApp.Models;

namespace JetInteriorApp.Interfaces
{
    /// <summary>
    /// Interface for accessing and persisting configuration data.
    /// Supports multiple storage formats (e.g., JSON, XML, SQL) via polymorphic implementations.
    /// Promotes single responsibility and extensibility.
    /// </summary>
    public interface IConfigurationRepository
    {
        /// <summary>
        /// Loads all configuration entries asynchronously.
        /// </summary>
        Task<List<JetConfiguration>> LoadAllAsync();

        /// <summary>
        /// Saves all configuration entries asynchronously.
        /// </summary>
        Task<bool> SaveAllAsync(Dictionary<Guid, JetLayout> configs);

        /// <summary>
        /// Saves a single configuration entry asynchronously.
        /// </summary>
        Task<bool> SaveConfigAsync(Guid configId, string JetConfig);
    }
}
