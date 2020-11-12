using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Veneka.Indigo.Audit.Interfaces.DataContracts;

namespace Veneka.Indigo.Audit.Interfaces
{    
    [ServiceContract (Name = AuditInterfaceConstants.IndigoAuditName, 
                      ConfigurationName = AuditInterfaceConstants.IndigoAuditConfig, 
                      Namespace = AuditInterfaceConstants.IndigoAuditURL, 
                      ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
    public interface IWcfAuditInterface
    {
        /// <summary>
        /// Post single audit entry to Indigo
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [OperationContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
        AuditResponse PostAudit(AuditEvent value);

        /// <summary>
        /// Post multiple audits to Indigo. Audits will be persisted to the database in chronological order.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [OperationContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
        AuditResponse PostAudits(AuditEvent[] value);
    }
}
