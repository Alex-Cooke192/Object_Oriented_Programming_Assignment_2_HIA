using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using JIDS.Helpers;
using JIDS.Interfaces;
using JIDS.Models;

namespace JIDS.ViewModels
{
    public class SummaryViewModel : INotifyPropertyChanged
    {
        private readonly INavigationService? _navigation;
        private readonly IConfigurationServiceWriter? _writer;
        private readonly IUserSessionService? _session;

        private readonly bool _isNew;
        private JetConfiguration _model;

        public string? Name => _model.Name;
        public string? CabinDimensions => _model.CabinDimensions;
        public int SeatingCapacity => _model.SeatingCapacity;
        public ObservableCollection<InteriorComponent> InteriorComponents { get; } = new();

        private string? _statusMessage;
        public string? StatusMessage { get => _statusMessage; set { _statusMessage = value; OnPropertyChanged(); } }

        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand SubmitCommand { get; }

        public SummaryViewModel(JetConfiguration? configuration,
                                INavigationService? navigation,
                                IConfigurationServiceWriter? writer,
                                IUserSessionService? session)
        {
            _navigation = navigation;
            _writer = writer;
            _session = session;

            if (configuration == null)
            {
                _isNew = true;
                _model = new JetConfiguration
                {
                    ConfigID = Guid.NewGuid(),
                    UserID = session?.CurrentUser?.UserID ?? Guid.Empty,
                    Name = "New configuration",
                    CabinDimensions = string.Empty,
                    SeatingCapacity = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    InteriorComponents = new System.Collections.Generic.List<InteriorComponent>()
                };
            }
            else
            {
                _isNew = false;
                // clone to avoid accidental modification in summary
                _model = new JetConfiguration
                {
                    ConfigID = configuration.ConfigID,
                    UserID = configuration.UserID,
                    Name = configuration.Name,
                    CabinDimensions = configuration.CabinDimensions,
                    SeatingCapacity = configuration.SeatingCapacity,
                    Version = configuration.Version,
                    CreatedAt = configuration.CreatedAt,
                    UpdatedAt = configuration.UpdatedAt,
                    InteriorComponents = configuration.InteriorComponents?.Select(c => new InteriorComponent
                    {
                        ComponentID = c.ComponentID,
                        ConfigID = c.ConfigID,
                        Name = c.Name,
                        Type = c.Type,
                        Tier = c.Tier,
                        Material = c.Material,
                        Position = c.Position,
                        PropertiesJson = c.PropertiesJson,
                        CreatedAt = c.CreatedAt
                    }).ToList() ?? new System.Collections.Generic.List<InteriorComponent>()
                };
            }

            // populate observable list for UI
            foreach (var c in _model.InteriorComponents ?? Enumerable.Empty<InteriorComponent>())
                InteriorComponents.Add(c);

            EditCommand = new RelayCommand(_ => Edit());
            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            ExportCommand = new RelayCommand(async _ => await ExportAsync());
            SubmitCommand = new RelayCommand(async _ => await SubmitAsync());
        }

        private void Edit()
        {
            // Navigate back to editor with the configuration
            _navigation?.NavigateTo("ConfigurationEditor", _model);
        }

        private async Task SaveAsync()
        {
            try
            {
                if (_writer != null)
                {
                    if (_isNew)
                    {
                        var created = await _writer.CreateConfigurationAsync(_model.Name ?? "New config", _model);
                        StatusMessage = created != null ? "Configuration created." : "Create failed.";
                        if (created != null) _navigation?.NavigateTo("ConfigurationList");
                    }
                    else
                    {
                        var ok = await _writer.SaveAllChangesAsync(new System.Collections.Generic.List<JetConfiguration> { _model });
                        StatusMessage = ok ? "Configuration saved." : "Save failed.";
                        if (ok) _navigation?.NavigateTo("ConfigurationList");
                    }
                }
                else
                {
                    StatusMessage = "No writer available to save.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Save failed: {ex.Message}";
            }
        }

        private async Task ExportAsync()
        {
            try
            {
                var safeName = string.IsNullOrWhiteSpace(_model.Name) ? "config" : string.Concat(_model.Name.Split(Path.GetInvalidFileNameChars()));
                var fileName = $"{safeName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
                var exportDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "exports");
                Directory.CreateDirectory(exportDir);
                var path = Path.Combine(exportDir, fileName);

                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_model, options);
                await File.WriteAllTextAsync(path, json);

                StatusMessage = $"Exported to {path}";
                // Optionally show a dialog
                MessageBox.Show($"Exported to:\n{path}", "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Export failed: {ex.Message}";
            }
        }

        private async Task SubmitAsync()
        {
            // Submit = Save then navigate to list (business rules may differ)
            await SaveAsync();
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion
    }
}