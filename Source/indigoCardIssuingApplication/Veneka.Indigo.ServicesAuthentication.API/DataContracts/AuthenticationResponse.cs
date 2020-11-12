using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.ServicesAuthentication.API.DataContracts
{
    [DataContract]
    public class AuthenticationResponse
    {
        [DataMember]
        public string ResponseCode { get; set; }

        [DataMember]
        public string ResponseMessage { get; set; }

        [DataMember]
        public string AuthToken { get; set; }

        [DataMember]
        public bool RequireMultiFactor { get; set; }
    }
}
