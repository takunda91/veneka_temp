using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common.Utilities;

namespace Veneka.Indigo.PINManagement.dal
{
    internal class TerminalManagementDAL
    {
        private readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;

        /// <summary>
        /// Create a new terminal
        /// </summary>
        /// <param name="terminal_name"></param>
        /// <param name="terminal_model"></param>
        /// <param name="session_key"></param>
        /// <param name="device_id"></param>
        /// <param name="branch_id"></param>
        /// <param name="terminal_masterkey_id"></param>
        /// <param name="audit_user_id"></param>
        /// <param name="audit_workstation"></param>
        /// <returns></returns>
        internal SystemResponseCode CreateTerminal(string TerminalName, string TerminalModel, string DeviceId,
            int BranchId, int TerminalMasterkeyId, string password, bool IsMacUsed, long AuditUserId, string AuditWorkstation, out int Terminalid)
        {
            ObjectParameter ResultCode1 = new ObjectParameter("ResultCode", typeof(int));
            ObjectParameter newTerminalId = new ObjectParameter("new_terminal_id", typeof(int));


            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_create_terminal(TerminalName, TerminalModel, DeviceId,
            BranchId, TerminalMasterkeyId, password, IsMacUsed, AuditUserId, AuditWorkstation, newTerminalId, ResultCode1);
            }


            Terminalid = 0;

            int resultCode = int.Parse(ResultCode1.Value.ToString());
            Terminalid = int.Parse(newTerminalId.Value.ToString());

            return (SystemResponseCode)resultCode;

        }

        /// <summary>
        /// Update an existing terminal
        /// </summary>
        /// <param name="terminal_id"></param>
        /// <param name="terminal_name"></param>
        /// <param name="terminal_model"></param>
        /// <param name="device_id"></param>
        /// <param name="branch_id"></param>
        /// <param name="terminal_masterkey_id"></param>
        /// <param name="audit_user_id"></param>
        /// <param name="audit_workstation"></param>
        /// <returns></returns>
        internal SystemResponseCode UpdateTerminal(int TerminalId, string TerminalName, string TerminalModel, string DeviceId,
            int BranchId, int TerminalMasterkeyId, string password, bool IsMacUsed, long AuditUserId, string AuditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_update_terminal(TerminalId, TerminalName, TerminalModel, DeviceId,
            BranchId, TerminalMasterkeyId, password, IsMacUsed, AuditUserId, AuditWorkstation, ResultCode);
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;
        }
        internal SystemResponseCode DeleteTerminal(int TerminalId, long AuditUserId, string AuditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_delete_terminaldetails(TerminalId, AuditUserId, AuditWorkstation, ResultCode);
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;
        }
        internal SystemResponseCode DeleteMasterkey(int? masterkeyid, long AuditUserId, string AuditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_delete_masterkey(masterkeyid, AuditUserId, AuditWorkstation, ResultCode);
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;
        }

        /// <summary>
        /// Get terminal by Id
        /// </summary>
        /// <param name="terminal_id"></param>
        /// <returns></returns>
        internal TerminalResult GetTerminals(int TerminalId)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<TerminalResult> results = context.usp_get_terminal(TerminalId);

                return results.First();
            }
        }

        /// <summary>
        /// Get a list of Terminals
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowsPerPage"></param>
        /// <returns></returns>
        internal List<TerminalListResult> GetTerminalsList(int? IssuerId,int? branchid, int LanguageId, int PageIndex, int RowsPerPage)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<TerminalListResult> results = context.usp_get_terminals_list(LanguageId, IssuerId,branchid, PageIndex, RowsPerPage);

                return results.ToList();
            }
        }

        /// <summary>
        /// Search for Terminals either by branches, masterkeys or id.
        /// </summary>
        /// <param name="terminal_id"></param>
        /// <param name="branch_id"></param>
        /// <param name="masterkey_id"></param>
        /// <returns></returns>
        internal List<TerminalListResult> SearchTerminals(int? IssuerId, int? BranchId, string terminalname, string deviceid, string terminalmodel, int PageIndex, int RowsPerPage)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<TerminalListResult> results = context.usp_search_terminal(IssuerId, BranchId, terminalname, deviceid, terminalmodel, PageIndex, RowsPerPage);

                return results.ToList();
            }
        }

        /// <summary>
        /// Get Terminal Masterkeys for devices
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal TerminalTMKResult GetTMKForTerminal(string deviceId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<TerminalTMKResult> results = context.usp_get_tmk_by_terminal(deviceId, auditUserId, auditWorkstation);

                return results.FirstOrDefault();
            }
        }

        /// <summary>
        /// Get Masterkey by issuer
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        internal List<TerminalTMKIssuerResult> GetTMKByIssuer(int issuerId,int PageIndex,int rowsPerpage, long auditUserId, string auditWorkStation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<TerminalTMKIssuerResult> results = context.usp_get_tmk_by_issuer(issuerId, null, PageIndex, rowsPerpage);

                return results.ToList();
            }
        }

        internal ZoneKeyResult GetZoneKey(int issuerId, long auditUserId, string auditWorkStation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<ZoneKeyResult> results = context.usp_get_zone_key(issuerId, auditUserId, auditWorkStation);

                return results.FirstOrDefault();
            }
        }

        /// <summary>
        /// Get Masterkey details
        /// </summary>
        /// <param name="masterkeyId"></param>
        /// <returns></returns>
        internal MasterkeyResult GetMasterkey(int masterkeyId)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<MasterkeyResult> results = context.usp_get_masterkey(masterkeyId);

                return results.FirstOrDefault();
            }
        }


        internal TerminalParametersResult LoadParameters(int productId, int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<TerminalParametersResult> results = context.usp_get_terminal_parameters(productId, auditUserId, auditWorkstation);

                return results.FirstOrDefault();
            }
        }

        /// <summary>
        /// Create a new masterkey
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="masterkey"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal SystemResponseCode CreateMasterkey(int IssuerId, string Masterkey, string MasterkeyName, long AuditUserId, string AuditWorkstation, out int newMasterkeyId)
        {

            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));
            ObjectParameter new_masterkey_id = new ObjectParameter("new_masterkey_id", typeof(int));


            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_create_masterkey(Masterkey, MasterkeyName, IssuerId, AuditUserId, AuditWorkstation, new_masterkey_id, ResultCode);
            }


            newMasterkeyId = 0;

            int resultCode = int.Parse(ResultCode.Value.ToString());
            newMasterkeyId = int.Parse(new_masterkey_id.Value.ToString());

            return (SystemResponseCode)resultCode;

        }

        /// <summary>
        /// Update Masterkey
        /// </summary>
        /// <param name="MasterkeyId"></param>
        /// <param name="Masterkey"></param>
        /// <param name="MasterkeyName"></param>
        /// <param name="IssuerId"></param>
        /// <param name="AuditUserId"></param>
        /// <param name="AuditWorkstation"></param>
        /// <returns></returns>
        internal SystemResponseCode UpdateMasterkey(int MasterkeyId, string Masterkey, string MasterkeyName, int IssuerId, long AuditUserId, string AuditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_update_masterkey(MasterkeyId, Masterkey, MasterkeyName, IssuerId, AuditUserId, AuditWorkstation, ResultCode);
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;
        }


    }
}
