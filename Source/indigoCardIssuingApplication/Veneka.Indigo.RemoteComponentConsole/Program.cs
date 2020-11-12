using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.RemoteComponentClient;

namespace Veneka.Indigo.RemoteComponentConsole
{
    class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            try
            {
                _log.Info("Starting remote CMS Update.");
                CardUpdateComponent cuc = new CardUpdateComponent();

                cuc.ProcessCardUpdates();

                _log.Info("Completed remote CMS Update");
            }
            catch(Exception ex)
            {
                _log.Error(ex);
            }
            finally
            {
                if (args == null || args.Length <= 0)
                    Console.ReadKey();
            }
        }
    }
}
