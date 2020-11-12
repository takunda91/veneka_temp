using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Veneka.Indigo.BackOffice.Application.ViewModels
{
    public class CommandViewModel : ViewModel
    {
        public CommandViewModel(string displayName, ICommand command)
        {
            base.DisplayName = displayName;
            this.Command = command ?? null;
        }

        public ICommand Command { get; private set; }
    }
}
