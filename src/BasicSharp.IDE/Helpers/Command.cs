using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BasicSharp.IDE.Helpers
{
    public class Command : ICommand
    {
        public Func<object, bool> CanExecuteFunction { get; set; }
        public Action<object> ExecuteAction { get; set; }


        public bool CanExecute(object parameter)
        {
            if (CanExecuteFunction != null)
                return CanExecuteFunction(parameter);

            return true;
        }

        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            if (ExecuteAction == null)
                return;

            ExecuteAction(parameter);
        }
    }
}
