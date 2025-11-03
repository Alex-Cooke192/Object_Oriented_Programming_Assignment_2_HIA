using System;
using BCrypt.Net;

namespace JetInteriorApp.Models
{
    public class User
    {
        // Basic user fields
        public int UserID { get; set; } // You can set manually for testing
        public string Username { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }

        // Password storage (hashed)
        public string PasswordHash { get; set; }

        // Method to set password securely
        public void SetPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty");

            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Method to verify password
        public bool VerifyPassword(string password)
        {
            if (string.IsNullOrEmpty(PasswordHash))
                return false;

            return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
        }

        // For debugging/testing purposes
        public override string ToString()
        {
            return $"UserID: {UserID}, Username: {Username}, Email: {Email}, CreatedAt: {CreatedAt}";
        }
    }
}

/*
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BCrypt.Net; // Need to install BCrypt.Net-Next NuGet package for hashing

namespace JetInteriorApp.Models
{
    public class User
    {
        [Key]
        public Guid UserID { get; private set; }

        [Required]
        public string Username { get; private set; }

        [Required]
        public string PasswordHash { get; private set; }

        [Required]
        public string Email { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public ICollection<JetConfiguration> Configurations { get; private set; } = new List<JetConfiguration>();


        // Default constructor for EF or deserialization
        private User() { }

        // Public factory-style constructor (for creating new users in your app)
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
            Email = email;
            CreatedAt = DateTime.UtcNow;

            SetPassword(plainPassword);
        }

        // Encapsulated password hashing logic
        public void SetPassword(string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new ArgumentException("Password cannot be empty.");

            PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
        }

        // Password verification logic (used during login)
        public bool VerifyPassword(string plainPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, PasswordHash);
        }
    }
}
*/