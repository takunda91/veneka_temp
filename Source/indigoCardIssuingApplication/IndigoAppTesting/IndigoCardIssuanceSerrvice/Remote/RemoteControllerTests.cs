using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IndigoAppTesting.MockDataAccess;
using Veneka.Indigo.Integration.Remote;

namespace Veneka.Indigo.Remote.Tests
{
    [TestClass()]
    public class RemoteControllerTests
    {
        //[Ignore]
        [TestMethod()]
        public void GetExports_IntegrationTest()
        {
            RemoteController remoteController = new RemoteController();

            var exports = remoteController.GetPendingCardUpdates("a195c883-540d-4ab1-bc73-128477c621a6", "UnitTestIP");

            RemoteCardUpdatesResponse responseCards = new RemoteCardUpdatesResponse();

            for(int i = 0; i < exports.Cards.Count; i++)
            {
                responseCards.CardsResponse.Add(new CardDetailResponse
                {
                    CardId = exports.Cards[i].card_id,
                    Detail = (i + 1) % 2 == 0 ? "Good" : "BAD",
                    TimeUpdated = DateTime.Now,
                    UpdateSuccessful = (i + 1) % 2 == 0
                });
            }


            remoteController.CardUpdateResults("14b225b9-e11f-4a56-a588-8f96b370dd23", responseCards, "UnitTestIP");

            //Assert.Fail();
        }

        [TestMethod()]
        public void GetExportsTest()
        {
            RemoteController remoteController = new RemoteController(new MockRemoteTokenDAL(), new MockRemoteCardUpdateDAL(), null);

            var exports = remoteController.GetPendingCardUpdates(MockRemoteTokenDAL.RemoteTokenValid.ToString(), "UnitTestIP");

            //Assert.Fail();
        }
    }
}