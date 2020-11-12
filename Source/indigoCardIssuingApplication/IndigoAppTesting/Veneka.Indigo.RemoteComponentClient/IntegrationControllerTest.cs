using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.RemoteComponentClient.BLL;
using System.ComponentModel.Composition.Hosting;
using IndigoAppTesting.MockIntegration;

namespace IndigoAppTesting.Veneka.Indigo.RemoteComponentClient
{
    [TestClass]
    public class IntegrationControllerTest
    {
        [TestMethod]
        public void LoadControllerTest()
        {
            AssemblyCatalog catalog = new AssemblyCatalog(typeof(MockRemoteCMS).Assembly);

            IntegrationController integration = new IntegrationController(catalog);

            var interfaces = integration.Interfaces;

            Assert.AreEqual(1, interfaces.Count);
            Assert.AreEqual(MockRemoteCMS.GUID, interfaces[0].IntegrationGUID);
            Assert.AreEqual(MockRemoteCMS.Name, interfaces[0].IntegrationName);
        }
    }
}
