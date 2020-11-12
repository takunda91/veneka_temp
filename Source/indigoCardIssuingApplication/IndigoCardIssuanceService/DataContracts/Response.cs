using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace IndigoCardIssuanceService.DataContracts
{
    /// <summary>
    /// Returns the BaseResponse as well as a value.
    /// </summary>
    /// <typeparam name="V"></typeparam>
    [DataContract(Name = "Response", Namespace = "http://schemas.veneka.com/Indigo")]
    public sealed class Response<V> : BaseResponse
    {
        [DataMember]
        public V Value { get; set; }

        private Response()
            :base()
        {
        }

        /// <summary>
        /// Response object including BaseResponse and a value.
        /// </summary>
        /// <param name="value">Returned value for the response.</param>
        /// <param name="responseType">What state the response is in.</param>
        /// <param name="responseMessage">User friendly message to display status of response.</param>
        /// <param name="responseException">Options exception message to be included. Will only be shown in debugging or tracing is enabled.</param>
        public Response(V value, ResponseType responseType,string responseMessage, string responseException)
            : base(responseType, responseMessage, responseException)
        {
            this.Value = value;
        }
    }
}