using System;
using Common.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Veneka.IndigoAppTests
{
    [TestClass]
    public class Logging
    {
        [TestMethod]
        public void CheckDebugLoggingEnabled()
        {
            // Because some error messages are only returned when debugging or trace level logging is set. 
            // We need to ensure logging is set to debug level.
            var logger = LogManager.GetLogger("all");

            Assert.IsTrue(logger.IsDebugEnabled);        
        }
    }
}
