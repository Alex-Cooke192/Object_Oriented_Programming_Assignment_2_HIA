using System;
using BCrypt.Net;

namespace JetInteriorApp.Services
{
    /// <summary>
    /// Provides secure password hashing and verification using BCrypt.
    /// </summary>
    internal static class PasswordHasher
    {
        /// <summary>
        /// Hashes a plain text password using BCrypt.
        /// </summary>
        /// <param name="plainPassword">The plain text password to hash.</param>
        /// <returns>A securely hashed password string.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the password is null or empty.</exception>
        public static string HashPassword(string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new ArgumentNullException(nameof(plainPassword), "Password cannot be null or empty.");

            // BCrypt automatically handles salt generation internally.
            return BCrypt.Net.BCrypt.HashPassword(plainPassword);
        }

        /// <summary>
        /// Verifies a plain text password against a previously hashed password.
        /// </summary>
        /// <param name="plainPassword">The plain text password to check.</param>
        /// <param name="hashedPassword">The stored hashed password to compare against.</param>
        /// <returns>True if the password matches the hash; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either argument is null or empty.</exception>
        public static bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new ArgumentNullException(nameof(plainPassword), "Password cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(hashedPassword))
                throw new ArgumentNullException(nameof(hashedPassword), "Hashed password cannot be null or empty.");

            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }
    }
}