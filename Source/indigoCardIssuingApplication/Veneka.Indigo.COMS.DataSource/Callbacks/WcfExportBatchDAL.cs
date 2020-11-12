using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.COMS.Core;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.COMS.DataSource.Callbacks
{
    public class WcfExportBatchDAL: IExportBatchDAL
    {
        private readonly IComsCallback _proxy;
        public WcfExportBatchDAL(IComsCallback proxy)
        {
            _proxy = proxy;
        }
        public List<CardObject> FetchCardObjectsForExportBatch(long exportBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _proxy.FetchCardObjectsForExportBatch(exportBatchId, languageId, auditUserId, auditWorkstation);
        }

        public Dictionary<long, string> FindExportBatches(int issuerId, int? productId, int exportBatchStatusesId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _proxy.FindExportBatches(issuerId, productId, exportBatchStatusesId, languageId, auditUserId, auditWorkstation);
        }

        public ExportBatchGeneration GenerateBatches(int? issuerId, long auditUserId, string auditWorkStation)
        {
            return _proxy.GenerateBatches(issuerId, auditUserId, auditWorkStation);
        }

        public int StatusChangeExported(long exportBatchId, int languageId, long auditUserId, string auditWorkStation)
        {
            return _proxy.StatusChangeExported(exportBatchId, languageId, auditUserId, auditWorkStation);
        }
    }
}
