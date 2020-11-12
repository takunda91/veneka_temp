using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.BackOffice.Application.ViewModels;

namespace Veneka.Indigo.BackOffice.Application.Views
{
    public interface IView
    {
        IViewModel ViewModel
        {
            get;
            set;
        }

        
    }
}
