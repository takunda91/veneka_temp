using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Veneka.Indigo.Audit.Interfaces;
using Veneka.Indigo.Audit.Interfaces.DataContracts;
using Veneka.Indigo.AuditManagement;

namespace IndigoCardIssuanceService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AuditInterface" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select AuditInterface.svc or AuditInterface.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(Namespace = AuditInterfaceConstants.IndigoAuditURL)]
    public class AuditInterface : IWcfAuditInterface
    {
        private readonly AuditController _auditController = new AuditController();

        public AuditResponse PostAudit(AuditEvent value)
        {
            //AuditController.LogAuditEvent();
            throw new NotImplementedException();
        }

        public AuditResponse PostAudits(AuditEvent[] value)
        {
            throw new NotImplementedException();
        }
    }
}
