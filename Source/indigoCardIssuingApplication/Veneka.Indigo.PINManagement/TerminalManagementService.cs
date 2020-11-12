using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.PINManagement.dal;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common.Language;
using Veneka.Indigo.Common;

namespace Veneka.Indigo.PINManagement
{
    public class TerminalManagementService
    {
        private readonly TerminalManagementDAL _terminalManager = new TerminalManagementDAL();
        private readonly ResponseTranslator _translator = new ResponseTranslator();

        public bool CreateTerminal(string TerminalName, string TerminalModel, string DeviceId,
            int BranchId, int TerminalMasterkeyId, string password, bool IsMacUsed, long AuditUserId, string AuditWorkstation, int languageId, out int terminalid, out string responseMessage)
        {
          
           
            var response = _terminalManager.CreateTerminal(TerminalName, TerminalModel, DeviceId,
            BranchId, TerminalMasterkeyId, password, IsMacUsed, AuditUserId, AuditWorkstation, out terminalid);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, AuditUserId, AuditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }
             return false;
        }

        public bool UpdateTerminal(int TerminalId, string TerminalName, string TerminalModel, string DeviceId, int BranchId,
            int TerminalMasterkeyId, string password,bool IsMacUsed,long AuditUserId, string AuditWorkstation, int languageId, out string responseMessage)
        {
            var response = _terminalManager.UpdateTerminal(TerminalId, TerminalName, TerminalModel, DeviceId, BranchId,
                TerminalMasterkeyId, password, IsMacUsed, AuditUserId, AuditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, AuditUserId, AuditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }
        public bool DeleteTerminal(int TerminalId, long AuditUserId, string AuditWorkstation, int languageId, out string responseMessage)
        {
            var response = _terminalManager.DeleteTerminal(TerminalId, AuditUserId, AuditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, AuditUserId, AuditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }
        public bool DeleteMasterkey(int MasterkeyId, long AuditUserId, string AuditWorkstation, int languageId, out string responseMessage)
        {
            var response = _terminalManager.DeleteMasterkey(MasterkeyId,  AuditUserId, AuditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, AuditUserId, AuditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }
        public TerminalResult GetTerminals(int TerminalId)
        {
            var result = _terminalManager.GetTerminals(TerminalId);

            return result;
        }

        public List<TerminalListResult> GetTerminalsList(int? IssuerId,int? BranchId, int LanguageId, int PageIndex, int RowsPerPage)
        {
            return _terminalManager.GetTerminalsList(IssuerId,BranchId, LanguageId, PageIndex, RowsPerPage);
        }

        public MasterkeyResult GetMasterkey(int masterkeyId)
        {
            return _terminalManager.GetMasterkey(masterkeyId);
        }

        public List<TerminalListResult> SearchTerminals(int? IssuerId, int? BranchId, string terminalname, string deviceid, string terminalmodel, int PageIndex, int RowsPerPage)
        {
            return _terminalManager.SearchTerminals(IssuerId, BranchId, terminalname, deviceid, terminalmodel,  PageIndex,  RowsPerPage);
        }

        public TerminalTMKResult GetTMKForTerminal(string DeviceId, long AuditUserId, string AuditWorkstation)
        {
            return _terminalManager.GetTMKForTerminal(DeviceId, AuditUserId, AuditWorkstation);
        }

        public List<TerminalTMKIssuerResult> GetTMKByIssuer(int IssuerId,int PageIndex,int rowsPerpage, long AuditUserId, string AuditWorkstation)
        {
            return _terminalManager.GetTMKByIssuer(IssuerId,PageIndex,rowsPerpage, AuditUserId, AuditWorkstation);

        }

        public ZoneKeyResult GetZoneKey(int issuerId, long auditUserId, string auditWorkstation)
        {
            return _terminalManager.GetZoneKey(issuerId, auditUserId, auditWorkstation);
        }

        public TerminalParametersResult LoadParameters(int productId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _terminalManager.LoadParameters(productId, languageId, auditUserId, auditWorkstation);
        }

        public bool CreateMasterkey(string Masterkey, string MasterkeyName, int IssuerId, long AuditUserId, string AuditWorkstation,int languageId, out int masterkeyid,out string responseMessage)
        {
           
            var response =  _terminalManager.CreateMasterkey(IssuerId, Masterkey, MasterkeyName, AuditUserId, AuditWorkstation, out masterkeyid);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, AuditUserId, AuditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }
            return false;
        }

        public bool UpdateMasterkey(int MasterkeyId, string Masterkey, string MasterkeyName, int IssuerId, long AuditUserId, string AuditWorkstation,int languageId,out string responseMessage)
        {
            var response = _terminalManager.UpdateMasterkey(MasterkeyId, Masterkey, MasterkeyName, IssuerId, AuditUserId, AuditWorkstation);
            responseMessage = _translator.TranslateResponseCode(response, SystemArea.GENERIC, languageId, AuditUserId, AuditWorkstation);

            if (response == SystemResponseCode.SUCCESS)
            {
                return true;
            }

            return false;
        }        
    }
}
