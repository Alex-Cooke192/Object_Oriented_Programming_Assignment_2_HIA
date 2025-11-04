using JetInteriorApp.Data;
using JetInteriorApp.Models;
using JetInteriorApp.Services;
using JetInteriorApp.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace JetInteriorApp.Repositories
{
    internal class AuthRepository : IAuthRepository
    {
        private readonly JetDbContext _context;

        public AuthRepository(JetDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a user from the database by username.
        /// Uses FromExisting to preserve UserID and hashed password.
        /// </summary>
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            var userDb = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (userDb == null)
                return null;

            return User.FromExisting(
                userDb.UserID,
                userDb.Username,
                userDb.Email,
                userDb.PasswordHash,
                userDb.CreatedAt
            );
        }

        /// <summary>
        /// Validates user login by comparing plaintext password with stored hash.
        /// </summary>
        public async Task<bool> ValidateUserAsync(string username, string plainPassword)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null)
                return false;

            return PasswordHasher.VerifyPassword(plainPassword, user.PasswordHash);
        }

        /// <summary>
        /// Registers a new user in the database.
        /// Uses User.CreateNew to generate ID, hash password, and set CreatedAt.
        /// </summary>
        public async Task<bool> RegisterUserAsync(string username, string email, string plainPassword)
        {
            // Check if username already exists
            if (await _context.Users.AnyAsync(u => u.Username == username))
                return false;

            // Create domain user using factory method
            var newUser = User.CreateNew(username, email, plainPassword);

            // Map to persistence model
            var newUserDb = new UserDB
            {
                UserID = newUser.UserID,
                Username = newUser.Username,
                Email = newUser.Email,
                PasswordHash = newUser.PasswordHash,
                CreatedAt = newUser.CreatedAt
            };

            // Add to database and save
            _context.Users.Add(newUserDb);
            await _context.SaveChangesAsync();

            Console.WriteLine($"New user saved with username: {username}");

            return true;
        }
    }
}
