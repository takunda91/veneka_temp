using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.NativeApp.Device.PINPad.PAXS300.GPT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using Veneka.Data.Hexadecimal;

namespace Veneka.Indigo.NativeApp.Device.PINPad.GptPaxS300.Tests
{
    [TestClass()]
    public class GptPaxS300PINPadTests
    {
        GptPaxS300PINPad pinPad;

        [TestMethod()]
        public void GptPaxS300PINPadTest()
        {
            //var ports = SerialPort.GetPortNames();

            pinPad = new GptPaxS300PINPad(2);

            pinPad.InitialisePinPad();            

            var resp = pinPad.SetTPK(new HexString("CF60D91CB4E4D1CB5BCE0DEFAF768DD6"));
            var resp2 = pinPad.PresentCard("Present Card");
            var resp3 = pinPad.GetNewPIN(resp2.Value, 4, 8, "Enter PIN");
            var resp4 = pinPad.GetNewPINConfirmation(resp2.Value, 4, 8, "Enter PIN");
            var resp5 = pinPad.RemoveCard("Present Card");
        }

        [TestMethod()]
        public void InitialisePinPadTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetTPKTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetTrackDataTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetPINBlockTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RemoveCardTest()
        {
            Assert.Fail();
        }
    }
}