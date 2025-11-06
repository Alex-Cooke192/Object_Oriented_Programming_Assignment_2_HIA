using JetInteriorApp.Interfaces;
using JetInteriorApp.Models;
using JetInteriorApp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Navigation;

namespace JIDS.ViewModels
{
    public class ConfigurationList : INotifyPropertyChanged
    {
        private readonly IConfigurationService _configurationService;
        private readonly INavigationService _navigationService;
        private readonly IUserSessionService _userSession;

        public ObservableCollection<JetConfiguration> Configurations { get; set; }
        public ObservableCollection<JetConfiguration> FilteredConfigurations {  get; set; }

        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
                ApplySearchFilter();
            }
        }

        public ICommand CreateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand LogoutCommand { get; }

        public ConfigurationListViewModel(IConfigurationService configService, INavigationService navService, IUserSessionService userSession)
        {
            _configurationService = configService;
            _navigationService = navService;
            _userSession = userSession;

            Configurations = new ObservableCollection<JetConfiguration>(_configurationService.GetUserConfigurations(_userSession.CurrentUserId));
            FilteredConfigurations = new ObservableCollection<JetConfiguration>(Configurations);

            CreateCommand = new RelayCommand(CreateNew);
            DeleteCommand = new RelayCommand<JetConfiguration>(Delete);
            EditCommand = new RelayCommand(Edit);
            LogoutCommand = new RelayCommand(Logout);
        }

        private void ApplySearchFilter()
        {
            FilteredConfigurations.Clear();
            foreach (var config in Configurations)
            {
                if (string.IsNullOrWhiteSpace(SearchQuery)  || config.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                    FilteredConfigurations.Add(config);
            }
        }

        private void CreateNew() => _navigationService.NavigateTo("ConfigurationEditor");

        private void Edit(JetConfiguration config) => _navigationService.NavigateTo("ConfigurationEditor", config);

        private void Delete(JetConfiguration config)
        {
            _configurationService.DeleteConfiguration(config.Id);
            Configurations.Remove(config);
            ApplySearchFilter();
        }

        private void Logout()
        {
            _userSession.Clear();
            _navigationService.NavigateTo("Login");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}