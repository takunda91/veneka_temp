using System;
using System.Collections.Generic;
using Common.Logging;
using System.Runtime.Serialization;

namespace IndigoCardIssuanceService.DataContracts
{
    /// <summary>
    /// Basic web method response, contains the ResponseType, ResponseMessage.
    /// This should be inherited in any other web method responses.
    /// </summary>
    [DataContract(Name = "BaseResponse", Namespace = "http://schemas.veneka.com/Indigo")]
    public class BaseResponse
    {
        [DataMember(IsRequired = true)]
        public ResponseType ResponseType { get; set; }     
        [DataMember]   
        public string ResponseMessage { get; set; }

        //Send the exception back to the front end for debuging
        [DataMember]
        public string ResponseException { get; set; } 

        public BaseResponse()
        {
        }

        /// <summary>
        /// Basic response object.
        /// </summary>
        /// <param name="responseType">What state the response is in.</param>
        /// <param name="responseMessage">User friendly message to display status of response.</param>
        /// <param name="responseException">Options exception message to be included. Will only be shown in debugging or tracing is enabled.</param>
        public BaseResponse(ResponseType responseType, string responseMessage, string responseException)
        {
            this.ResponseType = responseType;
            this.ResponseMessage = responseMessage;
            this.ResponseException = responseException;            
        }
    }
}