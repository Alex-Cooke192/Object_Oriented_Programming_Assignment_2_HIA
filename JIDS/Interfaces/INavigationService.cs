using System;

namespace JIDS.Interfaces
{
    /// <summary>
    /// Small, testable navigation contract.
    /// View layer subscribes to <see cref="Navigated"/> and performs WPF navigation.
    /// </summary>
    public interface INavigationService
    {
        // Raised when a navigation request is issued. Parameter may be null.
        event Action<string, object?>? Navigated;

        void NavigateTo(string viewName);
        void NavigateTo(string viewName, object? parameter);
    }
}