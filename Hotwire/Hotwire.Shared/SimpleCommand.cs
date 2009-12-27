using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Hotwire.Shared
{
    public class SimpleCommand : ICommand
    {
        private Func<object, bool> _canExecute;
        private Action<object> _execute;
        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute.Invoke(parameter);
            else
                return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (_execute != null)
                _execute.Invoke(parameter);
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the SimpleCommand class.
        /// </summary>
        /// <param name="canExecute"></param>
        /// <param name="execute"></param>
        public SimpleCommand(Func<object, bool> canExecute, Action<object> execute)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        /// <summary>
        /// Initializes a new instance of the SimpleCommand class.
        /// </summary>
        /// <param name="execute"></param>
        public SimpleCommand(Action<object> execute)
        {
            _execute = execute;
        }
    }
}
