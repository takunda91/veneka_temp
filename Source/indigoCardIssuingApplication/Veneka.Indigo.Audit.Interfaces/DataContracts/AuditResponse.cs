using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Audit.Interfaces.DataContracts
{
    [DataContract(Namespace = AuditInterfaceConstants.IndigoAuditDataContractURL)]
    public sealed class AuditResponse
    {
        [DataMember(IsRequired = true)]
        public string ResponseCode { get; set; }

        [DataMember(IsRequired = true)]
        public string ResponseMessage { get; set; }
    }
}
