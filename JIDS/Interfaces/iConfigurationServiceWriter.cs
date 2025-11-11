using JIDS.Interfaces;
using JIDS.Models; 
using System.Threading.Tasks;

namespace JIDS.Interfaces
{
    public interface IConfigurationServiceWriter
    {
        /// <summary>
        /// Creates a new configuration and returns the resulting JetConfiguration.
        /// </summary>
        /// <param name="userId">The ID of the user who owns the configuration.</param>
        /// <param name="layoutName">The name of the new layout.</param>
        /// <param name="layout">The JetLayout object to store.</param>
        /// <returns>The created JetConfiguration.</returns>
        Task<JetConfiguration> CreateConfigurationAsync(string name, JetConfiguration baseLayout);
        
        /*
        /// <summary>
        /// Updates an existing configuration with new layout data.
        /// </summary>
        /// <param name="configId">The ID of the configuration to update.</param>
        /// <param name="layout">The updated JetLayout object.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        bool UpdateConfiguration(Guid configId, JetLayout layout);
        */
        /// <summary>
        /// Clones an existing configuration with a new Id
        /// </summary>
        /// <param name="configId">The ID of the configuration to update.</param>
        /// <param name="layout">The JetLayout object. to be cloned</param>
        /// <returns>The cloned JetLayout</returns>
        Task<JetConfiguration?> CloneConfigurationAsync(Guid configId);

        /// <summary>
        /// Deletes a configuration by its ID.
        /// </summary>
        /// <param name="configId">The ID of the configuration to delete.</param>
        /// <returns>True if the deletion was successful; otherwise, false.</returns>
        Task<bool> DeleteConfigurationAsync(Guid id);

        /// <summary>
        /// Saves all pending changes to the underlying data store.
        /// </summary>
        /// <returns>True if the save was successful; otherwise, false.</returns>
        Task<bool> SaveAllChangesAsync(List<JetConfiguration> configs);
    }
}