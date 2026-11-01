
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RESULT_ANALYTICS.Helpers
{
    internal class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Func<object, bool> canExecute;

        // Ham khoi tao
        public RelayCommand(
            Action<object> execute,
            Func<object, bool> canExecute = null)
        {
            this.execute = execute ??
                throw new ArgumentNullException(nameof(execute));

            this.canExecute = canExecute;
        }

        // Kiem tra nut co duoc phep thuc hien
        public bool CanExecute(object parameter)
        {
            return canExecute == null ||
                   canExecute(parameter);
        }

        // Thuc hien lenh
        public void Execute(object parameter)
        {
            execute(parameter);
        }

        // Cap nhat trang thai cua nut
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
    }
}
