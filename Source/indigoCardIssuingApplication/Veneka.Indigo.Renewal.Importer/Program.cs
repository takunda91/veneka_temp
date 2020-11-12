using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Renewal.Importer
{
    class Program
    {
        static void Main(string[] args)
        {
            ILog logger = LogManager.GetLogger("Veneka.Indigo.Renewal.Importer");
            logger.Info("Start Veneka.Indigo.Renewal.Importer");
            try
            {
                Incoming.RenewalExtractor processor = new Incoming.RenewalExtractor();
                logger.Info("Veneka.Indigo.Renewal: Start Import");
                processor.ExtractRenewalFiles();
                logger.Info("Veneka.Indigo.Renewal: Import Complete");

                logger.Debug("File Import Completed");
                Console.ReadLine();
            }
            catch (Exception exp)
            {
                logger.Error("Fatal error in Renewal Card import", exp);
                Console.ReadLine();
            }
        }
    }
}
