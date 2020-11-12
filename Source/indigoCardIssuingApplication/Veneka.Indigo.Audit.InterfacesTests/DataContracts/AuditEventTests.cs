using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.Audit.Interfaces.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Audit.Interfaces.DataContracts.Tests
{
    [TestClass()]
    public class AuditEventTests
    {
        [TestMethod()]
        public void AuditEventTest()
        {
            AuditEvent auditEvent = new AuditEvent(DateTimeOffset.Now, "00", 12, "details", "127.0.0.1", Guid.NewGuid().ToString() + "-1");
        }

        [TestMethod()]
        public void AuditEventTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AuditEventTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void AuditEventTest3()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetAuditDetailExceptionTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetClientAddressTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetClientAddressTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetIndigoIDTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetIndigoIDTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetIssuerIDTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetIssuerIDTest()
        {
            Assert.Fail();
        }
    }
}