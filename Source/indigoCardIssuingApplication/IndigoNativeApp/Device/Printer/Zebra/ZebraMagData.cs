using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zebra.Sdk.Card.Containers;

namespace Veneka.Indigo.DesktopApp.Device.Printer.Zebra
{
    public class ZebraMagData : IDeviceMagData
    {
        private ZebraMagData() { }

        public ZebraMagData(byte[][] trackData)
        {
            TrackData = trackData;
        }

        public ZebraMagData(MagTrackData trackData)
        {
            var track1Bytes = Encoding.UTF8.GetBytes(trackData.Track1);
            var track2Bytes = Encoding.UTF8.GetBytes(trackData.Track2);
            var track3Bytes = Encoding.UTF8.GetBytes(trackData.Track3);

            TrackData = new byte[][]
            {
                track1Bytes, track2Bytes, track3Bytes
            };
        }

        public byte[][] TrackData { get; private set; }

        public byte[] this[int track]
        {
            get { return TrackData[track - 1]; } // Correct for 0 based index and track 1/2/3
        }

        public string TrackDataToString(int track)
        {
            if(track > TrackData.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(track));
            }

            return Encoding.UTF8.GetString(TrackData[track - 1]);
        }

        public string BytesToString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
