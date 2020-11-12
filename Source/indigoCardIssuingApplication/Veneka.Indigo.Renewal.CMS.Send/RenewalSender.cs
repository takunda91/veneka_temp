using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Renewal.dal;
using Veneka.Indigo.Renewal.Entities;

namespace Veneka.Indigo.Renewal.CMS.Send
{
    public class RenewalSender
    {
        IRenewalDataAccess _renewalDataAccess = new RenewalDataAccess();

        public void Execute()
        {
            ILog logger = LogManager.GetLogger("Veneka.Indigo.Renewal.CMS.Send.RenewalSender");

            var baseDirectory = ConfigurationManager.AppSettings["baseDir"];
            
            if (baseDirectory != null)
            {
                string exportDirectory = Path.Combine(baseDirectory.ToString(), "renewals", "tocms");
                logger.Info($"Execute {exportDirectory}");

                Integration.Send.IRenewalRequest processor = new Integration.Send.TMB.RenewalRequest();
                var batches = _renewalDataAccess.RetrieveBatches(RenewalBatchStatusType.Approved, 1, "SYSTEM");
                if (batches != null && batches.Count > 0)
                {
                    logger.Info($"Total of {batches.Count} in status Approved batches found for export.");
                    foreach (var item in batches)
                    {
                        var entries = _renewalDataAccess.RetrieveBatchDetails(item.RenewalBatchId, false, 1, 1, "SYSTEM").ToList();
                        if (entries != null && entries.Count > 0)
                        {
                            string issuerCode = entries.FirstOrDefault().IssuerCode;
                            string productCode = entries.FirstOrDefault().ProductCode;
                            string issuerName = entries.FirstOrDefault().IssuerName;
                            string bin = entries.FirstOrDefault().CardNumber.Substring(0, 6);

                            int nextFileNumber = _renewalDataAccess.NextSequenceNumber("RENEW" + issuerCode + productCode, ResetPeriod.DAILY);
                            string newFileName = $"{issuerCode}_{bin}_Reissue_Cards_Renewal_{nextFileNumber.ToString("0000")}_{DateTime.Today.ToString("yyyyMMdd")}.xml";

                            var files = processor.BuildFile(entries, exportDirectory, newFileName);
                            foreach (var savedFile in files)
                            {
                                logger.Debug($"File generated : {savedFile}");
                            }
                        }
                        _renewalDataAccess.ChangeBatchStatus(item.RenewalBatchId, RenewalBatchStatusType.Exported, 1, "SYSTEM");
                    }
                }
                else
                {
                    logger.Info("No Approved batches found for export.");
                }

                logger.Info("Veneka.Indigo.Renewal.CMS.Send.RenewalSender.Execute Completed");
            }
            else
            {
                logger.Error("Veneka.Indigo.Renewal: Base Directory not configured");
            }
        }
    }
}
