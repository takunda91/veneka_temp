using Catel.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndigoToolkit2.Models
{
    public class DacNotifications : DispatcherObservableObject
    {
        private string _progress;

        public string Progress
        {
            get { return _progress; }
            set
            {
                var oldValue = _progress;
                _progress = value;
                RaisePropertyChanged(() => Progress, oldValue, value);
            }
        }

        public void AddProgress(string message)
        {
            if(!String.IsNullOrWhiteSpace(message))
            {
                if(!String.IsNullOrWhiteSpace(_progress))
                {
                    message = Environment.NewLine + message;
                }

                Progress += message;
            }
        }
    }
}
