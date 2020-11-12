using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.External;
using Veneka.Indigo.NotificationService.DAL;
using System.Threading.Tasks;

namespace Veneka.Indigo.NotificationService.BLL
{
   
    public class NotificationsController : IDisposable
    {
       
        #region Constants
        private const int SERVICE_USER_ID = -2;
        private const string SERVICE_WORKSTATION = "NOTIFICATION_SERVICE";
        public enum MessagePumpState
        {
            Running,
            Faulted
        }
        private MessagePumpState _state;
        #endregion

        #region Fields
        private static readonly ILog _log = LogManager.GetLogger(typeof(NotificationsController));
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private string _baseDir;
        private string _connectionString;
        private Integration.IntegrationController _integration;
        private NotificationDAL _notificationDal;
        #endregion

        #region Constructors
        public NotificationsController(string baseDir, string connectionString)
        {
            _baseDir = baseDir;
            _connectionString = connectionString;
            _integration = new Integration.IntegrationController(baseDir, connectionString, SERVICE_USER_ID, SERVICE_WORKSTATION);
            _notificationDal = new NotificationDAL(connectionString);
        }
        #endregion

        #region Public Methods


    
        public  void Start()
        {
            _log.Trace(t => t("Starting Notifications Controller"));

            bool branchNotifications = bool.Parse(ConfigurationManager.AppSettings["EnableBranchNotifications"].ToString());
            bool batchNotifications = bool.Parse(ConfigurationManager.AppSettings["EnableBatchNotifications"].ToString());

            _log.Trace(t => t("EnableBranchNotifications: " + branchNotifications));
            _log.Trace(t => t("EnableBatchNotifications: " + batchNotifications));

            if (branchNotifications)
            {
                //Get configs
                int branchCycle = int.Parse(ConfigurationManager.AppSettings["BranchTime"].ToString());
                int branchMaxSend = int.Parse(ConfigurationManager.AppSettings["BranchMaxSend"].ToString());

                _log.Trace(t => t("BranchTime: " + branchCycle));
                _log.Trace(t => t("BranchMaxSend: " + branchMaxSend));
                this._state = MessagePumpState.Running;
                

                //start threads to handle notifications
                try
                {
                    Task.Factory.StartNew(() => BranchChecker(branchCycle, _cts.Token), _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                    //await task;
                    //Task.Factory.StartNew(()=>BranchChecker(branchCycle, _cts.Token));
                    //Task.WaitAll(t1);
                }
                catch
                {
                    this._state = MessagePumpState.Faulted;
                    throw;
                }
            }
            if (batchNotifications)
            {
                //Get configs
                int batchCycle = int.Parse(ConfigurationManager.AppSettings["BatchTime"].ToString());
                int batchMaxSend = int.Parse(ConfigurationManager.AppSettings["BatchMaxSend"].ToString());

                _log.Trace(t => t("BatchTime: " + batchCycle));
                _log.Trace(t => t("BatchMaxSend: " + batchMaxSend));
                this._state = MessagePumpState.Running;
                //start threads to handle notifications
                //Task.Factory.StartNew(() => BatchChecker(batchCycle, _cts.Token));
                try
                {
                    Task.Factory.StartNew(() => BatchChecker(batchCycle, _cts.Token), _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                    //await task1;
                    //Task.Factory.StartNew(()=>BranchChecker(branchCycle, _cts.Token));
                    //Task.WaitAll(t1);
                }
                catch
                {
                    this._state = MessagePumpState.Faulted;
                    throw;
                }
            }

            _log.Trace(t => t("Starting Notifications Controller Done"));

        }

        public void Stop()
        {
            _log.Trace(t => t("Stopping Notifications Controller"));

            _cts.Cancel();
            Thread.Sleep(10000);

            _log.Trace(t => t("Stopping Notifications Controller Done"));
        }
        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();

            _baseDir = String.Empty;
            _connectionString = String.Empty;
            _integration.Dispose();
        }

        #endregion

        #region Private Methods
        private void BranchChecker(int cycle, CancellationToken ct)
        {
            _log.Trace(t => t("Entering Branch Outbox Checker"));
            Console.WriteLine("Entering Branch Outbox Checker");

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    //Get Branch Notifications outbox
                    var notifications = _notificationDal.GetBranchOutbox(SERVICE_USER_ID, SERVICE_WORKSTATION);

                    _log.Trace(t => t("Processing notification count: " + notifications.Count));

                    Console.WriteLine("Processing notification count: " + notifications.Count);

                    //Send for each issuer
                    foreach (var issuerId in notifications.Where(i => i.Channel == 1).Select(s => s.IssuerId).Distinct().ToList())// channel 1 is SMS
                    {
                        IConfig config;

                        string response;

                        var issuerNotifications = notifications.Where(w => w.IssuerId == issuerId).ToList();

                        var issuerIntegration = _integration.NotificationSystem(issuerId, 1, out config);

                        //TODO EMAIL vs SMS
                        issuerIntegration.SMS(ref issuerNotifications, config, 0, SERVICE_USER_ID, SERVICE_WORKSTATION, out response);

                        //Warehouse them
                        _notificationDal.LogBranchNotifications(issuerNotifications, SERVICE_USER_ID, SERVICE_WORKSTATION);
                    }

                    notifications = null;
                    sw.Stop();
                    //Console.WriteLine("Time: " + sw.Elapsed.TotalMilliseconds);


                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                    Console.WriteLine(ex.Message);
                }

                //Sleep till next time or if we're canceled exit
                if (!ct.IsCancellationRequested)
                    ct.WaitHandle.WaitOne(cycle);
            }

            _log.Trace(t => t("Exiting Branch Outbox Checker"));

        }

        private void BatchChecker(int cycle, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                //Do somework
                _log.Info("Batch Do Something");
                Console.WriteLine("Entering Batch Outbox Checker");
                try
                {
                    //Get Batch Notifications
                    var notifications = _notificationDal.GetBatchOutbox(SERVICE_USER_ID, SERVICE_WORKSTATION);

                    foreach (var issuerId in notifications.Where(i => i.Channel == 0).Select(s => s.IssuerId).Distinct().ToList())// channel 0 is Email
                    {
                        IConfig config;
                        string response;

                        var issuerNotifications = notifications.Where(w => w.IssuerId == issuerId).ToList();

                        var issuerIntegration = _integration.NotificationSystem(issuerId, 0, out config);

                        //TODO EMAIL vs SMS
                        issuerIntegration.Email(ref issuerNotifications, config, 0, SERVICE_USER_ID, SERVICE_WORKSTATION, out response);

                        //Warehouse them
                        _notificationDal.LogBatchNotifications(issuerNotifications, SERVICE_USER_ID, SERVICE_WORKSTATION);
                    }



                    notifications = null;

                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                    Console.WriteLine(ex.Message);
                }
                //Sleep till next time or if we're canceled
                if (!ct.IsCancellationRequested)
                    ct.WaitHandle.WaitOne(cycle);
            }
            _log.Info("Exiting Batch Checker");
            Console.WriteLine("Exiting Batch Checker");
        }
        #endregion        
    }
}
