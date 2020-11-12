using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.ServicesAuthentication.API;

namespace Veneka.Indigo.BackOffice.API
{
    [ServiceContract(Namespace = Constants.BackOfficeApiUrl)]
    public interface IBackOfficeAPI :  IGeneral, IBulkPrintingAPI
    {      


    }
}
