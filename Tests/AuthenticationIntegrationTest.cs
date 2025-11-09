using System;
using System.Linq;
using System.Threading.Tasks;
using JetInteriorApp.Data;
using JetInteriorApp.Repositories;
using JetInteriorApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class AuthenticationIntegrationTests
{
    private JetDbContext GetDb()
    {
        return new JetDbContext(
            new DbContextOptionsBuilder<JetDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options
        );
    }

    [Fact]
    public async Task Full_Authentication_Flow_Works()
    {
        // SETUP
        var db = GetDb();
        var repo = new AuthRepository(db);
        var vm = new LoginViewModel(repo);

        //
        // 1. USER REGISTRATION
        //
        vm.Username = "alex";
        vm.Email = "alex@mail.com";
        vm.Password = "Secret123";

        await CallRegisterAsync(vm);
        Assert.Contains("registered successfully", vm.StatusMessage);

        // Verify user stored in DB + hashed password
        var createdUser = db.Users.FirstOrDefault(u => u.Username == "alex");
        Assert.NotNull(createdUser);
        Assert.NotEqual("Secret123", createdUser.PasswordHash);

        //
        // 2. LOGIN FAIL (wrong password)
        //
        vm.Password = "WrongPW";
        await CallLoginAsync(vm);
        Assert.Equal("Invalid username or password.", vm.StatusMessage);

        //
        // 3. LOGIN SUCCESS (correct password)
        //
        vm.Password = "Secret123";
        await CallLoginAsync(vm);
        Assert.Contains("Welcome, alex!", vm.StatusMessage);

        //
        // 4. DUPLICATE REGISTRATION FAILS
        //
        vm.Username = "alex";
        vm.Email = "another@mail.com";
        vm.Password = "123";
        await CallRegisterAsync(vm);

        Assert.Equal("Username already exists.", vm.StatusMessage);
    }

    // -------------------------------------------------------
    // Helper methods — MUST be inside the class
    // -------------------------------------------------------

    private static async Task CallLoginAsync(LoginViewModel vm)
    {
        if (vm.LoginCommand.CanExecute(null))
            vm.LoginCommand.Execute(null);

        // allow async command to run
        await Task.Delay(30);
    }

    private static async Task CallRegisterAsync(LoginViewModel vm)
    {
        if (vm.RegisterCommand.CanExecute(null))
            vm.RegisterCommand.Execute(null);

        await Task.Delay(30);
    }
}
