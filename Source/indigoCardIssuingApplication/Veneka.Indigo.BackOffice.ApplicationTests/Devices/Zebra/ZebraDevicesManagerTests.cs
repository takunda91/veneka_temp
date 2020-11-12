using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.BackOffice.Application.Devices.Zebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace Veneka.Indigo.BackOffice.Application.Devices.Zebra.Tests
{
    [TestClass()]
    public class ZebraDevicesManagerTests
    {
        [TestMethod()]
        public void FindDevicesTest()
        {
            ZebraDevicesManager zebraManager = new ZebraDevicesManager();
            var devices = zebraManager.FindDevices();
        }

        [TestMethod()]
        public void FindDevicesTest1()
        {
            ZebraDevicesManager zebraManager = new ZebraDevicesManager();
            var devices = zebraManager.FindDevices(DeviceConnectionTypes.Ethernet);

            if(devices != null && devices[0] is ZebraZXP7)
            {
                var zxp7 = (ZebraZXP7)devices[0];

                try
                {
                    zxp7.Connect();
                    zxp7.Disconnect();
                    zxp7.Connect();
                    //zxp7.Print("");
                    string _msg = string.Empty;
                   string _track1Data = string.Empty;
                    string _track2Data = string.Empty;
                    string _track3Data = string.Empty;
                    List<PrintField> _list = new List<PrintField>();
                    byte[] b = File.ReadAllBytes(@"C:\god songs\123.png");
                    //zxp7.MagRead(out _track1Data,out _track2Data,out _track3Data,out _msg);
                    
                    _list.Add(new PrintField() { Value = System.Text.ASCIIEncoding.ASCII.GetBytes("University Of Pretoria"), Font = "Arial", FontColourRGB=  1, FontSize = 13, X = 120, Y = 50,PrintFieldTypeId=0,PrintSide=0 });
                    _list.Add(new PrintField() { Value = System.Text.ASCIIEncoding.ASCII.GetBytes("Name :"), Font = "Arial", FontColourRGB = 1, FontSize = 9, X = 50, Y = 150, PrintFieldTypeId = 0, PrintSide = 0 });
                    _list.Add(new PrintField() { Value = System.Text.ASCIIEncoding.ASCII.GetBytes("Student 1"), Font = "Arial", FontColourRGB = 1, FontSize = 9, X = 200, Y = 150, PrintFieldTypeId = 0, PrintSide = 0 });
                    _list.Add(new PrintField() { Value = System.Text.ASCIIEncoding.ASCII.GetBytes("Blood Group :"), Font = "Arial", FontColourRGB = 1, FontSize = 9, X = 50, Y = 200, PrintFieldTypeId = 0, PrintSide = 0 });
                    _list.Add(new PrintField() { Value = System.Text.ASCIIEncoding.ASCII.GetBytes("O+"), Font = "Arial", FontColourRGB = 1, FontSize = 9, X = 200, Y = 200, PrintFieldTypeId = 0, PrintSide = 0 });
                    _list.Add(new PrintField() { Value = System.Text.ASCIIEncoding.ASCII.GetBytes("Address :"), Font = "Arial", FontColourRGB = 1, FontSize = 9, X = 50, Y = 250, PrintFieldTypeId = 0, PrintSide = 0 });
                    _list.Add(new PrintField() { Value = System.Text.ASCIIEncoding.ASCII.GetBytes("H:No 8,Road no 8,street123,Johannesburg"), Font = "Arial", FontColourRGB = 1, FontSize = 9, X = 200, Y = 350, PrintFieldTypeId = 0, PrintSide = 0 });
                    _list.Add(new PrintField() { Value = System.Text.ASCIIEncoding.ASCII.GetBytes("Signature"), Font = "Arial", FontColourRGB = 1, FontSize = 9, X = 50, Y = 270, PrintFieldTypeId = 0, PrintSide = 1 });
                    _list.Add(new PrintField() { Value = b, Font = "Arial", FontColourRGB = 0, FontSize = 13, X = 400, Y = 150, PrintFieldTypeId = 1, PrintSide = 0 , Width=200, Height=250});
                    string errMsg;
                    zxp7.Print(_list.ToArray(),out errMsg);
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    zxp7.Disconnect();
                }
            }
        }

        [TestMethod()]
        public void GetDeviceListTest()
        {
            Assert.Fail();
        }
    }
}