using System;
using System.Windows.Input;

namespace Parcel.Neo.Base.Framework.ViewModels
{
    public interface ICommandManager
    {
        public void AddEvent(EventHandler e);
        public void RemoveEvent(EventHandler e);
    }
    public class RequeryCommand : IProcessorNodeCommand
    {
        public static ICommandManager CommandManager;
        private readonly Action _action;
        private readonly Func<bool> _condition;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager?.AddEvent(value);
            remove => CommandManager?.RemoveEvent(value);
        }

        public RequeryCommand(Action action, Func<bool> executeCondition = default)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _condition = executeCondition;
        }

        public bool CanExecute(object parameter)
            => _condition?.Invoke() ?? true;

        public void Execute(object parameter)
            => _action();

        public void RaiseCanExecuteChanged() { }
    }

    public class RequeryCommand<T> : IProcessorNodeCommand
    {
        public static ICommandManager CommandManager => RequeryCommand.CommandManager;
        private readonly Action<T> _action;
        private readonly Func<T, bool> _condition;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager?.AddEvent(value);
            remove => CommandManager?.RemoveEvent(value);
        }

        public RequeryCommand(Action<T> action, Func<T, bool> executeCondition = default)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _condition = executeCondition;
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is T value)
            {
                return _condition?.Invoke(value) ?? true;
            }

            return _condition?.Invoke(default!) ?? true;
        }

        public void Execute(object parameter)
        {
            if (parameter is T value)
            {
                _action(value);
            }
            else
            {
                _action(default!);
            }
        }

        public void RaiseCanExecuteChanged() { }
    }
}
