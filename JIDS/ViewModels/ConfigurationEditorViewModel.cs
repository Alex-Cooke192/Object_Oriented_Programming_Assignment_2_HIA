using JIDS.Helpers;
using JIDS.Interfaces;
using JIDS.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;

namespace JIDS.ViewModels
{
    public class ConfigurationEditorViewModel : INotifyPropertyChanged
    {
        private readonly IConfigurationRepository? _repository;
        private readonly IConfigurationServiceWriter? _writer;
        private readonly INavigationService? _navigation;
        private readonly IUserSessionService? _session;

        public Guid ConfigID { get; private set; }

        private string? _name;
        public string? Name { get => _name; set { _name = value; OnPropertyChanged(); } }

        private string? _cabinDimensions;
        public string? CabinDimensions { get => _cabinDimensions; set { _cabinDimensions = value; OnPropertyChanged(); } }

        private int _seatingCapacity;
        public int SeatingCapacity { get => _seatingCapacity; set { _seatingCapacity = value; OnPropertyChanged(); } }

        public ObservableCollection<InteriorComponent> InteriorComponents { get; } = new();

        private InteriorComponent? _selectedComponent;
        public InteriorComponent? SelectedComponent { get => _selectedComponent; set { _selectedComponent = value; OnPropertyChanged(); } }

        private string? _statusMessage;
        public string? StatusMessage { get => _statusMessage; set { _statusMessage = value; OnPropertyChanged(); } }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand AddComponentCommand { get; }
        public ICommand RemoveComponentCommand { get; }
        public ICommand CloneCommand { get; }
        public ICommand ReviewCommand { get; } // added: command to open Summary

        private readonly bool _isNew;

        public ConfigurationEditorViewModel(
            IConfigurationRepository? repository,
            IConfigurationServiceWriter? writer,
            INavigationService? navigation,
            IUserSessionService? session,
            JetConfiguration? existing = null)
        {
            _repository = repository;
            _writer = writer;
            _navigation = navigation;
            _session = session;

            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            CancelCommand = new RelayCommand(_ => Cancel());
            AddComponentCommand = new RelayCommand(_ => AddComponent());
            RemoveComponentCommand = new RelayCommand(_ => RemoveSelectedComponent(), _ => SelectedComponent != null);
            CloneCommand = new RelayCommand(async _ => await CloneAsync(), _ => existing != null);
            ReviewCommand = new RelayCommand(_ => Review()); // initialize review command

            if (existing != null)
            {
                _isNew = false;
                ConfigID = existing.ConfigID;
                Name = existing.Name;
                CabinDimensions = existing.CabinDimensions;
                SeatingCapacity = existing.SeatingCapacity;
                if (existing.InteriorComponents != null)
                {
                    foreach (var c in existing.InteriorComponents)
                        InteriorComponents.Add(CloneComponentForEditing(c));
                }
            }
            else
            {
                _isNew = true;
                ConfigID = Guid.NewGuid();
                Name = "New configuration";
                CabinDimensions = string.Empty;
                SeatingCapacity = 0;
            }
        }

        private InteriorComponent CloneComponentForEditing(InteriorComponent src)
        {
            return new InteriorComponent
            {
                ComponentID = src.ComponentID,
                ConfigID = src.ConfigID,
                Name = src.Name,
                Type = src.Type,
                Tier = src.Tier,
                Material = src.Material,
                Position = src.Position,
                PropertiesJson = src.PropertiesJson,
                CreatedAt = src.CreatedAt
            };
        }

        private void AddComponent()
        {
            var comp = new InteriorComponent
            {
                ComponentID = Guid.NewGuid(),
                ConfigID = ConfigID,
                Name = "New Component",
                Type = "Seat",
                Tier = "Economy",
                Material = "Default",
                Position = "{x:0,y:0}",
                PropertiesJson = "{}",
                CreatedAt = DateTime.UtcNow
            };
            InteriorComponents.Add(comp);
            SelectedComponent = comp;
        }

        private void RemoveSelectedComponent()
        {
            if (SelectedComponent == null) return;
            InteriorComponents.Remove(SelectedComponent);
            SelectedComponent = null;
        }

