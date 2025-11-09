using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JIDS.Interfaces;
using JIDS.Models;
using JIDS.ViewModels;

namespace JIDS
{
    public partial class MainWindow : Window
    {
        private readonly INavigationService _navigationService;
        private readonly IUserSessionService _userSession;

        public MainWindow()
        {
            InitializeComponent();

            // Resolve or create NavigationService and UserSessionService in Application resources
            if (Application.Current.Resources["NavigationService"] is INavigationService nav)
                _navigationService = nav;
            else
            {
                _navigationService = new JIDS.Services.NavigationService();
                Application.Current.Resources["NavigationService"] = _navigationService;
            }

            if (Application.Current.Resources["UserSessionService"] is IUserSessionService session)
                _userSession = session;
            else
            {
                _userSession = new JIDS.Services.UserSessionService();
                Application.Current.Resources["UserSessionService"] = _userSession;
            }

            // Wire UI events
            NavList.SelectionChanged += NavList_SelectionChanged;
            BtnLogout.Click += BtnLogout_Click;

            // Subscribe to navigation requests from viewmodels/services
            _navigationService.Navigated += OnNavigated;

            // Start at login
            _navigationService.NavigateTo("Login");
        }

        private void BtnNew_Click(object? sender, RoutedEventArgs e)
        {
            _navigationService.NavigateTo("ConfigurationEditor");
        }

        private void BtnLogout_Click(object? sender, RoutedEventArgs e)
        {
            _userSession.Clear();
            _navigationService.NavigateTo("Login");
        }

        private void NavList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (NavList.SelectedItem is ListBoxItem item && item.Tag is string tag)
            {
                _navigationService.NavigateTo(tag);
            }
        }

        private void OnNavigated(string viewName, object? parameter)
        {
            // Ensure navigation runs on UI thread
            Dispatcher.Invoke(() =>
            {
                object? view = CreateViewFor(viewName, parameter);
                if (view != null)
                {
                    MainFrame.Navigate(view);
                }
                else
                {
                    // Fallback placeholder
                    MainFrame.Navigate(new Page { Content = new TextBlock { Text = $"View '{viewName}' not found.", Margin = new Thickness(20) } });
                }
            });
        }

        private object? CreateViewFor(string viewName, object? parameter)
        {
            // Try to map to strongly-typed views if present in assembly
            var asmName = Assembly.GetExecutingAssembly().GetName().Name; // usually "JIDS"
            object? view = null;

            try
            {
                switch (viewName)
                {
                    case "ConfigurationList":
                        view = InstantiateIfExists($"JIDS.Views.ConfigurationListView, {asmName}");
                        // If view exists and DataContext is null try to create ViewModel using available resources
                        if (view is FrameworkElement fe && fe.DataContext == null)
                        {
                            if (Application.Current.Resources["ConfigurationRepository"] is IConfigurationRepository repo
                                && Application.Current.Resources["ConfigurationWriter"] is IConfigurationServiceWriter writer)
                            {
                                var vm = new ConfigurationListViewModel(repo, writer, _navigationService, _userSession);
                                fe.DataContext = vm;
                            }
                            // If parameter is a search string, propagate it to VM (if present)
                            if (parameter is string s && fe.DataContext != null)
                            {
                                try
                                {
                                    var prop = fe.DataContext.GetType().GetProperty("SearchQuery");
                                    prop?.SetValue(fe.DataContext, s);
                                }
                                catch { /* ignore */ }
                            }
                        }
                        break;

                    case "ConfigurationEditor":
                        // Pass parameter to constructor if editor view supports it
                        view = InstantiateIfExists($"JIDS.Views.ConfigurationEditorView, {asmName}", parameter);
                        break;

                    case "Login":
                        view = InstantiateIfExists($"JIDS.Views.LoginView, {asmName}");
                        break;

                    case "Summary":
                        // Only allow Summary when a JetConfiguration is supplied as parameter
                        if (parameter is JetConfiguration cfg)
                        {
                            view = InstantiateIfExists($"JIDS.Views.SummaryView, {asmName}", cfg);
                        }
                        else
                        {
                            // explicit denial: no parameter => do not show summary
                            view = null;
                        }
                        break;

                    default:
                        // attempt to instantiate view with same name + "View"
                        view = InstantiateIfExists($"JIDS.Views.{viewName}View, {asmName}", parameter);
                        break;
                }
            }
            catch
            {
                view = null;
            }

            return view;
        }

        private object? InstantiateIfExists(string typeName, object? parameter = null)
        {
            // Robust type lookup: try Type.GetType, then search loaded assemblies for the type name
            try
            {
                // 1) Try Type.GetType first (works with assembly-qualified names)
                var type = Type.GetType(typeName);
                if (type == null)
                {
                    // Extract the short type name (before comma) and try the executing assembly and all loaded assemblies
                    var shortName = typeName.Split(',')[0].Trim();

                    // Try executing assembly
                    var asm = Assembly.GetExecutingAssembly();
                    type = asm.GetType(shortName);

                    // Try all loaded assemblies
                    if (type == null)
                    {
                        foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            try
                            {
                                type = a.GetType(shortName);
                                if (type != null) break;
                            }
                            catch { /* ignore */ }
                        }
                    }
                }

                if (type == null) return null;

                // If a parameter is provided, attempt to find a matching ctor
                if (parameter != null)
                {
                    // Try exact parameter type first
                    var ctor = type.GetConstructor(new[] { parameter.GetType() });
                    if (ctor != null) return ctor.Invoke(new[] { parameter });

                    // Try a constructor accepting object (fallback)
                    ctor = type.GetConstructor(new[] { typeof(object) });
                    if (ctor != null) return ctor.Invoke(new[] { parameter });
                }

                // Default ctor
                return Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                // Write debug info and fail gracefully so the UI can show the fallback message
                Console.WriteLine($"InstantiateIfExists failed for '{typeName}': {ex}");
                return null;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _navigationService.Navigated -= OnNavigated;
        }
    }
}