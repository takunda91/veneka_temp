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
    public class ThreedBatchManagementDAL
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
        internal List<ThreedBatchResult> GetThreedBatches(string ThreedBatchReference, int issuerId, ThreedBatchStatus? ThreedBatchStatus, DateTime? startDate, DateTime? endDate,
                                                        int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkStation)
        {
            List<ThreedBatchResult> rtnValue = new List<ThreedBatchResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                int? status = null;
                if(ThreedBatchStatus !=null)
                {
                    status = (int)ThreedBatchStatus;
                }
                ObjectResult<ThreedBatchResult> results = context.usp_get_threedsecure_batch_list(issuerId, startDate, endDate, status, ThreedBatchReference, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkStation);

                foreach (ThreedBatchResult result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }
        internal ThreedBatchResult GetThreedBatch(long ThreedBatchId, int languageId, long auditUserId, string auditWorkStation)
        {
            ThreedBatchResult rtnValue = new ThreedBatchResult();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<ThreedBatchResult> results = context.usp_get_threedsecure_batch(ThreedBatchId, languageId, auditUserId, auditWorkStation);

                foreach (ThreedBatchResult result in results)
                {
                    return result;
                }
            }

            return null;
        }

        internal SystemResponseCode RecreateThreedBatch(long threedBatchId,int Threed_batch_statues_id, string notes, int languageId, long auditUserId, string auditWorkStation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_update_threed_batch_status(threedBatchId, Threed_batch_statues_id, notes, languageId, auditUserId, auditWorkStation, ResultCode);                
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());
            return (SystemResponseCode)resultCode;
        }
        #endregion
    }
}


