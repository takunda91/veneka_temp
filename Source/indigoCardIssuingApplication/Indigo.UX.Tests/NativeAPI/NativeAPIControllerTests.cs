using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.UX.NativeAppAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace Veneka.Indigo.UX.NativeAppAPI.Tests
{
    [TestClass()]
    public class NativeAPIControllerTests
    {
        [TestMethod()]
        public void CreateTokenTest()
        {            
            var token = NativeAPIController.CreateToken(Guid.NewGuid(), new indigoCardIssuingWeb.Old_App_Code.security.IndigoIdentity(new GenericIdentity("Test"), 
                                                                1, "1", "2", "em", true, false, false,0,"abcd"),
                                                                Action.PINSelect,0, 0,0,false,"");

            long userId;
            string nextToken;

            NativeAPIController.ValidateToken(token, Action.PINSelect, 0, out Guid checkGuid, out string sessionKey, out long cardId, out int branchId, out bool isPinReissue,out string printJobId,out string productBin, out nextToken);
        }

        [TestMethod()]
        public void ValidateTokenTest()
        {
            Assert.Fail();
        }
    }
}