using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Veneka.Indigo.UX.NativeAppAPI
{
    [DataContract]
    public class Response<T>
    {
        public Response() { }

        public Response(bool success, string additionalInfo, T value, string session)
        {
            Success = success;
            AdditionalInfo = additionalInfo;
            Value = value;
            Session = session;
        }

        [DataMember(IsRequired = true)]
        public bool Success { get; set; }

        [DataMember]
        public string AdditionalInfo { get; set; }

        [DataMember]
        public T Value { get; set; }

        [DataMember(IsRequired = true)]
        public string Session { get; set; }
    }
}
