using System;
using JIDS.Interfaces;

namespace JIDS.Services
{
    /// <summary>
    /// Raises an event for navigation requests. UI layer (Window / App) should subscribe and perform navigation.
    /// Keeps ViewModels decoupled from WPF types.
    /// </summary>
    public class NavigationService : INavigationService
    {
        public event Action<string, object?>? Navigated;

        public void NavigateTo(string viewName) => Navigated?.Invoke(viewName, null);

        public void NavigateTo(string viewName, object? parameter) => Navigated?.Invoke(viewName, parameter);
    }
}