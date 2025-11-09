using System;
using System.ComponentModel;
using JIDS.Interfaces;
using JIDS.Models;

namespace JIDS.Services
{
    /// <summary>
    /// Simple in-memory session. Exposes events for UI to react.
    /// Keep as singleton in DI.
    /// </summary>
    public class UserSessionService : IUserSessionService, INotifyPropertyChanged
    {
        private User? _currentUser;

        // Return the GUID from the domain user (or Guid.Empty when not authenticated)
        public Guid CurrentUserId => _currentUser?.UserID ?? Guid.Empty;

        public User? CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser != null;

        public event Action? SessionChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public void SetCurrentUser(User user)
        {
            _currentUser = user;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentUser)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentUserId)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAuthenticated)));
            SessionChanged?.Invoke();
        }

        public void Clear()
        {
            _currentUser = null;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentUser)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentUserId)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAuthenticated)));
            SessionChanged?.Invoke();
        }
    }
}