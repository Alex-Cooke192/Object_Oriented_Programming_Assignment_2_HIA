using JetInteriorApp.Helpers;
using JetInteriorApp.Models;
using JetInteriorApp.Services.Configuration;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JetInteriorApp.ViewModels
{
    public class JetConfigurationListVM : INotifyPropertyChanged
    {
        private readonly ConfigurationManager _manager;

        public ObservableCollection<JetConfigurationModel> Configurations { get; set; }
            = new ObservableCollection<JetConfigurationModel>();

        private JetConfigurationModel? _selectedConfiguration;
        public JetConfigurationModel? SelectedConfiguration
        {
            get => _selectedConfiguration;
            set { _selectedConfiguration = value; OnPropertyChanged(); }
        }

        public string StatusText { get; set; } = "";

        // Commands
        public ICommand RefreshCommand { get; }
        public ICommand SaveAllCommand { get; }
        public ICommand NewConfigCommand { get; }
        public ICommand CloneConfigCommand { get; }
        public ICommand DeleteConfigCommand { get; }
        public ICommand AddComponentCommand { get; }
        public ICommand RemoveComponentCommand { get; }

        public JetConfigurationListVM(ConfigurationManager manager)
        {
            _manager = manager;

            RefreshCommand = new RelayCommand(async _ => await Refresh());
            SaveAllCommand = new RelayCommand(async _ => await SaveAll());
            NewConfigCommand = new RelayCommand(async _ => await NewConfig());
            CloneConfigCommand = new RelayCommand(async _ => await CloneConfig());
            DeleteConfigCommand = new RelayCommand(async _ => await DeleteConfig());
            AddComponentCommand = new RelayCommand(_ => AddComponent());
            RemoveComponentCommand = new RelayCommand(_ => RemoveComponent());

            _ = Refresh();
        }

        private async Task Refresh()
        {
            StatusText = "Loading...";
            OnPropertyChanged(nameof(StatusText));

            // Get list of domain configs directly from InitializeAsync
            var configs = await _manager.InitializeAsync();

            Configurations.Clear();
            foreach (var c in configs.Select(ToVM))
                Configurations.Add(c);

            StatusText = "Loaded.";
            OnPropertyChanged(nameof(StatusText));
        }


        private async Task NewConfig()
        {
            var baseLayout = new JetConfiguration
            {
                CabinDimensions = "10x3x2.5m",
                SeatingCapacity = 6,
                Version = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            var created = await _manager.CreateConfigurationAsync("New Config", baseLayout);
            Configurations.Add(ToVM(created));
        }

        private async Task CloneConfig()
        {
            if (SelectedConfiguration == null) return;

            var cloned = await _manager.CloneConfigurationAsync(SelectedConfiguration.ConfigID);
            if (cloned != null)
                Configurations.Add(ToVM(cloned));
        }

        private async Task DeleteConfig()
        {
            if (SelectedConfiguration == null) return;
            await _manager.DeleteConfigurationAsync(SelectedConfiguration.ConfigID);
            Configurations.Remove(SelectedConfiguration);
            SelectedConfiguration = null;
        }

        private async Task SaveAll()
        {
            StatusText = "Saving...";
            OnPropertyChanged(nameof(StatusText));

            await _manager.SaveAllChangesAsync();

            StatusText = "Saved";
            OnPropertyChanged(nameof(StatusText));
        }

        private void AddComponent()
        {
            if (SelectedConfiguration == null) return;

            SelectedConfiguration.Components.Add(new ComponentModel
            {
                ComponentID = Guid.NewGuid(),
                Name = "New Component",
                Type = "Seat",
                Tier = "Economy",
                Material = "Fabric",
                X = 0,
                Y = 0,
                CreatedAt = DateTime.UtcNow
            });
        }

        private void RemoveComponent()
        {
            if (SelectedConfiguration == null) return;
            if (SelectedConfiguration.Components.Any())
                SelectedConfiguration.Components.RemoveAt(SelectedConfiguration.Components.Count - 1);
        }

        private JetConfigurationModel ToVM(JetConfiguration config)
        {
            var vm = new JetConfigurationModel
            {
                ConfigID = config.ConfigID,
                UserID = config.UserID,
                Name = config.Name,
                CabinDimensions = config.CabinDimensions ?? "",
                SeatingCapacity = config.SeatingCapacity,
                CreatedAt = config.CreatedAt,
                UpdatedAt = config.UpdatedAt,
                Version = config.Version
            };

            if (config.InteriorComponents != null)
            {
                foreach (var ic in config.InteriorComponents)
                {
                    vm.Components.Add(new ComponentModel
                    {
                        ComponentID = ic.ComponentID,
                        Name = ic.Name,
                        Type = ic.Type,
                        Tier = ic.Tier,
                        Material = ic.Material,
                        CreatedAt = ic.CreatedAt,
                        Position = ic.Position ?? "{\"x\":0,\"y\":0}",
                        PropertiesJson = ic.PropertiesJson ?? "{}"
                    });
                }
            }

            return vm;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }


    // ✅ Nested configuration record
    public class JetConfigurationModel : INotifyPropertyChanged
    {
        private string _name = "";
        private string _cabinDimensions = "";
        private int _seatingCapacity;
        private int? _version;

        public Guid ConfigID { get; set; }
        public Guid UserID { get; set; }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string CabinDimensions
        {
            get => _cabinDimensions;
            set { _cabinDimensions = value; OnPropertyChanged(); }
        }

        public int SeatingCapacity
        {
            get => _seatingCapacity;
            set { _seatingCapacity = value; OnPropertyChanged(); }
        }

        public int? Version
        {
            get => _version;
            set { _version = value; OnPropertyChanged(); }
        }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ObservableCollection<ComponentModel> Components { get; set; }
            = new ObservableCollection<ComponentModel>();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }

    // ✅ Nested component record
    public class ComponentModel : INotifyPropertyChanged
    {
        private string _name = "";
        private string _type = "";
        private string _tier = "";
        private string _material = "";
        private string _position = "{}";
        private string _propertiesJson = "{}";
        private double _x;
        private double _y;

        public Guid ComponentID { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Type
        {
            get => _type;
            set { _type = value; OnPropertyChanged(); }
        }

        public string Tier
        {
            get => _tier;
            set { _tier = value; OnPropertyChanged(); }
        }

        public string Material
        {
            get => _material;
            set { _material = value; OnPropertyChanged(); }
        }

        public string Position
        {
            get => _position;
            set { _position = value; TryParsePosition(); OnPropertyChanged(); }
        }

        public string PropertiesJson
        {
            get => _propertiesJson;
            set { _propertiesJson = value; OnPropertyChanged(); }
        }

        public double X
        {
            get => _x;
            set { _x = value; UpdatePositionJson(); OnPropertyChanged(); }
        }

        public double Y
        {
            get => _y;
            set { _y = value; UpdatePositionJson(); OnPropertyChanged(); }
        }

        private void TryParsePosition()
        {
            try
            {
                using var doc = JsonDocument.Parse(_position);
                if (doc.RootElement.TryGetProperty("x", out var px))
                    _x = px.GetDouble();
                if (doc.RootElement.TryGetProperty("y", out var py))
                    _y = py.GetDouble();
            }
            catch
            {
                _x = 0;
                _y = 0;
            }
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
        }

        private void UpdatePositionJson()
        {
            _position = $"{{\"x\":{_x},\"y\":{_y}}}";
            OnPropertyChanged(nameof(Position));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
