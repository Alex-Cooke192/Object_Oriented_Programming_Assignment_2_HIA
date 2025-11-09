using JetInteriorApp.Data;
using JetInteriorApp.Helpers;
using JetInteriorApp.Interfaces;
using JetInteriorApp.Models;
using JetInteriorApp.ViewModels;
using Moq;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xunit;

[Collection("STA Tests")]
public class LoginViewModelTests
{
    private readonly Mock<IAuthRepository> _mockRepo;
    private readonly LoginViewModel _vm;

    public LoginViewModelTests()
    {
        _mockRepo = new Mock<IAuthRepository>();

        // Default mock behaviours now return null, not false
        _mockRepo.Setup(r => r.ValidateUserAsync(It.IsAny<string>(), It.IsAny<string>()))
                 .ReturnsAsync((UserDB?)null);

        _mockRepo.Setup(r => r.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                 .ReturnsAsync(false);

        _vm = new LoginViewModel(_mockRepo.Object);
    }

    // ----------------------------------------------------
    // Manual Test Runner
    // ----------------------------------------------------
    public async Task RunTestsAsync()
    {
        Console.WriteLine("\nRunning LoginViewModel tests...\n");

        await TestLogin_Success();
        await TestLogin_Failure();
        await TestLogin_InvalidInput();

        await TestRegister_Success();
        await TestRegister_Failure();
        await TestRegister_NoEmail();

        Console.WriteLine("\nAll LoginViewModel tests completed.");
    }

    // ----------------------------------------------------
    // Test Logic
    // ----------------------------------------------------

    private async Task TestLogin_Success()
    {
        // Return a UserDB object for valid login
        _mockRepo.Setup(r => r.ValidateUserAsync("testUser", "pw"))
                 .ReturnsAsync(new UserDB
                 {
                     UserID = Guid.NewGuid(),
                     Username = "testUser",
                     Email = "test@mail.com"
                 });

        _vm.Username = "testUser";
        _vm.Password = "pw";

        await InvokeCommand(_vm.LoginCommand);

        if (!_vm.StatusMessage.Contains("Welcome"))
            throw new Exception($"Expected 'Welcome', got '{_vm.StatusMessage}'");

        Console.WriteLine("Login_Success: PASSED");
    }

    private async Task TestLogin_Failure()
    {
        // Force NULL result for incorrect user
        _mockRepo.Setup(r => r.ValidateUserAsync("wrong", "bad"))
                 .ReturnsAsync((UserDB?)null);

        _vm.Username = "wrong";
        _vm.Password = "bad";

        await InvokeCommand(_vm.LoginCommand);

        if (_vm.StatusMessage != "Invalid username or password.")
            throw new Exception($"Expected failure message, got '{_vm.StatusMessage}'");

        Console.WriteLine("Login_Failure: PASSED");
    }

    private async Task TestLogin_InvalidInput()
    {
        _vm.Username = "";
        _vm.Password = "";

        await InvokeCommand(_vm.LoginCommand);

        if (_vm.StatusMessage != "Invalid username or password.")
            throw new Exception($"Invalid input did not trigger error: {_vm.StatusMessage}");

        Console.WriteLine("Login_InvalidInput: PASSED");
    }

    private async Task TestRegister_Success()
    {
        _mockRepo.Setup(r => r.RegisterUserAsync("newGuy", "mail@mail.com", "pw"))
                 .ReturnsAsync(true);

        _vm.Username = "newGuy";
        _vm.Password = "pw";
        _vm.Email = "mail@mail.com";

        await InvokeCommand(_vm.RegisterCommand);

        if (!_vm.StatusMessage.Contains("registered successfully"))
            throw new Exception($"Expected success message, got '{_vm.StatusMessage}'");

        Console.WriteLine("Register_Success: PASSED");
    }

    private async Task TestRegister_Failure()
    {
        _mockRepo.Setup(r => r.RegisterUserAsync("exists", "mail@mail.com", "pw"))
                 .ReturnsAsync(false);

        _vm.Username = "exists";
        _vm.Password = "pw";
        _vm.Email = "mail@mail.com";

        await InvokeCommand(_vm.RegisterCommand);

        if (_vm.StatusMessage != "Username already exists.")
            throw new Exception($"Expected failure message, got '{_vm.StatusMessage}'");

        Console.WriteLine("Register_Failure: PASSED");
    }

    private async Task TestRegister_NoEmail()
    {
        _vm.Username = "user";
        _vm.Password = "pw";
        _vm.Email = "";

        await InvokeCommand(_vm.RegisterCommand);

        if (_vm.StatusMessage != "Email is required for registration.")
            throw new Exception($"Expected missing-email message, got '{_vm.StatusMessage}'");

        Console.WriteLine("Register_NoEmail: PASSED");
    }

    // Utility: async command executor
    private async Task InvokeCommand(ICommand command)
    {
        if (!command.CanExecute(null))
            return;

        if (command is RelayCommand relay && relay.ExecuteAsync != null)
        {
            var task = relay.ExecuteAsync(null);
            if (task != null)
                await task;
        }
        else
        {
            command.Execute(null);
        }
    }

    // ----------------------------------------------------
    // xUnit Test Wrappers
    // ----------------------------------------------------
    [Fact] public async Task Login_Success_Fact() => await TestLogin_Success();
    [Fact] public async Task Login_Failure_Fact() => await TestLogin_Failure();
    [Fact] public async Task Login_InvalidInput_Fact() => await TestLogin_InvalidInput();

    [Fact] public async Task Register_Success_Fact() => await TestRegister_Success();
    [Fact] public async Task Register_Failure_Fact() => await TestRegister_Failure();
    [Fact] public async Task Register_NoEmail_Fact() => await TestRegister_NoEmail();
}
