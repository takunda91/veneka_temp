using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.NativeApp.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Veneka.Indigo.NativeApp.Config.Tests
{
    [TestClass()]
    public class ConfigReaderTests
    {
        [TestMethod()]
        public void GetDeviceSearchOptionTest()
        {           

            var deviceOption = ConfigReader.GetDeviceSearchOption("gpts300");
        }
    }
}