using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Renewal.dal;

namespace Veneka.Indigo.Renewal.CMS.Send
{
    class Program
    {
        static void Main(string[] args)
        {
            ILog logger = LogManager.GetLogger("Veneka.Indigo.Renewal.CMS.Send");

            try
            {
                RenewalSender sender = new RenewalSender();
                logger.Info("START Veneka.Indigo.Renewal.CMS.Sender");
                sender.Execute();
                logger.Info("END Veneka.Indigo.Renewal.CMS.Sender");

                Console.ReadLine();
            }
            catch (Exception exp)
            {
                logger.Error("Fatal error in Veneka.Indigo.Renewal.CMS.Sender", exp);
                Console.ReadLine();
            }
        }
    }
}
