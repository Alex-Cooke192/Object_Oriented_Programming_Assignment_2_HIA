using JIDS.Services;
using System;
using System.Collections.Generic;

namespace JIDS.Models
{
    public class User
    {
        public Guid UserID { get; private set; }
        public string Username { get; private set; }
        public string PasswordHash { get; private set; }
        public string Email { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public ICollection<JetConfiguration> Configurations { get; private set; }

        // Private constructor to force use of factory methods
        private User(Guid userID, string username, string email, string passwordHash, DateTime createdAt)
        {
            UserID = userID;
            Username = username;
            PasswordHash = passwordHash;
            Email = email;
            CreatedAt = createdAt;
            Configurations = new List<JetConfiguration>();
        }

        // Factory method for creating a NEW user
        public static User CreateNew(string username, string email, string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty.");
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.");
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new ArgumentException("Password cannot be empty.");

            var hashedPassword = PasswordHasher.HashPassword(plainPassword);
            return new User(Guid.NewGuid(), username, email, hashedPassword, DateTime.Now);
        }

        // Factory method for loading an EXISTING user (from DB)
        public static User FromExisting(Guid userID, string username, string email, string passwordHash, DateTime createdAt)
        {
            return new User(userID, username, email, passwordHash, createdAt);
        }
    }
}

