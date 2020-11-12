using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Veneka.Indigo.BackOffice.Application.Devices
{
    public enum DeviceConnectionTypes
    {
        USB = 0,
        Ethernet = 1
    }
    public enum CardSide
    {
        Front = 0,
        Back = 1
    }
    public enum PrintFieldTypeId
    {
        Text = 0,
        Image = 1
    }
    public enum FontType
    {
        Regular = 0,
        Bold = 1,
        Italic = 2,
        Underline = 4,
        Strikeout = 8
    }
    public struct PrintField
    {
        public byte[] Value;
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string Font { get; set; }
        public int FontSize { get; set; }
        public int FontColourRGB { get; set; }
        public int PrintSide { get; set; }
        public string MappedName { get; set; }
        public int PrintFieldTypeId { get; set; }


        public string ValueToString()
        {
            return System.Text.Encoding.UTF8.GetString(Value);
        }
    }
    public enum Orientation
    {
        Portrait = 0,
        Landscape = 1
    }
    public interface IDevice
    {
        string DeviceName { get; }
        string Vendor { get; }
        string Model { get; }
        string SerialNo { get; }
        string MAC { get; }
        string FirmwareVersion { get; }
                
        DeviceConnectionTypes ConnectionType { get; }
        bool Connect();
        bool Eject(out string errMsg);
        bool Disconnect();
        bool MagRead(out string _track1Data, out string _track2Data, out string _track3Data, out string _msg);
        bool Print(PrintField[] printfields, out string errMsg);
    }
}
