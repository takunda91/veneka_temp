using System;

namespace Veneka.Indigo.AuditManagement.objects
{
    public class Audit
    {
        private readonly string _actionDescription;
        private readonly DateTime _auditDate;
        private readonly long _auditID;
        private readonly string _dataAfter;
        private readonly string _dataBefore;
        private readonly string _dataChanged;
        private readonly int _issuerID;
        private readonly string _userAction;
        private readonly string _userAudit;
        private readonly string _workstationAddress;

        public Audit(long auditID, string userAudit
                     , string workstationAddress
                     , string userAction
                     , string actionDescription
                     , DateTime auditDate
                     , string dataChanged
                     , string dataBefore
                     , string dataAfter
                     , int issuerID)
        {
            _auditID = auditID;
            _userAudit = userAudit;
            _workstationAddress = workstationAddress;
            _userAction = userAction;
            _actionDescription = actionDescription;
            _auditDate = auditDate;
            _dataChanged = dataChanged;
            _dataBefore = dataBefore;
            _dataAfter = dataAfter;
            _issuerID = issuerID;
        }

        public long AuditID
        {
            get { return _auditID; }
        }

        public string UserAudit
        {
            get { return _userAudit; }
        }

        public string WorkstationAddress
        {
            get { return _workstationAddress; }
        }

        public string UserAction
        {
            get { return _userAction; }
        }

        public string ActionDescription
        {
            get { return _actionDescription; }
        }

        public DateTime AuditDate
        {
            get { return _auditDate; }
        }

        public string DataChanged
        {
            get { return _dataChanged; }
        }

        public string DataBefore
        {
            get { return _dataBefore; }
        }

        public string DataAfter
        {
            get { return _dataAfter; }
        }

        public int IssuerID
        {
            get { return _issuerID; }
        }

        public string Issuer { get; set; }
    }
}