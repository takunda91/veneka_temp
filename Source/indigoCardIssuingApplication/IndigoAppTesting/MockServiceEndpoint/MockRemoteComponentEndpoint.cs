using IndigoAppTesting.MockDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.Remote;
using Veneka.Indigo.Remote;

namespace IndigoAppTesting.MockServiceEndpoint
{
    public class MockRemoteComponentEndpoint
    {
        public class GoodEndpoint : IRemoteComponent
        {
            private RemoteController _remoteController = new RemoteController(new MockRemoteTokenDAL(), new MockRemoteCardUpdateDAL(), null);

            public RemoteCardUpdates GetPendingCardUpdates(string token)
            {
                return _remoteController.GetPendingCardUpdates(token, "UnitTestIP");
            }

            public Response CardUpdateResults(string token, RemoteCardUpdatesResponse remoteCardUpdatesResponse)
            {
                throw new NotImplementedException();
            }
        }

        public class MockEndpoint : IRemoteComponent
        {
            public RemoteCardUpdatesResponse CardUpdatesResponse { get; set; }
            public RemoteCardUpdates CardUpdates { get; set; }
            public Response MockResponse { get; set; }

            public RemoteCardUpdates GetPendingCardUpdates(string token)
            {
                return CardUpdates;
            }

            Response IRemoteComponent.CardUpdateResults(string token, RemoteCardUpdatesResponse remoteCardUpdatesResponse)
            {
                CardUpdatesResponse = remoteCardUpdatesResponse;
                return MockResponse;
            }
        }
    }
}
