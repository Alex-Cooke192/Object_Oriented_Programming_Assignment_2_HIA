using System;
using System.Threading.Tasks;
using JIDS.Services;
using Xunit;

public class PasswordHasherTests
{
    // ----------------------------------------------------
    // Manual Test Runner
    // ----------------------------------------------------
    public async Task RunTestsAsync()
    {
        Console.WriteLine("\nRunning PasswordHasher tests...\n");

        await TestHashPassword_ReturnsHashedValue();
        await TestHashPassword_ThrowsOnNullOrEmpty();

        await TestVerifyPassword_Valid();
        await TestVerifyPassword_Invalid();
        await TestVerifyPassword_ThrowsOnInvalidArgs();

        Console.WriteLine("\nAll PasswordHasher tests completed.");
    }

    // ----------------------------------------------------
    // Test Logic
    // ----------------------------------------------------

    private Task TestHashPassword_ReturnsHashedValue()
    {
        try
        {
            string plain = "Secret123";
            string hashed = PasswordHasher.HashPassword(plain);

            if (string.IsNullOrWhiteSpace(hashed))
                throw new Exception("Hashed password is null/empty");

            if (hashed == plain)
                throw new Exception("Hash should not equal plain text");

            Console.WriteLine("HashPassword_ReturnsHashedValue: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"HashPassword_ReturnsHashedValue: FAILED - {ex.Message}");
            throw;
        }

        return Task.CompletedTask;
    }

    private Task TestHashPassword_ThrowsOnNullOrEmpty()
    {
        try
        {
            Assert.Throws<ArgumentNullException>(() => PasswordHasher.HashPassword(null!));
            Assert.Throws<ArgumentNullException>(() => PasswordHasher.HashPassword(""));
            Assert.Throws<ArgumentNullException>(() => PasswordHasher.HashPassword(" "));

            Console.WriteLine("HashPassword_ThrowsOnNullOrEmpty: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"HashPassword_ThrowsOnNullOrEmpty: FAILED - {ex.Message}");
            throw;
        }

        return Task.CompletedTask;
    }

    private Task TestVerifyPassword_Valid()
    {
        try
        {
            string plain = "MyPass123";
            string hashed = PasswordHasher.HashPassword(plain);

            bool result = PasswordHasher.VerifyPassword(plain, hashed);
            if (!result)
                throw new Exception("Expected true, got false");

            Console.WriteLine("VerifyPassword_Valid: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"VerifyPassword_Valid: FAILED - {ex.Message}");
            throw;
        }

        return Task.CompletedTask;
    }

    private Task TestVerifyPassword_Invalid()
    {
        try
        {
            string plain = "RealPassword";
            string hashed = PasswordHasher.HashPassword("DifferentPassword");

            bool result = PasswordHasher.VerifyPassword(plain, hashed);
            if (result)
                throw new Exception("Expected false, got true");

            Console.WriteLine("VerifyPassword_Invalid: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"VerifyPassword_Invalid: FAILED - {ex.Message}");
            throw;
        }

        return Task.CompletedTask;
    }

    private Task TestVerifyPassword_ThrowsOnInvalidArgs()
    {
        try
        {
            string validHash = PasswordHasher.HashPassword("abc");

            // plain null/empty
            Assert.Throws<ArgumentNullException>(() => PasswordHasher.VerifyPassword(null!, validHash));
            Assert.Throws<ArgumentNullException>(() => PasswordHasher.VerifyPassword("", validHash));
            Assert.Throws<ArgumentNullException>(() => PasswordHasher.VerifyPassword(" ", validHash));

            // hashed null/empty
            Assert.Throws<ArgumentNullException>(() => PasswordHasher.VerifyPassword("abc", null!));
            Assert.Throws<ArgumentNullException>(() => PasswordHasher.VerifyPassword("abc", ""));
            Assert.Throws<ArgumentNullException>(() => PasswordHasher.VerifyPassword("abc", " "));

            Console.WriteLine("VerifyPassword_ThrowsOnInvalidArgs: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"VerifyPassword_ThrowsOnInvalidArgs: FAILED - {ex.Message}");
            throw;
        }

        return Task.CompletedTask;
    }

    // ----------------------------------------------------
    // xUnit Fact wrappers
    // ----------------------------------------------------
    [Fact] public async Task HashPassword_ReturnsHashedValue_Fact() => await TestHashPassword_ReturnsHashedValue();
    [Fact] public async Task HashPassword_ThrowsOnNullOrEmpty_Fact() => await TestHashPassword_ThrowsOnNullOrEmpty();
    [Fact] public async Task VerifyPassword_Valid_Fact() => await TestVerifyPassword_Valid();
    [Fact] public async Task VerifyPassword_Invalid_Fact() => await TestVerifyPassword_Invalid();
    [Fact] public async Task VerifyPassword_ThrowsOnInvalidArgs_Fact() => await TestVerifyPassword_ThrowsOnInvalidArgs();
}

