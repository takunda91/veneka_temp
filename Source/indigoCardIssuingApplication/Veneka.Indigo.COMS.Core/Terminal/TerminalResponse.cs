using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.COMS.Core.Terminal
{
    public class TerminalResponse<V> : ComsResponse<V>
    {
        public string ISOResponseCode { get; set; }

        public static TerminalResponse<V> Success(string responseMessage, string isoCode, V value)
        {
            return new TerminalResponse<V>
            {
                ResponseCode = (int)ComsResponseCodes.SUCCESS,
                ISOResponseCode = isoCode,
                ResponseMessage = responseMessage,
                Value = value
            };
        }

        public static TerminalResponse<V> Success(string responseMessage, int isoCode, V value)
        {
            return Success(responseMessage, isoCode.ToString("00"), value);
        }

        public static TerminalResponse<V> Retry(string responseMessage, string isoCode, V value)
        {
            return new TerminalResponse<V>
            {
                ResponseCode = (int)ComsResponseCodes.RETRY,
                ResponseMessage = responseMessage,
                ISOResponseCode = isoCode,
                Value = value
            };
        }

        public static TerminalResponse<V> Retry(string responseMessage, int isoCode, V value)
        {
            return Retry(responseMessage, isoCode.ToString("00"), value);
        }

        public static TerminalResponse<V> Warning(string responseMessage, string isoCode, V value)
        {
            return new TerminalResponse<V>
            {
                ResponseCode = (int)ComsResponseCodes.WARNING,
                ResponseMessage = responseMessage,
                ISOResponseCode = isoCode,
                Value = value
            };
        }

        public static TerminalResponse<V> Warning(string responseMessage, int isoCode, V value)
        {
            return Warning(responseMessage, isoCode.ToString("00"), value);
        }

        public static TerminalResponse<V> Failed(string responseMessage, string isoCode, V value)
        {
            return new TerminalResponse<V>
            {
                ResponseCode = (int)ComsResponseCodes.ERROR,
                ResponseMessage = responseMessage,
                ISOResponseCode = isoCode,
                Value = value
            };
        }
        public static TerminalResponse<V> Failed(string responseMessage, int isoCode, V value)
        {
            return Failed(responseMessage, isoCode.ToString("00"), value);
        }


        public static TerminalResponse<V> Exception(Exception exception, string isoCode, V value)
        {
            return new TerminalResponse<V>
            {
                ResponseCode = (int)ComsResponseCodes.ERROR,
                ResponseMessage = exception.Message,
                ISOResponseCode = isoCode,
                ResponseException = exception.ToString(),
                Value = value
            };
        }

        public static TerminalResponse<V> Exception(Exception exception, int isoCode, V value)
        {
            return Exception(exception, isoCode.ToString("00"), value);
        }
    }
}
