using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace IndigoDesktopApp.ViewModels
{
    public class HomeViewModel : IPageViewModel
    {
        private ICommand hiButtonCommand;
        private bool canExecute = true;

        public HomeViewModel()
        {
            HiButtonCommand = new RelayCommand(ShowMessage, param => this.canExecute);
        }

        public void ShowMessage(object obj)
        {
            //if(obj.ToString().Equals("PINSet", StringComparison.InvariantCultureIgnoreCase))
             //   ViewModel = new PinSelectViewModel();
        }

        public bool CanExecute
        {
            get { return this.canExecute; }
            set
            {
                if (this.canExecute == value)                
                    return;                

                this.canExecute = value;
            }
        }

        public ICommand HiButtonCommand
        {
            get { return hiButtonCommand; }
            set { hiButtonCommand = value; }
        }
    }
}
