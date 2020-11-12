using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Text;
using System.Linq;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.CardManagement.objects;
using System.Reflection;

namespace Veneka.Indigo.CardManagement.dal
{
    internal class DistBatchManagementDAL
    {
        private readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;

        #region EXPOSED METHODS

        /// <summary>
        /// Returns a list of distribution batches for a user. dist batch status and dates may be null to not do a filter on them.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="issuerId"></param>
        /// <param name="distBatchReference"></param>
        /// <param name="distBatchStatus"></param>
        /// <param name="branchId"></param>
        /// <param name="cardIssueMethodId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="langaugeId"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal List<DistBatchResult> GetDistBatchesForUser(long userId, int? issuerId, string distBatchReference, int? distBatchStatusId, int? flowdistBatchStatusId, int? branchId, int? cardIssueMethodId,
                                                          int? distBatchTypeId, DateTime? startDate, DateTime? endDate, bool? includeOriginBranch, int langaugeId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {
            List<DistBatchResult> rtnValue = new List<DistBatchResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<DistBatchResult> results = context.usp_get_dist_batches_for_user(userId, issuerId, distBatchReference, distBatchStatusId, flowdistBatchStatusId, branchId, cardIssueMethodId, distBatchTypeId, startDate,
                                                                                                    endDate, includeOriginBranch, langaugeId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);

                foreach (DistBatchResult result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }
        /// <summary>
        /// Get Dist Batches for Status
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="issuerId"></param>
        /// <param name="distBatchReference"></param>
        /// <param name="distBatchStatusId"></param>
        /// <param name="branchId"></param>
        /// <param name="cardIssueMethodId"></param>
        /// <param name="distBatchTypeId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="langaugeId"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal List<DistStatusResult> GetDistBatchesForStatus(long userId, int? issuerId, string distBatchReference, int? distBatchStatusId, int? branchId, int? cardIssueMethodId,
                                                               int? distBatchTypeId, DateTime? startDate, DateTime? endDate, bool cardCountForRedist, int langaugeId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {
            List<DistStatusResult> rtnValue = new List<DistStatusResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<DistStatusResult> results = context.usp_get_dist_batches_for_status(userId, issuerId, distBatchReference, distBatchStatusId, branchId, cardIssueMethodId, distBatchTypeId, startDate,
                                                                                                    endDate, cardCountForRedist, langaugeId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);

                foreach (DistStatusResult result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }

        internal int GetDistBatchCardCountForRedist(long distBatchId, int productId, int languageId, long auditUserId, string auditWorkstation)
        {
            List<DistStatusResult> rtnValue = new List<DistStatusResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<int?> result = context.usp_get_dist_batch_card_count_for_distribution(distBatchId, productId, languageId, auditUserId, auditWorkstation);

                return result.FirstOrDefault().Value;
            }
        }

        /// <summary>
        /// Retrieve the dist batch
        /// </summary>
        /// <param name="distBatchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal DistBatchResult GetDistBatch(long distBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            List<DistBatchResult> rtnValue = new List<DistBatchResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<DistBatchResult> results = context.usp_get_dist_batch(distBatchId, languageId, auditUserId, auditWorkstation);

                foreach (DistBatchResult result in results)
                {
                    rtnValue.Add(result);
                }

                if (rtnValue.Count > 0)
                {
                    return rtnValue[0];
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a list of cards and their details which are linked to the distribution batch
        /// </summary>
        /// <param name="distBatchId"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal List<DistBatchCardDetail> GetDistBatchCardDetails(long distBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<DistBatchCardDetail> results = context.usp_get_dist_batch_card_details(distBatchId, auditUserId, auditWorkstation);

                return results.ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distBatchId"></param>
        /// <param name="languageId"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="pageIndex"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal List<BatchCardDetailsPagedResult> GetDistBatchCardDetailsPaged(long distBatchId, int languageId, int rowsPerPage, int pageIndex, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<BatchCardDetailsPagedResult> results = context.usp_get_dist_batch_card_details_paged(distBatchId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);

                return results.ToList();
            }
        }

