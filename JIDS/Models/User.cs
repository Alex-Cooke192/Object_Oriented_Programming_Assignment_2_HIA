using System;
using BCrypt.Net;

namespace JIDS.Models
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
