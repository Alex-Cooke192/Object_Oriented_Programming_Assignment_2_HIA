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

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            var DesiredUserDB = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (DesiredUserDB == null)
                return null;

            return new User(
                DesiredUserDB.Username, 
                DesiredUserDB.Email, 
                DesiredUserDB.PasswordHash);
        }

        public async Task<bool> ValidateUserAsync(string username, string plainPassword)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null)
                return false;

            return PasswordHasher.VerifyPassword(plainPassword, user.PasswordHash);
        }

        public async Task<bool> RegisterUserAsync(string username, string email, string plainPassword)
        {
            // Check for existing username
            if (await _context.Users.AnyAsync(u => u.Username == username))
                return false;

            // Hash password
            var hashedPassword = PasswordHasher.HashPassword(plainPassword);

            // Create persistence model for Db 
            var newUserDb = new UserDB
            {
                UserID = Guid.NewGuid(),
                Username = username,
                Email = email,
                PasswordHash = hashedPassword  // always store hashed password
            };

            // Add to DB and save
            _context.Users.Add(newUserDb);
            await _context.SaveChangesAsync();

            Console.WriteLine($"New user saved with username: {username}"); 

            return true;
        }
    }
}
