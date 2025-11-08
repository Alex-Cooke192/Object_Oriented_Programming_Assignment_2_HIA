using System;
using System.ComponentModel;
using JetInteriorApp.Interfaces;
using JetInteriorApp.Models;

namespace JetInteriorApp.Services
{
    /// <summary>
    /// Simple in-memory session. Exposes events for UI to react.
    /// Keep as singleton in DI.
    /// </summary>
    public class UserSessionService : IUserSessionService, INotifyPropertyChanged
    {
        private User? _currentUser;

        public Guid CurrentUserId => _currentUser?.UserID is int id ? new Guid(id, 0, 0, new byte[12]) : Guid.Empty;
        // Note: your User model currently uses int UserID — adapt to Guid if you standardize on Guid.
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