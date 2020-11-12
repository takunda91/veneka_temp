using System;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.BackOffice.API;

namespace Indigo.BackOffice.Tests
{
    [TestClass]
    public class BackOfficeAPITest
    {
        [TestMethod()]
        public void CreateTokenTest()
        {
            var token = BackOfficeAPIController.CreateToken(Guid.NewGuid(), new indigoCardIssuingWeb.Old_App_Code.security.IndigoIdentity(new GenericIdentity("Test"),
                                                                1, "1", "2", "em", true, false, "abcd"),
                                                                Veneka.Indigo.BackOffice.API.Action.PrintCard);

            long userId;
            string nextToken;

            BackOfficeAPIController.ValidateToken(token, Veneka.Indigo.BackOffice.API.Action.PrintCard, 0, out Guid checkGuid, out string sessionKey, out userId, out nextToken);
        }


        [TestMethod()]
        public void GetapprovedPrintBatches()
        {
            bu
        }
    }
}
