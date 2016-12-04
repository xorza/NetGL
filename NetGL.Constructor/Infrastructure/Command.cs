using System;
using System.Windows.Input;

namespace NetGL.Constructor.Infrastructure {
    internal class Command : ICommand {
        private readonly Action<object> _action;
        private bool _isEnabled = true;

        public bool IsEnabled {
            get { return _isEnabled; }
            set {
                if (_isEnabled != value)
                    return;

                _isEnabled = value;
                if (CanExecuteChanged != null)
                    CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler CanExecuteChanged;

        public Command(Action action) {
            this._action = new Action<object>((obj) => { action(); });
            IsEnabled = true;
        }
        public Command(Action<object> action) {
            this._action = action;
            IsEnabled = true;
        }

        bool ICommand.CanExecute(object parameter) {
            return IsEnabled;
        }
        void ICommand.Execute(object parameter) {
            _action(parameter);
        }
    }
}