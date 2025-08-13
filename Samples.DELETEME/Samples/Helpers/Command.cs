using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Xamarin.Essentials
{
    //
    // Summary:
    //     Defines an System.Windows.Input.ICommand implementation wrapping a generic Action<T>.
    //
    // Type parameters:
    //   T:
    //     The Type of the parameter,
    //
    // Remarks:
    //     The following example creates a new Command and sets it to a button.
    public sealed class Command<T> : Command
    {
        public Command(Action<T?>? execute)
            : base(delegate (object? o)
            {
                if (IsValidParameter(o))
                {
                    execute?.Invoke((T?)o);
                }
            })
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }
        }

        public Command(Action<T?>? execute, Func<T?, bool> canExecute)
            : base(
                delegate (object? o)
            {
                if (IsValidParameter(o))
                {
                    execute?.Invoke((T?)o);
                }
            }, (object? o) => IsValidParameter(o) && canExecute.Invoke((T?)o))
        {
            ArgumentNullException.ThrowIfNull(execute, nameof(execute));
            ArgumentNullException.ThrowIfNull(canExecute, nameof(canExecute));
        }

        private static bool IsValidParameter(object? o)
        {
            if (o != null)
                return o is T;

            var typeFromHandle = typeof(T);
            if (Nullable.GetUnderlyingType(typeFromHandle) != null)
                return true;

            return !typeFromHandle.GetType().IsValueType;
        }
    }

    public class Command : ICommand
    {
        private readonly Func<object?, bool>? _canExecute;

        private readonly Action<object?> _execute;

        private readonly WeakEventManager _weakEventManager = new();

        // Summary:
        //     Occurs when the target of the Command should reevaluate whether or not the Command
        //     can be executed.
        //
        // Remarks:
        //     To be added.
        public event EventHandler? CanExecuteChanged
        {
            add
            {
                _weakEventManager.AddEventHandler(value, nameof(CanExecuteChanged));
            }

            remove
            {
                _weakEventManager.RemoveEventHandler(value, nameof(CanExecuteChanged));
            }
        }

        public Command(Action<object?> execute)
        {
            ArgumentNullException.ThrowIfNull(execute, nameof(execute));
            _execute = execute;
        }

        public Command(Action execute)
            : this((Action<object?>)delegate
            {
                execute?.Invoke();
            })
        {
            ArgumentNullException.ThrowIfNull(execute, nameof(execute));
        }

        public Command(Action<object?> execute, Func<object?, bool> canExecute)
            : this(execute)
        {
            ArgumentNullException.ThrowIfNull(canExecute, nameof(canExecute));
            _canExecute = canExecute;
        }

        public Command(Action? execute, Func<bool> canExecute)
            : this(
                delegate
            {
                execute?.Invoke();
            }, (object? o) => canExecute())
        {
            ArgumentNullException.ThrowIfNull(execute, nameof(execute));
            ArgumentNullException.ThrowIfNull(canExecute, nameof(canExecute));
        }

        //
        // Summary:
        //     Returns a System.Boolean indicating if the Command can be exectued with the given
        //     parameter.
        //
        // Parameters:
        //   parameter:
        //     An System.Object used as parameter to determine if the Command can be executed.
        //
        // Returns:
        //     true if the Command can be executed, false otherwise.
        //
        // Remarks:
        //     If no canExecute parameter was passed to the Command constructor, this method
        //     always returns true.
        //     If the Command was created with non-generic execute parameter, the parameter
        //     of this method is ignored.
        public bool CanExecute(object? parameter)
        {
            if (_canExecute != null)
            {
                return _canExecute(parameter);
            }

            return true;
        }

        //
        // Summary:
        //     Invokes the execute Action
        //
        // Parameters:
        //   parameter:
        //     An System.Object used as parameter for the execute Action.
        //
        // Remarks:
        //     If the Command was created with non-generic execute parameter, the parameter
        //     of this method is ignored.
        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        //
        // Summary:
        //     Send a System.Windows.Input.ICommand.CanExecuteChanged
        //
        // Remarks:
        //     To be added.
        public void ChangeCanExecute()
        {
            _weakEventManager.HandleEvent(this, EventArgs.Empty, "CanExecuteChanged");
        }
    }
}
