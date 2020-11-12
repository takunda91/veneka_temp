using IndigoCardIssuanceService.DataContracts;
using IndigoCardIssuanceService.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Veneka.Indigo.COMS.Core;

namespace IndigoCardIssuanceService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ManagementAPI" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ManagementAPI.svc or ManagementAPI.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(Namespace = Constants.IndigoComsURL)]

    public class ManagementAPI : IManagementAPI
    {
      
        public ComsResponse<bool> ReloadIntegration(byte[] fileStream, string checkSum)
        {
            return COMS.COMSController.ComsCore.ReloadIntegration(fileStream, checkSum);
        }
    }
}
