using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Veneka.Indigo.RemoteComponentClient.Configuration;
using System.Linq;
using System.IO;

namespace IndigoAppTesting.Veneka.Indigo.RemoteComponentClient
{
    [TestClass]
    public class ConfigurationTest
    {
        /// <summary>
        /// Make sure the tokens read from the config are the the same as valid tokens. Check app.config 
        /// </summary>
        [TestMethod]
        public void GetTokensFromConfigText()
        {
            List<string> validTokens = new List<string> { "14b225b9-e11f-4a56-a588-8f96b370dd23",
                                                          "BA7A2F42-8476-464B-8F50-897776CE70B0",
                                                          "95978F9B-3853-4512-9D40-675987BF34E0",
                                                          "670979EB-E8D3-4B34-964E-8ACB037CAA94" };
            
            
            var remoteTokens = ConfigReader.GetRemoteTokens();

            Assert.AreEqual(true, remoteTokens.SequenceEqual(validTokens));
        }


        [TestMethod]
        public void GetIntegrationPath()
        {
            var path = ConfigReader.ApplicationConfigPath;

            Assert.AreEqual(new DirectoryInfo("C:\\veneka\\integration").FullName, path.FullName);
        }


        [TestMethod]
        public void GetCardUploadInterval()
        {
            var interval = ConfigReader.CardUpdateTimerMili;

            Assert.AreEqual(5000, interval);
        }
    }
}
