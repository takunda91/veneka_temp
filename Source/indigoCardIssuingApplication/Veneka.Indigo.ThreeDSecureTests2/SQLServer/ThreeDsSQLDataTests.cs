using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.ThreeDSecure.Data.SQLServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.ThreeDSecure.Data.SQLServer.Tests
{
    [TestClass()]
    public class ThreeDsSQLDataTests
    {
        [TestMethod()]
        public void GetUnregisteredCardsTest()
        {

            ThreeDsSQLData data = new ThreeDsSQLData(@"data source=VENEKA-TEST\sqlexpress;initial catalog=indigo_database_2.1.4.0_RP;integrated security=True;");

            var values = data.GetUnregisteredCards(1, "68F7F36D-4C88-4DEB-8399-5C428B7CEC5E", false, 0, -2, "SYSTEM");

            Assert.Fail();
        }
    }
}