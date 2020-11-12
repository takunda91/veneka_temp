using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Renewal.dal;
using Veneka.Indigo.Renewal.Entities;

namespace Veneka.Indigo.Renewal.CMS.Receive
{
    public class RenewalReceiver
    {
        IRenewalDataAccess _renewalDataAccess = new RenewalDataAccess();
        IRenewalOperations _renewalOperations = new RenewalOperations();

        public void Execute()
        {
            ILog logger = LogManager.GetLogger("Veneka.Indigo.Renewal.CMS.Send.RenewalReceiver");

            var baseDirectory = ConfigurationManager.AppSettings["baseDir"];
            if (baseDirectory != null)
            {
                string sourceDirectory = Path.Combine(baseDirectory.ToString(), "renewals", "fromcms");
                logger.Info($"Execute {sourceDirectory}");

                Integration.Receive.IReceiveResponse processor = new Integration.Receive.TMB.ReceiveResponse();
                var batches = _renewalDataAccess.RetrieveBatches(RenewalBatchStatusType.Exported, 1, "SYSTEM");
                if (batches != null && batches.Count > 0)
                {
                    logger.Info($"Total of {batches.Count} in status Exported batches found for import.");
                    List<RenewalDetailListModel> expectedEntries = new List<RenewalDetailListModel>();
                    foreach (var item in batches)
                    {
                        expectedEntries.AddRange(_renewalDataAccess.RetrieveBatchDetails(item.RenewalBatchId, false, 1, 1, "SYSTEM").ToList());
                    }
                    var returningResidents = processor.ExtractFile(sourceDirectory);
                    List<RenewalResponseDetail> matchList = new List<RenewalResponseDetail>();
                    foreach (var expected in expectedEntries)
                    {
                        var match = returningResidents.Where(p => p.CardNumber == expected.CardNumber).FirstOrDefault();
                        if (match != null)
                        {
                            matchList.Add(match);
                        }
                    }

                    if (matchList.Count != returningResidents.Count)
                    {
                        throw new Exception("Count mismatch between entries received VS entries uploaded to CMS.  Check files in folder are correct.");
                    }
                    else
                    {
                        //ensure they are all exact in the matching
                        var query = (from itm in matchList
                                     join ret in returningResidents on itm.CardNumber equals ret.CardNumber
                                     select itm).ToList();
                        if (query.Count == returningResidents.Count)
                        {
                            foreach (var newCard in query)
                            {
                                var legOne = expectedEntries.Where(p => p.CardNumber == newCard.CardNumber).FirstOrDefault();
                                _renewalOperations.CreateRenewalCard(legOne.RenewalDetailId, newCard, 1, "SYSTEM");
                            }
                        }
                        else
                        {
                            throw new Exception("Data Mismatch between entries received VS entries uploaded to CMS.  Check files in folder are correct.");
                        }
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
