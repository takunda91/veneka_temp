using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Data.SqlClient;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.CardManagement.objects;
using Common.Logging;

namespace Veneka.Indigo.CardManagement.dal
{
    internal class LoadBatchManagementDAL
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LoadBatchManagementDAL));
        private readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;

        #region EXPOSED METHODS

        /// <summary>
        /// Return a list of load batches based on parameters.
        /// </summary>
        /// <param name="loadBatchReference"></param>
        /// <param name="loadBatchStatus"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        internal List<LoadBatchResult> GetLoadBatches(string loadBatchReference, int issuerId, LoadBatchStatus? loadBatchStatus, DateTime? startDate, DateTime? endDate,
                                                        int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            List<LoadBatchResult> rtnValue = new List<LoadBatchResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<LoadBatchResult> results = context.usp_get_load_batches(null, loadBatchReference, issuerId, (int?)loadBatchStatus, startDate, endDate, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkStation);

                foreach (LoadBatchResult result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }

        /// <summary>
        /// Return a single load batch.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <returns></returns>
        internal LoadBatchResult GetLoadBatch(long loadBatchId, int languageId, long auditUserId, string auditWorkStation)
        {
            LoadBatchResult rtnValue = new LoadBatchResult();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<LoadBatchResult> results = context.usp_get_load_batch(loadBatchId, languageId, auditUserId, auditWorkStation);

                foreach (LoadBatchResult result in results)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Return a list of cards linked to the specified load batch.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <returns></returns>
        internal List<LoadCardResult> GetLoadBatchCards(long loadBatchId, long auditUserId, string auditWorkStation)
        {
            List<LoadCardResult> rtnValue = new List<LoadCardResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<LoadCardResult> results = context.usp_get_load_batch_cards(loadBatchId, auditUserId, auditWorkStation);

                foreach (var result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }

        /// <summary>
        /// Fetch a load batch status history.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        internal List<LoadBatchHistoryResult> GetLoadBatchHistory(long loadBatchId,int languageId, long auditUserId, string auditWorkStation)
        {
            List<LoadBatchHistoryResult> rtnValue = new List<LoadBatchHistoryResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<LoadBatchHistoryResult> results = context.usp_get_load_batch_history(loadBatchId,languageId, auditUserId, auditWorkStation);

                foreach (var result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }

        /// <summary>
        /// Approve the load batch.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <param name="notes"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        internal SystemResponseCode ApproveLoadBatch(long loadBatchId, string notes, long auditUserId, string auditWorkStation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_load_batch_approve(loadBatchId, notes, auditUserId, auditWorkStation, ResultCode);
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());
            return (SystemResponseCode)resultCode;
        }

        /// <summary>
        /// Reject the load batch.
        /// </summary>
        /// <param name="loadBatchId"></param>
        /// <param name="notes"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        /// <returns></returns>
        internal SystemResponseCode RejectLoadBatch(long loadBatchId, string notes, long auditUserId, string auditWorkStation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_load_batch_reject(loadBatchId, notes, auditUserId, auditWorkStation, ResultCode);
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());
            return (SystemResponseCode)resultCode;
        }

        /// <summary>
        /// Get list if file_load
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal List<FileLoadResult> GetFileLoadList(DateTimeOffset dateFrom, DateTimeOffset dateTo, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var response = context.usp_get_file_load_list(dateFrom, dateTo, pageIndex, rowsPerPage,auditUserId,auditWorkstation);

                return response.ToList();
            }
        }

        /// <summary>
        /// Fetch all file histories
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal List<FileHistoryResult> GetFileHistorys(int? issuerId, DateTime dateFrom, DateTime dateTo, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var response = context.usp_get_file_historys(issuerId, dateFrom, dateTo, auditUserId, auditWorkstation);

                return response.ToList();
            }
        }

        /// <summary>
        /// Fetch specific file history
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal FileHistoryResult GetFileHistory(long fileId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var response = context.usp_get_file_history(fileId, auditUserId, auditWorkstation);

                return response.First();
            }
        }
        
        #endregion
    }
}