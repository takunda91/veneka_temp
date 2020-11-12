using System;
using System.Collections.Generic;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.CCO.objects;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.utility;
using indigoCardIssuingWeb.security;
using Common.Logging;
using System.Linq;
using indigoCardIssuingWeb.SearchParameters;
using indigoCardIssuingWeb.Old_App_Code.SearchParameters;
using System.Web;
using System.Security.Principal;

namespace indigoCardIssuingWeb.Old_App_Code.service
{
    public class TerminalManagementService : BaseService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TerminalManagementService));
        //private static readonly Service1SoapClient _issuanceService = new Service1SoapClient();

        internal bool CreateTerminal(string terminal_name, string terminal_model, string device_id, int branch_id,
            int terminal_masterkey_id, string password, bool chkMacUsed, out int TerminalId, out string responsemessage)
        {

            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CreateTerminal(terminal_name, terminal_model, device_id,
                branch_id, terminal_masterkey_id,password,chkMacUsed, encryptedSessionKey);

            if (base.CheckResponse(response, log, out responsemessage))
            {
                TerminalId = int.Parse(response.Value.ToString());
                return true;
            }
            else
            {
                TerminalId = 0;
                responsemessage = response.ResponseMessage;
                //throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
                return false;
            }

            //if (response.ResponseType != ResponseType.SUCCESSFUL)
            //{
            //    TerminalId = 0;
            //    responsemessage = response.ResponseMessage;
            //    //throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
            //    return false;
            //}

            //responsemessage = response.ResponseMessage;
            //TerminalId = int.Parse(response.Value.ToString());
            //return true;
        }

        internal bool UpdateTerminal(int terminalId, string terminalName, string terminalModel, string deviceId, int branchId,
            int terminalMasterkeyId, string password, bool chkMacUsed, out string responsemessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.UpdateTerminal(terminalId, terminalName, terminalModel, deviceId,
                branchId, terminalMasterkeyId,password,chkMacUsed, encryptedSessionKey);

            return base.CheckResponse(response, log, out responsemessage);      
        }

        internal TerminalResult GetTerminals(int terminalId, int rowsPerPage, int pageIndex)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetTerminals(terminalId, rowsPerPage, pageIndex, encryptedSessionKey);

            if(base.CheckResponse(response, log))
                return response.Value;

            return null;
        }

        internal List<TerminalListResult> GetTerminalList(int? issuerId,int? branchid, int rowsPerPage, int pageIndex)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetTerminalsList(issuerId, branchid,rowsPerPage, pageIndex, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
        internal List<TerminalListResult> SearchTerminal(TerminalSearchParams parms,int PageIndex,int RowsPerpage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.SearchTerminals(parms.IssuerId, parms.BranchId, parms.Terminalname, parms.DeviceId, parms.TerminalModel,PageIndex,RowsPerpage, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
        internal List<TerminalListResult> GetSearchResults(int issuerId,int? branchid, int rowsPerPage, int pageIndex)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetTerminalsList(issuerId,branchid, rowsPerPage, pageIndex, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
        internal List<TerminalTMKIssuerResult> GetTMKByIssuer(int issuerId, int pageIndex, int rowsperpage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetTMKByIssuer(issuerId, pageIndex, rowsperpage, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value.ToList();

            return null;
        }
        internal bool CreateMasterkey(int issuerId, string masterkey, string masterkeyName, out int newmasterkeyid, out string responsemessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                            SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                            SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.CreateMasterkey(masterkey, masterkeyName, issuerId, encryptedSessionKey);

            if(base.CheckResponse(response, log, out responsemessage))
            {
                responsemessage = response.ResponseMessage;
                newmasterkeyid = int.Parse(response.Value.ToString());
                return true;
            }
            else
            {
                newmasterkeyid = 0;
                responsemessage = response.ResponseMessage;
                //throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
                return false;
            }

            //if (response.ResponseType != ResponseType.SUCCESSFUL)
            //{
            //    newmasterkeyid = 0;
            //    responsemessage = response.ResponseMessage;
            //    //throw new Exception(log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage);
            //    return false;
            //}

            //responsemessage = response.ResponseMessage;
            //newmasterkeyid = int.Parse(response.Value.ToString());
            //return true;
        }

        internal bool UpdateMasterkey(int masterkeyId, string masterkeyName, int issuerId, string masterkey, out string responsemessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                          SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                          SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.UpdateMasterkey(masterkeyId, masterkey, masterkeyName, issuerId, encryptedSessionKey);

            return base.CheckResponse(response, log, out responsemessage);
        }

        internal MasterkeyResult GetMasterkey(int masterkeyId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                       SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                       SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetMasterkey(masterkeyId, encryptedSessionKey);

            if (base.CheckResponse(response, log))
                return response.Value;

            return null;
        }


        internal string DeleteTerminal(int? TerminalId)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.DeleteTerminal((int)TerminalId, encryptedSessionKey);

            base.CheckResponse(response, log);

            return "";
        }
        internal bool DeleteMasterKey(int? MasterkeyId,out string responsemessage)
        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                   SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                   SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.DeleteMasterkey((int)MasterkeyId, encryptedSessionKey);

            return base.CheckResponseException(response, log, out responsemessage);
        }
    }
}