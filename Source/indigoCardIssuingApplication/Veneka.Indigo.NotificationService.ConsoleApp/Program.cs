using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Veneka.Indigo.NotificationService.BLL;

namespace Veneka.Indigo.NotificationService.ConsoleApp
{
    class Program
    {
        private static readonly ILog _log = LogManager.GetLogger("NotificationInterfacelogging");
        private static string _baseDir;
        private static string _connectionString;
        private static NotificationsController _notificationController;
        static void Main(string[] args)
        {
            try
            {

                _log.Info("Starting Notification Service.");
                Console.WriteLine("Starting Notification Service.");

                _baseDir = ConfigurationManager.AppSettings["BaseConfigDir"].ToString();

                if (String.IsNullOrWhiteSpace(_baseDir))
                    throw new ArgumentNullException("BaseConfigDir", "Value cannot be null or empty");

                //Get connection string
                using (var reader = TextReader.Synchronized(new StreamReader(Path.Combine(_baseDir, @"config\Database.config"))))
                    _connectionString = reader.ReadLine();

                //Start if we have both
                if (String.IsNullOrWhiteSpace(_connectionString))
                    throw new ArgumentNullException("ConnectionString", "Value cannot be null or empty, please check Indigo Database.config");

                _notificationController = new NotificationsController(_baseDir, _connectionString);
                _notificationController.Start();
    //                .ContinueWith(t =>
    //            {
    //                _log.Fatal(t.Exception);
    //                OnStop();
    //            },
    //TaskContinuationOptions.OnlyOnFaulted);

                _log.Info("Starting Notification Service Done.");
                Console.WriteLine("Starting Notification Service Done");

            }
            catch (Exception ex)
            {
                _log.Fatal(ex);

                OnStop();
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
        }
        protected static void OnStop()
        {
            try
            {
                _log.Info("Stopping Notification Service.");
                Console.WriteLine("Stopping Notification Service.");
                _notificationController.Stop();
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            finally
            {
                _notificationController.Dispose();
                _log.Info("Stopping Notification Service Done.");
                Console.WriteLine("Stopping Notification Service Done.");
            }
        }

    }

}

