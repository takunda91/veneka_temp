using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Veneka.Indigo.Integration.Common
{
    [DataContract]
    [Serializable]
    public class Track2
    {
        private string _pan = String.Empty;
        private string _expiryDate = String.Empty;
        private string _serviceCode = String.Empty;
        private string _pvv = String.Empty;
        private string _discretionaryData = String.Empty;

        [DataMember]
        public string PAN { get { return _pan; } set { _pan = value; } }

        [DataMember]
        public string ExpiryDate { get { return _expiryDate; } set { _expiryDate = value; } }

        [DataMember]
        public string ServiceCode { get { return _serviceCode; } set { _serviceCode = value; } }

        [DataMember]
        public string PVV { get { return _pvv; } set { _pvv = value; } }

        [DataMember]
        public string DiscretionaryData { get { return _discretionaryData; } set { _discretionaryData = value; } }
        public Track2()
        {

        }
        public Track2(string pan, string expiryDate, string serviceCode, string pvv, string discretionaryData)
        {
            _pan = pan;
            _expiryDate = expiryDate;
            _serviceCode = serviceCode;
            _pvv = pvv;
            _discretionaryData = discretionaryData;
        }

        public Track2(string track2Data)
        {
            int dataToProcess = 0;

            foreach (char c in track2Data)
            {
                if (c == '=' || c =='D')
                {
                    dataToProcess++;
                    continue;
                }

                switch (dataToProcess)
                {
                    case 0: ProcessPan(c); break;
                    case 1: ProcessExpiry(c, ref dataToProcess); break;
                    case 2: ProcessServiceCode(c, ref dataToProcess); break;
                    case 3: ProcessPVV(c, ref dataToProcess); break;
                    case 4: ProcessDiscData(c); break;
                }
            }
        }

        private void ProcessPan(char c)
        {
            _pan += c;
        }

        private void ProcessExpiry(char c, ref int dataToProcess)
        {
            _expiryDate += c;

            if (_expiryDate.Length == 4)
                dataToProcess++;
        }

        private void ProcessServiceCode(char c, ref int dataToProcess)
        {
            _serviceCode += c;

            if (_serviceCode.Length == 3)
                dataToProcess++;
        }

        private void ProcessPVV(char c, ref int dataToProcess)
        {
            _pvv += c;

            if (_pvv.Length == 5)
                dataToProcess++;
        }

        private void ProcessDiscData(char c)
        {
            _discretionaryData += c;
        }
    }
}
