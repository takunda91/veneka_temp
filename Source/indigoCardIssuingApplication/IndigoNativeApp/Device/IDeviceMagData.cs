using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.DesktopApp.Device
{
    public interface IDeviceMagData
    {
        // Track data Indexer - Adjust for 0 based index
        byte[] this[int index]
        {
            get;            
        }

        byte[][] TrackData { get; }

        string TrackDataToString(int track);

        /// <summary>
        /// Use this to convert bytes to string using the object's Encoding
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        string BytesToString(byte[] bytes);
    }
}
