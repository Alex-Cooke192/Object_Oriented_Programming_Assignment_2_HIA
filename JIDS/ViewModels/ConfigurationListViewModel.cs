using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using JIDS.Interfaces;
using JIDS.Models;

namespace JIDS.ViewModels
{
    /// <summary>
    /// ViewModel for the Configuration List screen.
    /// - Displays configurations for the current user (loads from IConfigurationRepository)
    /// - Opens editor to create or edit
    /// - Deletes configuration using IConfigurationServiceWriter
    /// - Supports searching and logout
    /// </summary>
    public class ConfigurationListViewModel : INotifyPropertyChanged
    {
        private readonly IConfigurationRepository _repository;
        private readonly IConfigurationServiceWriter _writer;
        private readonly INavigationService _navigationService;
        private readonly IUserSessionService _userSession;

        public ObservableCollection<JetConfiguration> Configurations { get; }
        public ObservableCollection<JetConfiguration> FilteredConfigurations { get; }

        private JetConfiguration? _selectedConfiguration;
        public JetConfiguration? SelectedConfiguration
        {
            get => _selectedConfiguration;
            set { _selectedConfiguration = value; OnPropertyChanged(); }
        }

        private string? _searchQuery;
        public string? SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (_searchQuery == value) return;
                _searchQuery = value;
                OnPropertyChanged();
                ApplySearchFilter();
            }
        }

        public ICommand CreateCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand RefreshCommand { get; }

        public ConfigurationListViewModel(
            IConfigurationRepository repository,
            IConfigurationServiceWriter writer,
            INavigationService navigationService,
            IUserSessionService userSession)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));

            Configurations = new ObservableCollection<JetConfiguration>();
            FilteredConfigurations = new ObservableCollection<JetConfiguration>();

            CreateCommand = new RelayCommand(() => _navigationService.NavigateTo("ConfigurationEditor"));
            EditCommand = new RelayCommand<JetConfiguration?>(config =>
            {
                if (config != null) _navigationService.NavigateTo("ConfigurationEditor", config);
            }, config => config is not null);

            DeleteCommand = new RelayCommand<JetConfiguration?>(async config => await DeleteAsync(config), config => config is not null);
            LogoutCommand = new RelayCommand(() =>
            {
                _userSession.Clear();
                _navigationService.NavigateTo("Login");
            });

            RefreshCommand = new RelayCommand(async () => await LoadConfigurationsAsync());

            // subscribe to session changes so list reloads when the user switches or logs in/out
            _userSession.SessionChanged += () => { _ = LoadConfigurationsAsync(); };

            // initial load
            _ = LoadConfigurationsAsync();
        }

        private async Task LoadConfigurationsAsync()
        {
            try
            {
                var configs = await _repository.LoadAllAsync();
                Configurations.Clear();
                foreach (var c in configs.OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase))
                    Configurations.Add(c);

                ApplySearchFilter();
            }
            catch (Exception ex)
            {
                // keep ViewModel UI-friendly; logging can be added later
                Console.WriteLine($"LoadConfigurationsAsync failed: {ex.Message}");
            }
        }

        private async Task DeleteAsync(JetConfiguration? config)
        {
            if (config == null) return;

            try
            {
                var success = await _writer.DeleteConfigurationAsync(config.ConfigID);
                if (success)
                {
                    Configurations.Remove(config);
                    ApplySearchFilter();
                }
                else
                {
                    Console.WriteLine($"Delete failed for {config.ConfigID}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteAsync failed: {ex.Message}");
            }
        }

        private void ApplySearchFilter()
        {
            FilteredConfigurations.Clear();

            var query = SearchQuery?.Trim();
            if (string.IsNullOrEmpty(query))
            {
                foreach (var c in Configurations) FilteredConfigurations.Add(c);
                return;
            }

            foreach (var c in Configurations)
            {
                if (!string.IsNullOrEmpty(c.Name) &&
                    c.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                {
                    FilteredConfigurations.Add(c);
                }
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion

        #region Minimal RelayCommand implementations (self-contained)
        // Included so the ViewModel is copy-paste ready even if the project does not expose a command helper.
        private class RelayCommand : ICommand
        {
            private readonly Action _execute;
            private readonly Func<bool>? _canExecute;

            public RelayCommand(Action execute, Func<bool>? canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
            public void Execute(object? parameter) => _execute();
            public event EventHandler? CanExecuteChanged;
            public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        private class RelayCommand<T> : ICommand
        {
            private readonly Action<T?> _execute;
            private readonly Predicate<T?>? _canExecute;

            public RelayCommand(Action<T?> execute, Predicate<T?>? canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public bool CanExecute(object? parameter)
            {
                if (_canExecute == null) return true;
                return parameter is T t ? _canExecute(t) : _canExecute(default);
            }

            public void Execute(object? parameter)
            {
                if (parameter is T t) _execute(t);
                else _execute(default);
            }

            public event EventHandler? CanExecuteChanged;
            public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        private class RelayCommandAsync<T> : ICommand
        {
            private readonly Func<T?, Task> _executeAsync;
            private readonly Predicate<T?>? _canExecute;
            private bool _isExecuting;

            public RelayCommandAsync(Func<T?, Task> executeAsync, Predicate<T?>? canExecute = null)
            {
                _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
                _canExecute = canExecute;
            }

            public bool CanExecute(object? parameter)
            {
                if (_isExecuting) return false;
                if (_canExecute == null) return true;
                return parameter is T t ? _canExecute(t) : _canExecute(default);
            }

            public async void Execute(object? parameter)
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();
                try
                {
                    await _executeAsync(parameter is T t ? t : default);
                }
                finally
                {
                    _isExecuting = false;
                    RaiseCanExecuteChanged();
                }
            }

            public event EventHandler? CanExecuteChanged;
            public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}