using IndigoCardIssuanceService.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using IndigoCardIssuanceService.DataContracts;
using Veneka.Indigo.Integration.FileLoader.Objects;
using IndigoCardIssuanceService.bll;

namespace IndigoCardIssuanceService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CardRequest" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select CardRequest.svc or CardRequest.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(Namespace = "http://schemas.veneka.com/Indigo")]
    public sealed class CardRequest : ICardRequest
    {
        private readonly IssueCardController _issueCardController = new IssueCardController();

        public Response<string> RequestCard(BulkRequestRecord request)
        {
            return _issueCardController.RequestCardForCustomer(request);
        }

        public Response<string> RequestedCardStatus(string referenceNumber)
        {
            return new Response<string>("It worked", ResponseType.SUCCESSFUL, "Good", "No Exception");
        }
    }
}
