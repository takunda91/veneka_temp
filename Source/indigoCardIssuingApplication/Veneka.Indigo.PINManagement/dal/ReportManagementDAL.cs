using Common.Logging;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Common.Models;

namespace Veneka.Indigo.PINManagement.dal
{
    class ReportManagementDAL
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportManagementDAL));

        private readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;

        /// <summary>
        /// PDF Reports: Get heading names from database
        /// </summary>
        /// <param name="reportid"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        internal List<report_fields_Result> GetReportFields(int reportid, int languageId)
        {
            List<report_fields_Result> rtnValue = new List<report_fields_Result>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<report_fields_Result> results = context.usp_get_report_fields(reportid, languageId);

                foreach (report_fields_Result result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }
    }
}
