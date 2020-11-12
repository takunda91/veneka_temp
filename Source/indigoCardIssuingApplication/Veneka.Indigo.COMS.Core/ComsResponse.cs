using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.COMS.Core
{
    public enum ComsResponseCodes
    {
        SUCCESS = 0,
        WARNING = 1,
        ERROR = 9, //Integer value same as CMS_ERROR in branch card status
        RETRY = 15 //Integer value same as CMS_RETRY in branch card status
    }

    [DataContract(Namespace = Constants.IndigoComsURL)]
    public class ComsResponse<V> 
    {
        [DataMember(IsRequired = true)]
        public int ResponseCode { get; set; }

        [DataMember(IsRequired = true)]
        public string ResponseMessage { get; set; }

        [DataMember]
        public V Value { get; set; }
                
        [DataMember]
        public string ResponseException { get; set; }

        public static ComsResponse<V> Success(string responseMessage, V value)
        {
            return new ComsResponse<V>
            {
                ResponseCode = (int)ComsResponseCodes.SUCCESS,
                ResponseMessage = responseMessage,
                Value = value
            };
        }
        public static ComsResponse<V> Retry(string responseMessage, V value)
        {
            return new ComsResponse<V>
            {
                ResponseCode = (int)ComsResponseCodes.RETRY,
                ResponseMessage = responseMessage,
                Value = value
            };
        }

        public static ComsResponse<V> Warning(string responseMessage, V value)
        {
            return new ComsResponse<V>
            {
                ResponseCode = (int)ComsResponseCodes.WARNING,
                ResponseMessage = responseMessage,
                Value = value
            };
        }

        public static ComsResponse<V> Failed(string responseMessage, V value)
        {
            return new ComsResponse<V>
            {
                ResponseCode = (int)ComsResponseCodes.ERROR,
                ResponseMessage = responseMessage,
                Value = value
            };
        }

        public static ComsResponse<V> Exception(Exception exception, V value)
        {
            return new ComsResponse<V>
            {
                ResponseCode = (int)ComsResponseCodes.ERROR,
                ResponseMessage = exception.Message,
                ResponseException = exception.ToString(),
                Value = value
            };
        }

        public bool ResponseEquals(ComsResponseCodes responseCode)
        {
            if(ResponseCode == (int)responseCode)
            {
                return true;
            }

            return false;
        }
        
    }
}
