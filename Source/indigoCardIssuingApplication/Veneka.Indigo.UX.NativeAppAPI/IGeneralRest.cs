using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Veneka.Indigo.UX.NativeAppAPI
{
    [ServiceContract]
    public interface IGeneralRest
    {
        [OperationContract]
        [WebGet(UriTemplate = "general/checkstatus/get/{guid}", ResponseFormat = WebMessageFormat.Json)]
        int CheckStatusRest(string guid);
    }
}
