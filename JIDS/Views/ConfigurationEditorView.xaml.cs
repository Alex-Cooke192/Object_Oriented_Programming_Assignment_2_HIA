using System;
using System.Windows.Controls;
using JIDS.Interfaces;
using JIDS.Models;
using System.Windows;

namespace JIDS.Views
{
    public partial class ConfigurationEditorView : Page
    {
        // Constructor used by MainWindow.InstantiateIfExists when navigating with a parameter
        public ConfigurationEditorView(JetConfiguration? parameter = null)
        {
            InitializeComponent();

            // Resolve services from Application resources
            var nav = Application.Current.Resources["NavigationService"] as INavigationService;
            var session = Application.Current.Resources["UserSessionService"] as IUserSessionService;
            var repo = Application.Current.Resources["ConfigurationRepository"] as IConfigurationRepository;
            var writer = Application.Current.Resources["ConfigurationWriter"] as IConfigurationServiceWriter;

            // Create ViewModel using available services and assign DataContext
            var vm = new JIDS.ViewModels.ConfigurationEditorViewModel(repo, writer, nav, session, parameter);
            DataContext = vm;
        }

        // Parameterless constructor for designer / fallback
        public ConfigurationEditorView() : this(null) { }
    }
}