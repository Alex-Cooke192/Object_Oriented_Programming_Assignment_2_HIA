using System;
using System.Threading.Tasks;
using JIDS.Data;
using JIDS.Models;
using JIDS.Repositories;
using JIDS.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class AuthRepositoryTests
{
    private AuthRepository _repo;
    private JetDbContext _context;

    public AuthRepositoryTests()
    {
        _context = new JetDbContext(
            new DbContextOptionsBuilder<JetDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options
        );

        _repo = new AuthRepository(_context);
    }

    // ----------------------------------------------------
    // Manual Test Runner
    // ----------------------------------------------------
    public async Task RunTestsAsync()
    {
        Console.WriteLine("\nRunning AuthRepository tests...\n");

        await TestValidateUser_ValidCredentials();
        await TestValidateUser_IncorrectPassword();
        await TestValidateUser_UnknownUser();
        await TestValidateUser_InvalidInputs();

        await TestRegisterUser_Valid();
        await TestRegisterUser_UsernameExists();
        await TestRegisterUser_InvalidInputs();
        await TestRegister_AssignsGuid();

        Console.WriteLine("\nAll AuthRepository tests completed.");
    }

    // ----------------------------------------------------
    // Internal Test Logic
    // ----------------------------------------------------

    private async Task TestValidateUser_ValidCredentials()
    {
        try
        {
            var hashed = PasswordHasher.HashPassword("Correct123");
            _context.Users.Add(new UserDB
            {
                UserID = Guid.NewGuid(),
                Username = "validUser",
                Email = "valid@mail.com",
                PasswordHash = hashed
            });
            await _context.SaveChangesAsync();

            var user = await _repo.ValidateUserAsync("validUser", "Correct123");

            if (user == null || user.Username != "validUser")
                throw new Exception("Expected valid user returned null");

            Console.WriteLine("ValidateUser_ValidCredentials: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ValidateUser_ValidCredentials: FAILED - {ex.Message}");
            throw;
        }
    }

    private async Task TestValidateUser_IncorrectPassword()
    {
        try
        {
            var hashed = PasswordHasher.HashPassword("CorrectPW");
            _context.Users.Add(new UserDB
            {
                UserID = Guid.NewGuid(),
                Username = "wrongPassUser",
                Email = "test@mail.com",
                PasswordHash = hashed
            });
            await _context.SaveChangesAsync();

            var result = await _repo.ValidateUserAsync("wrongPassUser", "BADPW");

            if (result != null)
                throw new Exception("Expected null, got a user");

            Console.WriteLine("ValidateUser_IncorrectPassword: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ValidateUser_IncorrectPassword: FAILED - {ex.Message}");
            throw;
        }
    }

    private async Task TestValidateUser_UnknownUser()
    {
        try
        {
            var result = await _repo.ValidateUserAsync("ghostUser", "anything");

            if (result != null)
                throw new Exception("Expected null, got a user");

            Console.WriteLine("ValidateUser_UnknownUser: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ValidateUser_UnknownUser: FAILED - {ex.Message}");
            throw;
        }
    }

    private async Task TestValidateUser_InvalidInputs()
    {
        try
        {
            var r1 = await _repo.ValidateUserAsync(null, "test");
            var r2 = await _repo.ValidateUserAsync("", "test");
            var r3 = await _repo.ValidateUserAsync("user", null);
            var r4 = await _repo.ValidateUserAsync("user", "");

            if (r1 != null || r2 != null || r3 != null || r4 != null)
                throw new Exception("Invalid input returned a user");

            Console.WriteLine("ValidateUser_InvalidInputs: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ValidateUser_InvalidInputs: FAILED - {ex.Message}");
            throw;
        }
    }

    private async Task TestRegisterUser_Valid()
    {
        try
        {
            var result = await _repo.RegisterUserAsync("newUser", "new@mail.com", "Secret123");
            if (!result) throw new Exception("Expected true, got false");

            Console.WriteLine("RegisterUser_Valid: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"RegisterUser_Valid: FAILED - {ex.Message}");
            throw;
        }
    }

    private async Task TestRegisterUser_UsernameExists()
    {
        try
        {
            _context.Users.Add(new UserDB
            {
                UserID = Guid.NewGuid(),
                Username = "existsUser",
                Email = "exists@mail.com",
                PasswordHash = PasswordHasher.HashPassword("OldPW")
            });
            await _context.SaveChangesAsync();

            var result = await _repo.RegisterUserAsync("existsUser", "test@mail.com", "NewPW");

            if (result)
                throw new Exception("Expected false, got true");

            Console.WriteLine("RegisterUser_UsernameExists: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"RegisterUser_UsernameExists: FAILED - {ex.Message}");
            throw;
        }
    }

    private async Task TestRegisterUser_InvalidInputs()
    {
        try
        {
            var r1 = await _repo.RegisterUserAsync(null, "e@mail.com", "pw");
            var r2 = await _repo.RegisterUserAsync("", "e@mail.com", "pw");
            var r3 = await _repo.RegisterUserAsync("user", null, "pw");
            var r4 = await _repo.RegisterUserAsync("user", "", "pw");
            var r5 = await _repo.RegisterUserAsync("user", "e@mail.com", null);
            var r6 = await _repo.RegisterUserAsync("user", "e@mail.com", "");

            if (r1 || r2 || r3 || r4 || r5 || r6)
                throw new Exception("Invalid input returned true");

            Console.WriteLine("RegisterUser_InvalidInputs: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"RegisterUser_InvalidInputs: FAILED - {ex.Message}");
            throw;
        }
    }

    private async Task TestRegister_AssignsGuid()
    {
        try
        {
            await _repo.RegisterUserAsync("guidUser", "g@mail.com", "PW1");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == "guidUser");

            if (user == null || user.UserID == Guid.Empty)
                throw new Exception("UserID was not assigned");

            Console.WriteLine("Register_AssignsGuid: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Register_AssignsGuid: FAILED - {ex.Message}");
            throw;
        }
    }

    // ----------------------------------------------------
    // xUnit Test Wrappers
    // ----------------------------------------------------
    [Fact] public async Task ValidateUser_ValidCredentials_Fact() => await TestValidateUser_ValidCredentials();
    [Fact] public async Task ValidateUser_IncorrectPassword_Fact() => await TestValidateUser_IncorrectPassword();
    [Fact] public async Task ValidateUser_UnknownUser_Fact() => await TestValidateUser_UnknownUser();
    [Fact] public async Task ValidateUser_InvalidInputs_Fact() => await TestValidateUser_InvalidInputs();

    [Fact] public async Task RegisterUser_Valid_Fact() => await TestRegisterUser_Valid();
    [Fact] public async Task RegisterUser_UsernameExists_Fact() => await TestRegisterUser_UsernameExists();
    [Fact] public async Task RegisterUser_InvalidInputs_Fact() => await TestRegisterUser_InvalidInputs();
    [Fact] public async Task Register_AssignsGuid_Fact() => await TestRegister_AssignsGuid();
}
