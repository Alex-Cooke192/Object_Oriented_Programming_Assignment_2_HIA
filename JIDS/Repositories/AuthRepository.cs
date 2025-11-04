using System;
using System.Threading.Tasks;
using JetInteriorApp.Data;
using JetInteriorApp.Interfaces;
using JetInteriorApp.Models;
using JetInteriorApp.Services;
using Microsoft.EntityFrameworkCore;

namespace JetInteriorApp.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly JetDbContext _context;

        public AuthRepository(JetDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            var userDb = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (userDb == null)
                return false;

            // Secure BCrypt verification
            return PasswordHasher.VerifyPassword(password, userDb.PasswordHash);
        }

        public async Task<bool> RegisterUserAsync(string username, string email, string password)
        {
            var existingUserDb = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (existingUserDb != null)
                return false; // Username already exists

            // Hash password before storing
            var hashedPassword = PasswordHasher.HashPassword(password);

            var userDb = new UserDB
            {
                UserID = Guid.NewGuid(),
                Username = username,
                Email = email,
                PasswordHash = hashedPassword
            };

            _context.Users.Add(userDb);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