        /// <summary>
        /// Return a list of cards linked to the specified distribution batch.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        internal List<DistCardResult> GetDistBatchCards(long distBatchId, long auditUserId, string auditWorkStation)
        {
            List<DistCardResult> rtnValue = new List<DistCardResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<DistCardResult> results = context.usp_get_dist_batch_cards(distBatchId, auditUserId, auditWorkStation);

                foreach (var result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }

        /// <summary>
        /// Fetch a distribution batch status history.
        /// </summary>
        /// <param name="distBatchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        internal List<DistBatchHistoryResult> GetDistBatchHistory(long distBatchId, int languageId, long auditUserId, string auditWorkStation)
        {
            List<DistBatchHistoryResult> rtnValue = new List<DistBatchHistoryResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<DistBatchHistoryResult> results = context.usp_get_dist_batch_history(distBatchId, languageId, auditUserId, auditWorkStation);

                foreach (var result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }

        /// <summary>
        /// Create a new distribution batch
        /// </summary>
        /// <param name="branchId">Where this distribution batch is to be sent.</param>
        /// <param name="batchCardSize">The number of cards the distribution batch must have.</param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal DBResponseMessage CreateDistributionBatch(int issuerId, int branchId, int toBranchId, int cardIssueMethodId,
                                                            int productId, int batchCardSize,
                                                            int createBatchOption, string startRef, string endRef, long? fromdistbatchid,
                                                            long auditUserId, string auditWorkstation, out string dist_batch_refnumber, out int dist_batch_id)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));
            ObjectParameter dist_batch_ref = new ObjectParameter("dist_batch_refnumber", typeof(string));
            ObjectParameter dist_batchid = new ObjectParameter("dist_batchid", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_create_dist_batch(issuerId, branchId, toBranchId, cardIssueMethodId,
                                              productId, batchCardSize, createBatchOption, startRef, endRef,
                                              auditUserId, auditWorkstation, fromdistbatchid, ResultCode, dist_batchid, dist_batch_ref);
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());
            dist_batch_refnumber = dist_batch_ref.Value.ToString();
            dist_batch_id = int.Parse(dist_batchid.Value.ToString());
            return (DBResponseMessage)resultCode;
        }

        internal DBResponseMessage GetCentralBatchReferenceNumbers(int issuerId, int branchId, int toBranchId, int cardIssueMethodId,
                                                            int productId, out string startRef, out string endRef)
        {
            int resultCode;
            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_central_batch_ref_numbers]";

                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuerId;
                    command.Parameters.Add("@branch_id", SqlDbType.Int).Value = branchId;
                    command.Parameters.Add("@to_branch_id", SqlDbType.NVarChar).Value = toBranchId;

                    command.Parameters.Add("@product_id", SqlDbType.Int).Value = productId;
                    command.Parameters.Add("@card_issue_method_id", SqlDbType.Int).Value = cardIssueMethodId;

                    command.Parameters.Add("@start_ref", SqlDbType.Int).Direction = ParameterDirection.Output;
                    command.Parameters.Add("@end_ref", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    startRef = (command.Parameters["@start_ref"].Value.ToString());
                    endRef = (command.Parameters["@end_ref"].Value.ToString());
                    resultCode = int.Parse(command.Parameters["@ResultCode"].Value.ToString());
                }
            }

            return (DBResponseMessage)resultCode;
        }

        /// <summary>
        /// Create a new card stock order
        /// </summary>        
        /// <returns></returns>
        internal SystemResponseCode CreateCardStockOrder(int issuerId, int branchId, int productId, int cardIssuerMethodId, int cardPriorityId, int batchCardSize, long auditUserId, string auditWorkstation, out CardRequestBatchResponse cardReqResponse)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));
            ObjectParameter distBatchId = new ObjectParameter("dist_batch_id", typeof(long));
            ObjectParameter distBatchRef = new ObjectParameter("dist_batch_ref", typeof(string));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.CommandTimeout = 55;
                context.usp_request_card_stock(issuerId, branchId, productId, cardPriorityId, cardIssuerMethodId, batchCardSize, auditUserId, auditWorkstation, distBatchId, distBatchRef, ResultCode);
            }


            CardRequestBatchResponse cardreq = new CardRequestBatchResponse();
            cardreq.DistBatchId = int.Parse(distBatchId.Value.ToString());
            cardreq.DistBatchRef = distBatchRef.Value.ToString();
            cardReqResponse = cardreq;
            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;
        }

        #region Classic Card
        /// <summary>
        /// Create distribution batch based on the request for cards input parameter.
        /// </summary>
        /// <param name="cardIssueMethodId"></param>
        /// <param name="branchId"></param>
        /// <param name="productId"></param>
        /// <param name="cardPriorityId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="cardReqResponse"></param>
        /// <returns></returns>
        internal SystemResponseCode CreateCardRequestBatch(int cardIssueMethodId, int issuerId, int? branchId, int productId, int cardPriorityId,
                                                            long auditUserId, string auditWorkstation, out CardRequestBatchResponse cardReqResponse)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));
            ObjectParameter CardsInBatch = new ObjectParameter("cards_in_batch", typeof(int));
            ObjectParameter DistBatchId = new ObjectParameter("dist_batch_id", typeof(int));
            ObjectParameter DistBatchRef = new ObjectParameter("dist_batch_ref", typeof(string));
            int resultCode = -1;
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_request_create_dist_batch(cardIssueMethodId, issuerId, branchId, productId, cardPriorityId,
                                                        auditUserId, auditWorkstation, CardsInBatch, DistBatchId, DistBatchRef, ResultCode);
                CardRequestBatchResponse req = new CardRequestBatchResponse();
                req.CardsInBatch = int.Parse(CardsInBatch.Value.ToString());
                req.DistBatchId = int.Parse(DistBatchId.Value.ToString());
                req.DistBatchRef = DistBatchRef.Value.ToString();
                cardReqResponse = req;

                resultCode = int.Parse(ResultCode.Value.ToString());
            }



            return (SystemResponseCode)resultCode;
        }


        internal SystemResponseCode CreateHybridRequestBatch(int cardIssueMethodId, int issuerId, int? branchId, int productId, int cardPriorityId,
                                                            long auditUserId, string auditWorkstation, out HybridRequestBatchResponse hybridReqResponse)
        {
            int resultCode = 100;

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_request_hybrid_create_print_batch]";

                    command.Parameters.Add("@card_issue_method_id", SqlDbType.Int).Value = cardIssueMethodId;
                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuerId;
                    command.Parameters.Add("@branch_id", SqlDbType.Int).Value = branchId;
                    command.Parameters.Add("@product_id", SqlDbType.Int).Value = productId;
                    command.Parameters.Add("@card_priority_id", SqlDbType.Int).Value = cardPriorityId;

                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar, 500).Value = auditWorkstation;
                    command.Parameters.Add("@requests_in_batch", SqlDbType.Int).Direction = ParameterDirection.Output;
                    command.Parameters.Add("@print_batch_id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    command.Parameters.Add("@print_batch_ref", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();
                    HybridRequestBatchResponse req = new HybridRequestBatchResponse();
                    req.PinBatchId = Convert.ToInt32(command.Parameters["@print_batch_id"].Value);
                    req.PinBatchRef = command.Parameters["@print_batch_ref"].Value.ToString();
                    req.RequestsInBatch = Convert.ToInt32(command.Parameters["@requests_in_batch"].Value);
                    hybridReqResponse = req;

                    resultCode = Convert.ToInt32(command.Parameters["@ResultCode"].Value);

                }
            }

            //ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));
            //ObjectParameter requestsInBatch = new ObjectParameter("requests_in_batch", typeof(int));
            //ObjectParameter printBatchId = new ObjectParameter("print_batch_id", typeof(int));
            //ObjectParameter printBatchRef = new ObjectParameter("print_batch_ref", typeof(string));
            //int resultCode = -1;
            //using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            //{
            //    context.usp_request_hybrid_create_print_batch(cardIssueMethodId, issuerId, branchId, productId, cardPriorityId,
            //                                            auditUserId, auditWorkstation, requestsInBatch, printBatchId, printBatchRef, ResultCode);


            //    HybridRequestBatchResponse req = new HybridRequestBatchResponse();
            //    req.PinBatchId = int.Parse(printBatchId.Value.ToString());
            //    req.PinBatchRef = printBatchRef.Value.ToString();
            //    req.RequestsInBatch = int.Parse(requestsInBatch.Value.ToString());
            //    hybridReqResponse = req;

            //    resultCode = int.Parse(ResultCode.Value.ToString());
            //}

            return (SystemResponseCode)resultCode;
        }
        /// <summary>
        /// To Get Cards Requests List
        /// </summary>
        /// <param name="cardIssueMethodId"></param>
        /// <param name="issuerid"></param>
        /// <param name="branchId"></param>
        /// <param name="languageid"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal List<card_request_result> GetCardRequestList(int issuerid, int? branchId, int languageId, int pageIndex, int rowsPerPage,
                                                                            long auditUserId, string auditWorkstation)
        {
            List<card_request_result> rtnValue = new List<card_request_result>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<card_request_result> results = context.usp_get_cardrequests(issuerid, branchId, languageId, rowsPerPage, pageIndex, auditUserId, auditWorkstation);

                foreach (var result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }

        internal List<card_request_result> GetCardRequestList(int issuerId, int? branchId, int cardIssueMethodId, int languageId, int pageIndex, int rowsPerPage,
                                                                           long auditUserId, string auditWorkstation)
        {
            List<card_request_result> rtnValue = new List<card_request_result>();

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_cardrequests_by_issue_method]";

                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = issuerId;
                    command.Parameters.Add("@branch_id", SqlDbType.Int).Value = branchId;
                    command.Parameters.Add("@card_issue_method_id", SqlDbType.Int).Value = cardIssueMethodId;
                    command.Parameters.Add("@languageId", SqlDbType.Int).Value = languageId;
                    command.Parameters.Add("@PageIndex", SqlDbType.Int).Value = pageIndex;
                    command.Parameters.Add("@RowsPerPage", SqlDbType.Int).Value = rowsPerPage;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar, 500).Value = auditWorkstation;

                    DataTable dataTable = new DataTable();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        dataTable.Load(reader);
                    }

                    foreach (DataRow row in dataTable.Rows)
                    {
                        rtnValue.Add(new card_request_result
                        {
                            cardscount = row["cardscount"] != DBNull.Value ? Convert.ToInt32(row["cardscount"]) : 0,
                            card_priority_id = row["card_priority_id"] != DBNull.Value ? Convert.ToInt32(row["card_priority_id"]) : 0,
                            card_priority_name = row["card_priority_name"] != DBNull.Value ? Convert.ToString(row["card_priority_name"]) : string.Empty,
                            product_id = row["product_id"] != DBNull.Value ? Convert.ToInt32(row["product_id"]) : 0,
                            product_name = row["product_name"] != DBNull.Value ? Convert.ToString(row["product_name"]) : string.Empty
                        });
                    }
                }
                return rtnValue;
            }
            //using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            //{
            //    ObjectResult<card_request_result> results = context.usp_get_cardrequests(issuerid, branchId, languageId, rowsPerPage, pageIndex, auditUserId, auditWorkstation);

            //    foreach (var result in results)
            //    {
            //        rtnValue.Add(result);
            //    }
            //}

            return rtnValue;
        }
        /// <summary>
        /// to get requestes count to create print batch
        /// </summary>
        /// <param name="issuerid"></param>
        /// <param name="branchId"></param>
        /// <param name="languageId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal List<hybrid_request_result> GetHybridRequestList(int issuerid, int? branchId, int? productId, int? hybrid_request_statusId, string batch_refrence, int languageId, int pageIndex, int rowsPerPage,
                                                                       long auditUserId, string auditWorkstation)
        {
            List<hybrid_request_result> rtnValue = new List<hybrid_request_result>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<hybrid_request_result> results = context.usp_get_hybridrequests(issuerid, branchId, languageId, rowsPerPage, pageIndex, auditUserId, auditWorkstation);

                foreach (var result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }

        internal List<PrintBatchResult> GetPrintBatchesList(int issuerid, int? branchId, int? productId, string print_batch_refrence, int? print_statues_id, int card_issue_method_id, DateTimeOffset startdate, DateTimeOffset enddate, int languageId, int pageIndex, int rowsPerPage,
                                                                       long auditUserId, string auditWorkstation)
        {
            List<PrintBatchResult> rtnValue = new List<PrintBatchResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<PrintBatchResult> results = context.usp_get_print_batches_for_user(issuerid, productId, print_batch_refrence, print_statues_id, branchId, card_issue_method_id, startdate, enddate, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);

                foreach (var result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }

        internal bool CheckIfCMSUploadCenterOperator(long distBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            bool centerOperatorActivation = false;
            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_dist_batch_activate_credit_card]";

                    command.Parameters.Add("@dist_batch_id", SqlDbType.Int).Value = distBatchId;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar, 500).Value = auditWorkstation;
                    command.Parameters.Add("@activation_by_center_operator", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    centerOperatorActivation = Convert.ToBoolean(command.Parameters["@activation_by_center_operator"].Value);
                }
            }
            return centerOperatorActivation;
        }

        internal bool CheckIfCMSUploadRenewalCards(long distBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            bool centerOperatorActivation = false;
            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_get_dist_batch_activate_renewal]";

                    command.Parameters.Add("@dist_batch_id", SqlDbType.Int).Value = distBatchId;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar, 500).Value = auditWorkstation;
                    command.Parameters.Add("@activation_on_renewal", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    centerOperatorActivation = Convert.ToBoolean(command.Parameters["@activation_on_renewal"].Value);
                }
            }
            return centerOperatorActivation;
        }




        #endregion

        /// <summary>
        /// Reject 
        /// </summary>
        /// <param name="distBatchId"></param>
        /// <param name="notes"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="resultCode"></param>
        /// <returns></returns>
        internal SystemResponseCode DistBatchRejectStatus(long distBatchId, int distBatchStatusesId, string notes, int languageId, long auditUserId, string auditWorkstation, out DistBatchResult result)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<DistBatchResult> results = context.usp_dist_batch_status_reject(distBatchId, distBatchStatusesId, notes, languageId, auditUserId, auditWorkstation, ResultCode);

                result = results.First();

                return (SystemResponseCode)int.Parse(ResultCode.Value.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distBatchId"></param>
        /// <param name="distBatchStatusesId"></param>
        /// <param name="flowDistBatchStatusesId"></param>
        /// <param name="notes"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        internal SystemResponseCode DistBatchChangeStatus(long distBatchId, int? distBatchStatusesId, int? flowDistBatchStatusesId, string notes, int languageId, long auditUserId, string auditWorkstation, bool autogenerateDistBatch, out DistBatchResult result)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<DistBatchResult> results = context.usp_dist_batch_status_change(distBatchId, distBatchStatusesId, flowDistBatchStatusesId, notes, languageId, auditUserId, auditWorkstation, autogenerateDistBatch, ResultCode);

                result = results.First();

                return (SystemResponseCode)int.Parse(ResultCode.Value.ToString());
            }
        }

        internal SystemResponseCode DistBatchChangeStatusRenewal(long distBatchId, int? distBatchStatusesId, int? flowDistBatchStatusesId, string notes, int languageId, long auditUserId, string auditWorkstation, bool autogenerateDistBatch, out DistBatchResult result)
        {
            int resultCode;
            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_dist_batch_status_change_renewal]";

                    command.Parameters.Add("@dist_batch_id", SqlDbType.BigInt).Value = distBatchId;
                    command.Parameters.Add("@dist_batch_statuses_id", SqlDbType.Int).Value = distBatchStatusesId;
                    command.Parameters.Add("@new_dist_batch_statuses_id", SqlDbType.Int).Value = flowDistBatchStatusesId;
                    command.Parameters.Add("@status_notes", SqlDbType.VarChar).Value = notes;
                    command.Parameters.Add("@language_id", SqlDbType.Int).Value = languageId;
                    command.Parameters.Add("@audit_user_id", SqlDbType.Int).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;
                    command.Parameters.Add("@autogenerate_dist_batch_id", SqlDbType.Bit).Value = autogenerateDistBatch;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();
                    
                    resultCode = int.Parse(command.Parameters["@ResultCode"].Value.ToString());

                    result = GetDistBatch(distBatchId, languageId, auditUserId, auditWorkstation);

                    return (SystemResponseCode)resultCode;
                }
            }
        }


        internal SystemResponseCode DistBatchCancel(long distBatchId, int? distBatchStatusesId, int distbatchtypeid, int? cardissuemethod, string notes, int languageId, long auditUserId, string auditWorkstation, out DistBatchResult result)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<DistBatchResult> results = context.usp_distbatch_cancel(distBatchId, distBatchStatusesId, distbatchtypeid, cardissuemethod, notes, languageId, auditUserId, notes, ResultCode);


                result = results.First();
                return (SystemResponseCode)int.Parse(ResultCode.Value.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="distBatchId"></param>
        /// <param name="notes"></param>
        /// <param name="rejectedCards"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="resultCode"></param>
        /// <returns></returns>
        internal DistBatchResult RejectProductionBatch(long distBatchId, string notes, List<RejectCardInfo> rejectedCards, int languageId, long auditUserId, string auditWorkstation, out DBResponseMessage resultCode)
        {
            DistBatchResult result = new DistBatchResult();
            resultCode = DBResponseMessage.FAILURE;

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    DataTable dt_CardList = new DataTable();
                    dt_CardList.Columns.Add("key", typeof(long));
                    dt_CardList.Columns.Add("value", typeof(string));
                    DataRow workRow;


                    foreach (var item in rejectedCards)
                    {
                        workRow = dt_CardList.NewRow();
                        workRow["key"] = item.CardId;
                        workRow["value"] = item.Comments;
                        dt_CardList.Rows.Add(workRow);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_dist_batch_reject_production]";

                    command.Parameters.Add("@dist_batch_id", SqlDbType.BigInt).Value = distBatchId;
                    command.Parameters.Add("@status_notes", SqlDbType.VarChar).Value = notes;
                    command.Parameters.AddWithValue("@reject_card_list", dt_CardList);
                    command.Parameters.Add("@language_id", SqlDbType.Int).Value = languageId;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {

                            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

                            var properties = result.GetType().GetProperties(flags);

                            foreach (var property in properties)
                            {
                                var col = dataReader[property.Name];

                                if (col != DBNull.Value)
                                    property.SetValue(result, Common.Utilities.UtilityClass.ChangeType(col, property.PropertyType), null);
                            }

                            resultCode = (DBResponseMessage)Convert.ToInt32(command.Parameters["@ResultCode"].Value);
                        }
                    }
                }
            }

            return result;
        }

        #endregion
    }
}