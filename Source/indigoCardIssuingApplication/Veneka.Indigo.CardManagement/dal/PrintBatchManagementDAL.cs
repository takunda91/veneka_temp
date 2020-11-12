using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.CardManagement.objects;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Common.Models;

namespace Veneka.Indigo.CardManagement.dal
{
   internal class PrintBatchManagementDAL
    {
        private readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;

        internal List<HybridRequestResult> SearchHybridRequestList(int issuerid, int? branchId, int? productId, int? hybrid_request_statusId, string request_reference,int? card_issue_method_id,bool check_masking, int languageId, int pageIndex, int rowsPerPage,
                                                                       long auditUserId, string auditWorkstation)
        {
            List<HybridRequestResult> rtnValue = new List<HybridRequestResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<HybridRequestResult> results = context.usp_search_request(issuerid,  productId,request_reference,hybrid_request_statusId, branchId,card_issue_method_id, check_masking, languageId,pageIndex, rowsPerPage, auditUserId, auditWorkstation);

                foreach (var result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }
        internal SystemResponseCode SpoilPrintBatch(long print_batch_id,int? new_print_batch_statuses_id, List<RequestData> requestData,string notes,  int languageId, 
                                                                       long auditUserId, string auditWorkstation)
        {
            int resultCode;

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    DataTable dt_RequestList = new DataTable();
                    dt_RequestList.Columns.Add("request_id", typeof(int));
                    dt_RequestList.Columns.Add("request_statues_id", typeof(int));
                    dt_RequestList.Columns.Add("card_number", typeof(string));

                    DataRow workRow;

                    foreach (var item in requestData)
                    {
                        workRow = dt_RequestList.NewRow();
                        workRow["request_id"] = item.request_id;
                        workRow["request_statues_id"] = item.request_statues_id;
                        workRow["card_number"] = item.card_number;
                        dt_RequestList.Rows.Add(workRow);
                    }

                    
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_print_batch_spoil]";

                    command.Parameters.Add("@print_batch_id", SqlDbType.Int).Value = print_batch_id;
                    command.Parameters.Add("@new_print_batch_statuses_id", SqlDbType.Int).Value = new_print_batch_statuses_id;

                    command.Parameters.Add("@status_notes", SqlDbType.NVarChar).Value = notes;

                    command.Parameters.AddWithValue("@request_list", dt_RequestList);
                    command.Parameters.AddWithValue("@language_id", languageId);

                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.NVarChar).Value = auditWorkstation;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    resultCode = int.Parse(command.Parameters["@ResultCode"].Value.ToString());
                }
            }

            return (SystemResponseCode)resultCode;
        }

        internal bool InsertWorkStationKey(string Workstation, string key,string aData)
        {
          
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_insert_workstation_key(Workstation, key, aData);

               
            }

            return true;
        }

        internal workstationkeys_result GetWorkStationKey(string Workstation)
        {
            workstationkeys_result rtnValue = new workstationkeys_result();

            string key = string.Empty;
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {

                ObjectResult<workstationkeys_result> results =    context.usp_get_workstation_key(Workstation);


                foreach (var result in results)
                {
                    return result;
                }
            }

            return null;
        }
        internal int GetStockinBranch(int issuerid, int? branchId, int? productId, int? card_issue_method_id,
                                                                       long auditUserId, string auditWorkstation)
        {
            ObjectParameter cardcount = new ObjectParameter("cardcount", typeof(int));

            List<HybridRequestResult> rtnValue = new List<HybridRequestResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                 context.usp_get_mainbranch_card_count(issuerid,branchId, productId, card_issue_method_id, auditUserId, auditWorkstation, cardcount);

                if (cardcount.Value != null)
                    return int.Parse(cardcount.Value.ToString());
            }

            return 0;
        }
        internal List<PrintBatchResult> GetPrintBatchesForUser(int? issuerId,int? productId, string printBatchReference, int? printBatchStatusId, int? branchId, int? cardIssueMethodId,
                                                               DateTime? startDate, DateTime? endDate, int langaugeId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<PrintBatchResult> results = context.usp_get_print_batches_for_user(issuerId,productId, printBatchReference, printBatchStatusId, branchId, cardIssueMethodId, startDate,
                                                                                                    endDate, langaugeId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);

                return results.ToList();
            }
        }



        internal PrintBatchResult GetPrintBatch(long printBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<PrintBatchResult> results = context.usp_get_print_batch(printBatchId, languageId, auditUserId, auditWorkstation);

                return results.First();
            }
        }

        internal SystemResponseCode UpdatePrintBatchChangeStatus(long printBatchId, int? printBatchStatusesId, int? newprintBatchStatusesId, string notes, bool autogenerateDistBatch, int languageId, long auditUserId, string auditWorkstation, out PrintBatchResult result)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<PrintBatchResult> results = context.usp_print_batch_status_change(printBatchId, printBatchStatusesId, newprintBatchStatusesId, notes, languageId, auditUserId, auditWorkstation, autogenerateDistBatch, ResultCode);

                result = results.First();

                return (SystemResponseCode)int.Parse(ResultCode.Value.ToString());
            }
        }
        internal SystemResponseCode UpdatePrintBatchRequestsStatus(long print_batch_id, int print_batch_statuses_id, bool Successful, List<RequestData> requestdata, List<string> cardstospoil,string notes, long auditUserId, string auditWorkstation)
        {
            int resultCode;

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    DataTable dt_RequestList = new DataTable();
                    dt_RequestList.Columns.Add("request_id", typeof(int));
                    dt_RequestList.Columns.Add("request_statues_id", typeof(int));
                    dt_RequestList.Columns.Add("card_number", typeof(string));

                    DataRow workRow;

                    foreach (var item in requestdata)
                    {
                        workRow = dt_RequestList.NewRow();
                        workRow["request_id"] = item.request_id;
                        workRow["request_statues_id"] = item.request_statues_id;
                        workRow["card_number"] = item.card_number;
                        dt_RequestList.Rows.Add(workRow);
                    }

                    DataTable dt_cardsList = new DataTable();
                    dt_cardsList.Columns.Add("card_number", typeof(string));

                    foreach (var item in cardstospoil)
                    {
                        workRow = dt_cardsList.NewRow();
                        workRow["card_number"] = item.ToString();
                        dt_cardsList.Rows.Add(workRow);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_print_batch_request_status_change]";

                    command.Parameters.Add("@print_batch_id", SqlDbType.BigInt).Value = print_batch_id;
                    command.Parameters.Add("@print_batch_statuses_id", SqlDbType.Int).Value = print_batch_statuses_id;
                    command.Parameters.Add("@Successful", SqlDbType.Bit).Value = Successful;
                    command.Parameters.Add("@status_notes", SqlDbType.NVarChar).Value = notes;
                    
                    command.Parameters.AddWithValue("@request_list", dt_RequestList);
                    command.Parameters.AddWithValue("@card_to_be_spoiled", dt_cardsList);

                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.NVarChar).Value = auditWorkstation;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    resultCode = int.Parse(command.Parameters["@ResultCode"].Value.ToString());
                }
            }

            return (SystemResponseCode)resultCode;
        }
        internal SystemResponseCode UploadPrintBatchToCMS(long printBatchId, int? newPrintBatchStatusesId, List<RequestData> requestdata, bool autogenerateDistBatch,string notes, int languageId, long auditUserId, string auditWorkstation)
        {
            int resultCode;

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    DataTable dt_RequestList = new DataTable();
                    dt_RequestList.Columns.Add("request_id", typeof(int));
                    dt_RequestList.Columns.Add("request_statues_id", typeof(int));
                    dt_RequestList.Columns.Add("card_number", typeof(string));

                    DataRow workRow;

                    foreach (var item in requestdata)
                    {
                        workRow = dt_RequestList.NewRow();
                        workRow["request_id"] = item.request_id;
                        workRow["request_statues_id"] = item.request_statues_id;
                        workRow["card_number"] = item.card_number;
                        dt_RequestList.Rows.Add(workRow);
                    }

                   

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_print_batch_status_uploadtocms]";

                    command.Parameters.Add("@print_batch_id", SqlDbType.BigInt).Value = printBatchId;
                    //command.Parameters.Add("@print_batch_statuses_id", SqlDbType.Int).Value = printBatchStatusesId;
                    command.Parameters.Add("@new_print_batch_statuses_id", SqlDbType.Int).Value = newPrintBatchStatusesId;

                    command.Parameters.Add("@status_notes", SqlDbType.NVarChar).Value = notes;

                    command.Parameters.AddWithValue("@request_list", dt_RequestList);
                    command.Parameters.Add("@language_id", SqlDbType.Int).Value = auditUserId;
                    command.Parameters.Add("@autogenerate_dist_batch_id", SqlDbType.Bit).Value = autogenerateDistBatch;

                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.NVarChar).Value = auditWorkstation;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    resultCode = int.Parse(command.Parameters["@ResultCode"].Value.ToString());
                }
            }

            return (SystemResponseCode)resultCode;
        }
        internal List<HybridRequestResult> GetPrintBatchRequests(long printBatchId,int startindex,int size, int languageId, long auditUserId, string auditWorkStation)
        {
            List<HybridRequestResult> rtnValue = new List<HybridRequestResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<HybridRequestResult> results = context.usp_get_print_batch_requests(printBatchId, languageId, startindex, size, auditUserId, auditWorkStation);

                foreach (var result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }

        internal List<PrintBatchHistoryResult> GetPrintBatchHistory(long printBatchId, int langaugeId,  long auditUserId, string auditWorkStation)
        {
            List<PrintBatchHistoryResult> rtnValue = new List<PrintBatchHistoryResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<PrintBatchHistoryResult> results = context.usp_get_print_batch_history(printBatchId,  langaugeId, auditUserId, auditWorkStation);

                foreach (var result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }

    }
}
