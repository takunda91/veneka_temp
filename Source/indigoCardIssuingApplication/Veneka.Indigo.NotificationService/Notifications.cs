using Common.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Veneka.Indigo.NotificationService.BLL;
using System.Threading.Tasks;

namespace Veneka.Indigo.NotificationService
{
    public partial class Notifications : ServiceBase
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Notifications));
        private string _baseDir;
        private string _connectionString;
        private NotificationsController _notificationController;

        public Notifications()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _log.Info("Starting Notification Service.");

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
//                    .ContinueWith(t =>
//                {
//                    _log.Fatal(t.Exception);
//                    OnStop();
//                },
//TaskContinuationOptions.OnlyOnFaulted); 

                _log.Info("Starting Notification Service Done.");
            }
            catch(Exception ex)
            {
                _log.Fatal(ex);
                OnStop();
            }
        }

        protected override void OnStop()
        {
            try
            {
                _log.Info("Stopping Notification Service.");

                _notificationController.Stop();
            }
            catch(Exception ex)
            {
                _log.Error(ex);
            }
            finally
            {
                _notificationController.Dispose();
                _log.Info("Stopping Notification Service Done.");
            }
        }
    }
}
