using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.DAL;


namespace Veneka.Indigo.Integration
{
    /// <summary>
    /// This class will build up export batches
    /// </summary>
    public class ExportBatchGenerator
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ExportBatchGenerator));
        private readonly string _connectionString = String.Empty;
        private readonly IExportBatchDAL _exportBatchDAL;

        public ExportBatchGenerator(IExportBatchDAL exportBatchDAL)
        {
            _exportBatchDAL = exportBatchDAL;
        }        

        public int Generate(int? issuerId, int languageId, long auditUserId, string auditWorkstation, out List<int> exportBatchIds)
        {                        
            var result = _exportBatchDAL.GenerateBatches(issuerId, auditUserId, auditWorkstation);
            exportBatchIds = result.ExportBatchIds.ToList();

            return result.ResultId;
        }
    }
}
