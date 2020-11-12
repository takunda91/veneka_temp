using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.DesktopApp.Device.Printer.Simulator
{
  public  class PrintSimMagData : IDeviceMagData
    {
        private PrintSimMagData() { }

        public PrintSimMagData(byte[][] trackData)
        {
            TrackData = trackData;
        }



        public byte[][] TrackData { get; private set; }

        public byte[] this[int track]
        {
            get { return TrackData[track - 1]; } // Correct for 0 based index and track 1/2/3
        }

        public string TrackDataToString(int track)
        {
            if (track > TrackData.Length)
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
