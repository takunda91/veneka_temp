using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.BackOffice.Application.ViewModels
{
    public interface IViewModelNavigation
    {
        void ChangeViewModel(object viewModel);
    }
}
