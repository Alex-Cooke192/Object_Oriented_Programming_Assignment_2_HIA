using System;
using JIDS.Models;

namespace JIDS.Interfaces
{
    /// <summary>
    /// Session service for the current user.
    /// Singleton in DI. Notifies consumers when session state changes.
    /// </summary>
    public interface IUserSessionService
    {
        Guid CurrentUserId { get; }
        User? CurrentUser { get; }
        bool IsAuthenticated { get; }

        event Action? SessionChanged;

        void SetCurrentUser(User user);
        void Clear();
    }
}