        private async Task SaveAsync()
        {
            try
            {
                // Determine final ConfigID up front so components can use it
                var finalConfigId = _isNew ? Guid.NewGuid() : ConfigID;

                // Build domain object
                var config = new JetConfiguration
                {
                    ConfigID = finalConfigId,
                    UserID = _session?.CurrentUser?.UserID ?? Guid.Empty,
                    Name = Name,
                    CabinDimensions = CabinDimensions,
                    SeatingCapacity = SeatingCapacity,
                    Version = (_isNew ? 1 : (int?)null),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    InteriorComponents = InteriorComponents.Select(c => new InteriorComponent
                    {
                        ComponentID = c.ComponentID,
                        ConfigID = finalConfigId,
                        Name = c.Name,
                        Type = c.Type,
                        Tier = c.Tier,
                        Material = c.Material,
                        Position = c.Position,
                        CreatedAt = c.CreatedAt == default ? DateTime.UtcNow : c.CreatedAt,
                        PropertiesJson = c.PropertiesJson
                    }).ToList()
                };

                if (_writer != null)
                {
                    // If writer supports CreateConfigurationAsync for new
                    if (_isNew)
                    {
                        // Use CreateConfigurationAsync to create new config
                        var created = await _writer.CreateConfigurationAsync(config.Name ?? "New config", config);
                        StatusMessage = created != null ? "Configuration created." : "Create failed.";
                    }
                    else
                    {
                        // Save this single config via SaveAllChangesAsync (pass single-item list)
                        var ok = await _writer.SaveAllChangesAsync(new List<JetConfiguration> { config });
                        StatusMessage = ok ? "Configuration saved." : "Save failed.";
                    }
                }
                else if (_repository != null)
                {
                    // Best-effort: if repository exposes SaveConfigAsync directly (concrete impl)
                    var saveMethod = _repository.GetType().GetMethod("SaveConfigAsync");
                    if (saveMethod != null)
                    {
                        var task = (Task<bool>)saveMethod.Invoke(_repository, new object[] { config })!;
                        var result = await task;
                        StatusMessage = result ? "Configuration saved." : "Save failed.";
                    }
                    else
                    {
                        StatusMessage = "No writer available to save configuration.";
                    }
                }
                else
                {
                    StatusMessage = "No persistence available.";
                }

                // Navigate back to list after a short delay so the message can be seen (optional)
                await Task.Delay(250);
                _navigation?.NavigateTo("ConfigurationList");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Save failed: {ex.Message}";
            }
        }

        private void Cancel()
        {
            _navigation?.NavigateTo("ConfigurationList");
        }

        private async Task CloneAsync()
        {
            try
            {
                if (_writer == null)
                {
                    StatusMessage = "Clone not available (no writer).";
                    return;
                }

                var result = await _writer.CloneConfigurationAsync(ConfigID);
                if (result != null)
                {
                    StatusMessage = "Configuration cloned.";
                    // After cloning, show the list (or open the cloned config in editor if desired)
                    _navigation?.NavigateTo("ConfigurationList");
                }
                else
                {
                    StatusMessage = "Clone failed.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Clone failed: {ex.Message}";
            }
        }

        // New: build a snapshot and navigate to Summary
        private void Review()
        {
            try
            {
                var finalId = ConfigID;
                var snapshot = new JetConfiguration
                {
                    ConfigID = finalId,
                    UserID = _session?.CurrentUser?.UserID ?? Guid.Empty,
                    Name = Name,
                    CabinDimensions = CabinDimensions,
                    SeatingCapacity = SeatingCapacity,
                    Version = _isNew ? 1 : (int?)null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    InteriorComponents = InteriorComponents.Select(c => new InteriorComponent
                    {
                        ComponentID = c.ComponentID,
                        ConfigID = finalId,
                        Name = c.Name,
                        Type = c.Type,
                        Tier = c.Tier,
                        Material = c.Material,
                        Position = c.Position,
                        PropertiesJson = c.PropertiesJson,
                        CreatedAt = c.CreatedAt == default ? DateTime.UtcNow : c.CreatedAt
                    }).ToList()
                };

                _navigation?.NavigateTo("Summary", snapshot);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Review failed: {ex.Message}";
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion
    }
}