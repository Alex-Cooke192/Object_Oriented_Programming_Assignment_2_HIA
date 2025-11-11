using System;
using System.Threading.Tasks;
using JIDS.Helpers;
using Xunit;

public class RelayCommandTests
{
    private bool _executedFlag;
    private bool _canExecuteFlag;

    public RelayCommandTests()
    {
        _executedFlag = false;
        _canExecuteFlag = true;
    }

    // ----------------------------------------------------
    // Manual Test Runner
    // ----------------------------------------------------
    public async Task RunTestsAsync()
    {
        Console.WriteLine("\nRunning RelayCommand tests...\n");

        await TestExecute_CallsAction();
        await TestCanExecute_ReturnsTrueWhenAllowed();
        await TestCanExecute_ReturnsFalseWhenBlocked();
        await TestExecute_DoesNotFireWhenCannotExecute();
        await TestConstructor_ThrowsOnNull();

        Console.WriteLine("\n✅ All RelayCommand tests completed.");
    }

    // ----------------------------------------------------
    // Test Logic
    // ----------------------------------------------------

    private Task TestExecute_CallsAction()
    {
        try
        {
            var command = new RelayCommand(_ => _executedFlag = true);

            command.Execute(null);

            if (!_executedFlag)
                throw new Exception("Execute did not call action");

            Console.WriteLine("Execute_CallsAction: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Execute_CallsAction: FAILED - {ex.Message}");
            throw;
        }

        return Task.CompletedTask;
    }

    private Task TestCanExecute_ReturnsTrueWhenAllowed()
    {
        try
        {
            var command = new RelayCommand(_ => { }, _ => true);

            if (!command.CanExecute(null))
                throw new Exception("Expected CanExecute to return true");

            Console.WriteLine("CanExecute_ReturnsTrueWhenAllowed: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CanExecute_ReturnsTrueWhenAllowed: FAILED - {ex.Message}");
            throw;
        }

        return Task.CompletedTask;
    }

    private Task TestCanExecute_ReturnsFalseWhenBlocked()
    {
        try
        {
            var command = new RelayCommand(_ => { }, _ => false);

            if (command.CanExecute(null))
                throw new Exception("Expected CanExecute to return false");

            Console.WriteLine("CanExecute_ReturnsFalseWhenBlocked: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CanExecute_ReturnsFalseWhenBlocked: FAILED - {ex.Message}");
            throw;
        }

        return Task.CompletedTask;
    }

    private Task TestExecute_DoesNotFireWhenCannotExecute()
    {
        try
        {
            _executedFlag = false;

            var command = new RelayCommand(_ => _executedFlag = true, _ => false);

            if (command.CanExecute(null))
                throw new Exception("Guard failed: CanExecute returned true unexpectedly");

            // Still calling Execute, but typical MVVM UI won't
            command.Execute(null);

            if (_executedFlag)
                throw new Exception("Execute fired even when CanExecute returned false");

            Console.WriteLine("Execute_DoesNotFireWhenCannotExecute: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Execute_DoesNotFireWhenCannotExecute: FAILED - {ex.Message}");
            throw;
        }

        return Task.CompletedTask;
    }

    private Task TestConstructor_ThrowsOnNull()
    {
        try
        {
            Assert.Throws<ArgumentNullException>(() => new RelayCommand(null!));
            Console.WriteLine("Constructor_ThrowsOnNull: PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Constructor_ThrowsOnNull: FAILED - {ex.Message}");
            throw;
        }

        return Task.CompletedTask;
    }

    // ----------------------------------------------------
    // xUnit Fact wrappers
    // ----------------------------------------------------
    [Fact] public async Task Execute_CallsAction_Fact() => await TestExecute_CallsAction();
    [Fact] public async Task CanExecute_True_Fact() => await TestCanExecute_ReturnsTrueWhenAllowed();
    [Fact] public async Task CanExecute_False_Fact() => await TestCanExecute_ReturnsFalseWhenBlocked();
    [Fact] public async Task Execute_WhenCannotExecute_Fact() => await TestExecute_DoesNotFireWhenCannotExecute();
    [Fact] public async Task Constructor_Throws_Fact() => await TestConstructor_ThrowsOnNull();
}

