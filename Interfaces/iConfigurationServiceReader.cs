using JetInteriorApp.Models;

namespace JetInteriorApp.Interfaces
{
    /// <summary>
    /// Interface for accessing and persisting configuration data.
    /// Supports multiple storage formats (e.g., JSON, XML, SQL) via polymorphic implementations.
    /// Promotes single responsibility and extensibility.
    /// </summary>
    public interface IConfigurationServiceReader
    {
        /// <summary>
        /// Fetches a single configuration by ID
        /// </summary>
        JetConfiguration GetConfiguration(Guid id);
    }
}