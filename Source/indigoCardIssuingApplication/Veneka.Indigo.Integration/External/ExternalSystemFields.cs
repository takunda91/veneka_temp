using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Veneka.Indigo.Integration.External
{
    public enum ExternalSystemType
    {
        CoreBankingSystem = 0,
        CardProductionSystem = 1,
        CardManagementSystem = 2,
        HardwareSecurityModule = 3,
        NotificationSystem = 4
    }

    [DataContract]
    public struct ExternalSystemFields
    {
        [DataMember]
        public Dictionary<string, string> Field { get; set; }
    }
}
