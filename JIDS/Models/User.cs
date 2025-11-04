using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BCrypt.Net; // Need to install BCrypt.Net-Next NuGet package for hashing

namespace JetInteriorApp.Models
{
    public class User
    {
        public Guid UserID { get; private set; }
        public string Username { get; private set; }
        public string PasswordHash { get; private set; }
        public string Email { get; private set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<JetConfiguration> Configurations { get; set; }

        public User(string username, string email, string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty.");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.");

            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new ArgumentException("Password cannot be empty.");
            
            UserID = Guid.NewGuid();
            Username = username;
            PasswordHash = plainPassword;
            Email = email;
            CreatedAt = DateTime.Now;
            Configurations = new List<JetConfiguration>();

        }
    }
}
     