using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.COMS.Core
{
    [DataContract]
    public class AuditInfo
    {
        [DataMember(IsRequired = true)]
        public int LanguageId { get; set; }

        [DataMember(IsRequired = true)]
        public long AuditUserId { get; set; }

        [DataMember(IsRequired = true)]
        public string AuditWorkStation { get; set; }
    }
}
