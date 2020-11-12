using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.Cryptography
{
    public static class PinBlockDecode
    {
        private static string _pinBlockFormatExString = "PIN Block is not in {0}, supplied format is {1}.";

        private static int Getlength(char lengthChar)
        {
            switch(lengthChar)
            {
                case '0':
                case '1':
                case '2':
                case '3': throw new ArgumentException("Must be greater than 3", nameof(lengthChar));
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                case 'A': case 'a': return 10;
                case 'B': case 'b': return 11;
                case 'C': case 'c': return 12;
                default: throw new ArgumentException("Unknown length character", nameof(lengthChar));
            }
        }

        public static string ISO_0(string PinBlock, string PAN)
        {
            if (PinBlock[0] != '0')
                throw new ArgumentException(String.Format(_pinBlockFormatExString, "ISO Format 0", PinBlock[0]), nameof(PinBlock));

            var decodedPinBlock = Utility.XORHexStringsFull(PinBlock, "0000" + PAN.Substring(PAN.Length - 13, 12));
            return decodedPinBlock.Substring(2, Getlength(decodedPinBlock[1]));            
        }

        public static string ISO_1(string PinBlock)
        {
            if (PinBlock[0] != '1')
                throw new ArgumentException(String.Format(_pinBlockFormatExString, "ISO Format 1", PinBlock[0]), nameof(PinBlock));

            return PinBlock.Substring(2, Getlength(PinBlock[1]));
        }

        public static string ISO_2(string PinBlock)
        {
            if (PinBlock[0] != '2')
                throw new ArgumentException(String.Format(_pinBlockFormatExString, "ISO Format 2", PinBlock[0]), nameof(PinBlock));

            return PinBlock.Substring(2, Getlength(PinBlock[1]));
        }

        public static string ISO_3(string PinBlock, string PAN)
        {
            if (PinBlock[0] != '3')
                throw new ArgumentException(String.Format(_pinBlockFormatExString, "ISO Format 3", PinBlock[0]), nameof(PinBlock));

            var decodedPinBlock = Utility.XORHexStringsFull(PinBlock, "0000" + PAN.Substring(PAN.Length - 13, 12));
            return decodedPinBlock.Substring(2, Getlength(decodedPinBlock[1]));
        }

        /// <summary>
        /// OEM-1 / Diebold / Docutel / NCR
        /// </summary>
        /// <param name="PinBlock"></param>
        /// <returns></returns>
        public static string OEM_1(string pinBlock)
        {
            var firstIndex = pinBlock.IndexOf(pinBlock[15]); // Look at what the padding char is at the end of string and find its first occurance
            return pinBlock.Substring(0, firstIndex);
        }

        /// <summary>
        /// ECI-2 PIN block format supports a 4-digit PIN
        /// </summary>
        /// <param name="pinBlock"></param>
        /// <returns></returns>
        public static string ECI_2(string pinBlock)
        {            
            return pinBlock.Substring(0, 4);
        }

        /// <summary>
        /// ECI-3 PIN block format supports a PIN from 4 to 6 digits in length
        /// </summary>
        /// <param name="pinBlock"></param>
        /// <returns></returns>
        public static string ECI_3(string pinBlock)
        {
            return pinBlock.Substring(1, Getlength(pinBlock[0]));
        }
    }
}
