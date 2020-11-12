using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.DesktopApp.Device.PINPad
{
    public enum CardTrackDataType
    {
        PAN,
        Track2
    }

    public sealed class CardTrackData
    {
        public CardTrackData(string data, CardTrackDataType dataType, bool isEncrypted)
        {
            DataType = dataType;
            IsEncrypted = isEncrypted;

            switch (dataType)
            {
                case CardTrackDataType.PAN:
                    PAN = data;
                    break;
                case CardTrackDataType.Track2:
                    Track2 = data;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dataType), "Unknown data type");                    
            }
        }

        public string Track2 { get; private set; }
        public string PAN { get; private set; }
        public CardTrackDataType DataType { get; private set; }
        public bool IsEncrypted { get; private set; }
    }
}
