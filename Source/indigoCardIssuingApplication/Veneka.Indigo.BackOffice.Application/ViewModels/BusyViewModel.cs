using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Veneka.Indigo.BackOffice.Application.ViewModels
{
    public class BusyViewModel : INotifyPropertyChanged
    {
        private ICommand doSomethingCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Item1 { get; set; }
        public string Item2 { get; set; }
        public List<String> Results { get; set; }

        public ICommand DoSomething
        {
            get { return doSomethingCommand ?? (doSomethingCommand = new DelegateCommand(LongRunningTask)); }
        }
        
        private void LongRunningTask()
        {
            var results = Enumerable.Range(0, 12).Select(x =>
            {
                Thread.Sleep(250);
                return "Result " + x;
            }).ToList();
            this.Results = results;
        }
    }
}
