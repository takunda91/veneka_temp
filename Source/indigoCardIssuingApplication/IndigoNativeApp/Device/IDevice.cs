using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.DesktopApp.Device
{
    public delegate void DeviceNotificationEventHandler(object sender, string message, bool isCritical, EventArgs e);

    public interface IDevice : IDisposable
    {
        /// <summary>
        /// Used for the device to raise notifications back to the implementing class
        /// </summary>
        event DeviceNotificationEventHandler OnDeviceNotifcation;

        string Name { get; }
        string Manufacturer { get; }
        string Model { get; }
        string FirmwareVersion { get; }
        int ComPort { get; }
        string DeviceId { get; }

        /// <summary>
        /// Used to send key/value pair of settings to the device
        /// </summary>
        /// <param name="settings"></param>
        void SetDeviceSettings(Dictionary<string, string> settings);
    }
}
