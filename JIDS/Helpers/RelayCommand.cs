using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JetInteriorApp.Helpers
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object?>? _executeSync;
        private readonly Func<object?, Task>? _executeAsync;
        private readonly Predicate<object?>? _canExecute;

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _executeSync = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RelayCommand(Func<object?, Task> executeAsync, Predicate<object?>? canExecute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
            => _canExecute?.Invoke(parameter) ?? true;

        // ICommand requires void return type
        public void Execute(object? parameter)
        {
            if (_executeAsync != null)
            {
                // fire-and-forget to keep ICommand happy
                _ = _executeAsync(parameter);
            }
            else
            {
                _executeSync!(parameter);
            }
        }

        // Extra helper: allows tests to await async commands
        public Task? ExecuteAsync(object? parameter)
        {
            return _executeAsync?.Invoke(parameter);
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
