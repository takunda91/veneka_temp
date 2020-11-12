using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.BackOffice.Application.Authentication
{
    public interface IAuthViewModel
    {
        bool IsAuthenticated { get; }
    }
}
