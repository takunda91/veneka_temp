using IndigoCardIssuanceService.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Veneka.Indigo.Integration.FileLoader.Objects;

namespace IndigoCardIssuanceService.External
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICardRequest" in both code and config file together.
    [ServiceContract(Namespace = "http://schemas.veneka.com/Indigo")]
    public interface ICardRequest
    {
        [OperationContract]
        Response<string> RequestCard(BulkRequestRecord request);

        [OperationContract]
        Response<string> RequestedCardStatus(string referenceNumber);
    }

    
}
