using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Audit.Interfaces.DataContracts
{
    [DataContract(Namespace = AuditInterfaceConstants.IndigoAuditDataContractURL)]
    public sealed class AuditEvent
    {
        DateTimeOffset _auditDateTimeOffset = DateTimeOffset.UtcNow;
        string _responseCode;
        int _auditAction;
        string _auditDetail;
        System.Net.IPAddress _clientAddress;
        Guid _indigoID;
        int _issuerID;

        #region Constructors
        public AuditEvent() { }

        public AuditEvent(DateTimeOffset auditDateTime, string responseCode, int auditAction, string auditDetail, string clientAddress, string indigoID)
        {
            AuditDateTime = auditDateTime;
            ResponseCode = responseCode;
            AuditAction = auditAction;
            AuditDetail = auditDetail;
            ClientAddress = clientAddress;
            IndigoID = indigoID;
        }

        public AuditEvent(DateTimeOffset auditDateTime, string responseCode, int auditAction, string auditDetail, System.Net.IPAddress clientAddress, string indigoID)
        {
            AuditDateTime = auditDateTime;
            ResponseCode = responseCode;
            AuditAction = auditAction;
            AuditDetail = auditDetail;
            SetClientAddress(clientAddress);
            IndigoID = indigoID;
        }        

        public AuditEvent(DateTimeOffset auditDateTime, string responseCode, int auditAction, Exception auditException, System.Net.IPAddress clientAddress, string indigoID)
        {
            AuditDateTime = auditDateTime;
            ResponseCode = responseCode;
            AuditAction = auditAction;
            SetAuditDetailException(auditException);
            SetClientAddress(clientAddress);
            IndigoID = indigoID;
        }
        #endregion

        #region Properties/Data Members
        [DataMember(IsRequired = true)]
        public DateTimeOffset AuditDateTime
        {
            get { return _auditDateTimeOffset; }
            set { _auditDateTimeOffset = value; }
        }

        [DataMember]
        public string ResponseCode
        {
            get { return _responseCode; }
            set { _responseCode = value; }
        }

        [DataMember(IsRequired = true)]
        public int AuditAction
        {
            get { return _auditAction; }
            set { _auditAction = value; }
        }

        [DataMember]
        public string AuditDetail
        {
            get { return _auditDetail; }
            set { _auditDetail = value; }
        }

        [DataMember(IsRequired = true)]
        public string ClientAddress
        {
            get { return _clientAddress != null ? _clientAddress.ToString() : null; }
            set
            {
                if (!System.Net.IPAddress.TryParse(value, out _clientAddress))
                {
                    throw new FormatException(nameof(ClientAddress));
                }
            }
        }

        [DataMember(IsRequired = true)]
        public string IndigoID
        {
            get
            {
                return string.Format("{0}-{1}", _indigoID != null ? _indigoID.ToString() : null, _issuerID);
            }
            set
            {
                var trimmed = value.Replace("-", "");

                if (!Guid.TryParse(trimmed.Substring(0, 32), out _indigoID))
                {
                    throw new FormatException("Should be GUID-INT");
                }

                if(!Int32.TryParse(trimmed.Substring(32), out _issuerID))
                {
                    throw new FormatException("Should be GUID-INT");

                }
            }
        }
        #endregion

        #region Some Helper methods
        public void SetAuditDetailException(Exception exception)
        {
            _auditDetail = exception.ToString();
        }

        /// <summary>
        /// Returns the client address string as System.Net.IPAddress
        /// </summary>
        /// <returns>System.Net.IPAddress</returns>
        public System.Net.IPAddress GetClientAddress()
        {
            return _clientAddress;
        }

        /// <summary>
        /// Set the client address string using System.Net.IPAddress
        /// </summary>
        /// <param name="clientAddress">IP Address of the client</param>
        public void SetClientAddress(System.Net.IPAddress clientAddress)
        {
            _clientAddress = clientAddress;
        }

        /// <summary>
        /// Returns the Indigo ID string as a Guid object
        /// </summary>
        /// <returns></returns>
        public Guid GetIndigoID()
        {
            return _indigoID;
        }

        /// <summary>
        /// Set the Indigo ID string from Guid
        /// </summary>
        /// <param name="indigoID"></param>
        public void SetIndigoID(Guid indigoID)
        {
            _indigoID = indigoID;
        }

        public int GetIssuerID()
        {
            return _issuerID;
        }

        public void SetIssuerID(int issuerID)
        {
            _issuerID = issuerID;
        }
        #endregion
    }
}
