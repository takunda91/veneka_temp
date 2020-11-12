using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace Veneka.Indigo.DesktopApp.Device
{
    public class DeviceSearch
    {
        //public IDevice[] GetUSBDevices()
        //{
        //    List<IDevice> devices = new List<IDevice>();

        //    // Win32_USBHub
        //    // Win32_SerialPort
        //    // Win32_PnPEntity

        //    ManagementObjectCollection collection;
        //    using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_PnPEntity Where Manufacturer Like 'PAX%'"))
        //        collection = searcher.Get();

        //    foreach (var device in collection)
        //    {
        //        //foreach (var prop in device.Properties)
        //        //{
        //        //    devices.Add(prop.Name + " " + prop.Value);
        //        //}

        //        var comport = System.Text.RegularExpressions.Regex.Match((string)device.GetPropertyValue("Caption"), @"^.*?\([COM]*(\d+)[^\d]*\).*$").Groups[1].Value;

        //        devices.Add(new PINPad.GptPaxS300.GptPaxS300PINPad(int.Parse(comport))
        //        //(string)device.GetPropertyValue("Manufacturer") + " | " +
        //        //(string)device.GetPropertyValue("Caption") + " | " +
        //        //(string)device.GetPropertyValue("Description") + " | " +
        //        //comport
        //        );
        //    }

        //    collection.Dispose();
        //    return devices.ToArray();
        //}
    }
}
