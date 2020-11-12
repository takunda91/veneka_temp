using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Models;
using Common.Logging;
using Veneka.Indigo.PINManagement;
using Veneka.Indigo.PINManagement.dal;

namespace Veneka.Indigo.PINManagement
{
    public class ReportManagementService
    {
        private readonly ReportManagementDAL _reportDAL = new ReportManagementDAL();
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportManagementService));

        /// <summary>
        /// PDF Report Header
        /// </summary>
        /// <param name="reportid"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        public List<report_fields_Result> GetReportFields(int reportid, int languageId)
        {
            List<report_fields_Result> rtnValue = new List<report_fields_Result>();
            try
            {
                rtnValue = _reportDAL.GetReportFields(reportid, languageId);
            }
            catch (Exception ex)
            {
                log.Error(ex);


            }
            return rtnValue;
        }
    }
}
