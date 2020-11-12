using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.BackOffice.Application.ViewModels
{
    [PrincipalPermission(SecurityAction.Demand)]
    public class HomeViewModel : ViewModel
    {
    }
}
