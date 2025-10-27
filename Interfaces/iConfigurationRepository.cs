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
    Task<IDictionary<string, ConfigurationItem>> LoadAllAsync();

    /// <summary>
    /// Saves all configuration entries asynchronously.
    /// </summary>
    Task SaveAllAsync(IEnumerable<ConfigurationItem> configs);

    /// <summary>
    /// Saves a single configuration entry asynchronously.
    /// </summary>
    Task SaveConfigAsync(ConfigurationItem config);
}