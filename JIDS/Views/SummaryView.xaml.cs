using System;
using System.Windows;
using System.Windows.Controls;
using JIDS.Interfaces;
using JIDS.Models;

namespace JIDS.Views
{
    public partial class SummaryView : Page
    {
        public SummaryView(JetConfiguration? configuration = null)
        {
            InitializeComponent();

            // Resolve services from Application resources
            var nav = Application.Current.Resources["NavigationService"] as INavigationService;
            var writer = Application.Current.Resources["ConfigurationWriter"] as IConfigurationServiceWriter;
            var session = Application.Current.Resources["UserSessionService"] as IUserSessionService;

            // Create and assign ViewModel
            var vm = new JIDS.ViewModels.SummaryViewModel(configuration, nav, writer, session);
            DataContext = vm;
        }

        // Parameterless ctor for designer
        public SummaryView() : this(null) { }
    }
}