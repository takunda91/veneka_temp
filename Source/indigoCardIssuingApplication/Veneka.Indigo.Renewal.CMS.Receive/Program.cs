using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Renewal.CMS.Receive
{
    class Program
    {
        static void Main(string[] args)
        {
            ILog logger = LogManager.GetLogger("Veneka.Indigo.Renewal.CMS.Receive");

            try
            {
                RenewalReceiver sender = new RenewalReceiver();
                logger.Info("START Veneka.Indigo.Renewal.CMS.Receive");
                sender.Execute();
                logger.Info("END Veneka.Indigo.Renewal.CMS.Receive");

                Console.ReadLine();
            }
            catch (Exception exp)
            {
                logger.Error("Fatal error in Veneka.Indigo.Renewal.CMS.Receive", exp);
                Console.ReadLine();
            }
        }
    }
}
