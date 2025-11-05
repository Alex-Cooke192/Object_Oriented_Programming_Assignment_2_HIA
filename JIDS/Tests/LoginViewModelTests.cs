using System;
using System.Threading.Tasks;
using System.Windows;
using JetInteriorApp.Interfaces;
using JetInteriorApp.ViewModels;
using Moq;
using Xunit;

public class LoginViewModelTests
{
    private readonly Mock<IAuthRepository> _mockRepo;
    private readonly LoginViewModel _vm;

    public LoginViewModelTests()
    {
        _mockRepo = new Mock<IAuthRepository>();

        // Default mock behaviours
        _mockRepo.Setup(r => r.ValidateUserAsync(It.IsAny<string>(), It.IsAny<string>()))
                 .ReturnsAsync(false);
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

        Console.WriteLine("\n✅ All LoginViewModel tests completed.");
    }

    // ----------------------------------------------------
    // Test Logic
    // ----------------------------------------------------

    private async Task TestLogin_Success()
    {
        try
        {
            _mockRepo.Setup(r => r.ValidateUserAsync("testUser", "pw"))
                     .ReturnsAsync(true);

            _vm.Username = "testUser";
            _vm.Password = "pw";

            await InvokeCommand(_vm.LoginCommand);

            if (!_vm.StatusMessage.Contains("Welcome"))
                throw new Exception("Expected status message not found");

            Console.WriteLine("Login_Success: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login_Success: FAILED - {ex.Message}");
            throw;
        }
    }

    private async Task TestLogin_Failure()
    {
        try
        {
            _mockRepo.Setup(r => r.ValidateUserAsync("wrong", "bad"))
                     .ReturnsAsync(false);

            _vm.Username = "wrong";
            _vm.Password = "bad";

            await InvokeCommand(_vm.LoginCommand);

            if (_vm.StatusMessage != "Invalid username or password.")
                throw new Exception("Incorrect failure message");

            Console.WriteLine("Login_Failure: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login_Failure: FAILED - {ex.Message}");
            throw;
        }
    }

    private async Task TestLogin_InvalidInput()
    {
        try
        {
            _vm.Username = "";
            _vm.Password = "";

            await InvokeCommand(_vm.LoginCommand);

            if (_vm.StatusMessage != "Invalid username or password.")
                throw new Exception("Validation did not fail as expected");

            Console.WriteLine("Login_InvalidInput: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login_InvalidInput: FAILED - {ex.Message}");
            throw;
        }
    }

    private async Task TestRegister_Success()
    {
        try
        {
            _mockRepo.Setup(r => r.RegisterUserAsync("newGuy", "mail@mail.com", "pw"))
                     .ReturnsAsync(true);

            _vm.Username = "newGuy";
            _vm.Password = "pw";
            _vm.Email = "mail@mail.com";

            await InvokeCommand(_vm.RegisterCommand);

            if (!_vm.StatusMessage.Contains("registered successfully"))
                throw new Exception("Success message missing");

            Console.WriteLine("Register_Success: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Register_Success: FAILED - {ex.Message}");
            throw;
        }
    }

    private async Task TestRegister_Failure()
    {
        try
        {
            _mockRepo.Setup(r => r.RegisterUserAsync("exists", "mail@mail.com", "pw"))
                     .ReturnsAsync(false);

            _vm.Username = "exists";
            _vm.Password = "pw";
            _vm.Email = "mail@mail.com";

            await InvokeCommand(_vm.RegisterCommand);

            if (_vm.StatusMessage != "Username already exists.")
                throw new Exception("Incorrect failure message");

            Console.WriteLine("Register_Failure: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Register_Failure: FAILED - {ex.Message}");
            throw;
        }
    }

    private async Task TestRegister_NoEmail()
    {
        try
        {
            _vm.Username = "user";
            _vm.Password = "pw";
            _vm.Email = "";

            await InvokeCommand(_vm.RegisterCommand);

            if (_vm.StatusMessage != "Email is required for registration.")
                throw new Exception("Missing email was not detected");

            Console.WriteLine("Register_NoEmail: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Register_NoEmail: FAILED - {ex.Message}");
            throw;
        }
    }

    // Utility: execute commands that are async
    private async Task InvokeCommand(System.Windows.Input.ICommand command)
    {
        if (command.CanExecute(null))
        {
            var task = command.Execute(null) as Task;
            if (task != null) await task;
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
