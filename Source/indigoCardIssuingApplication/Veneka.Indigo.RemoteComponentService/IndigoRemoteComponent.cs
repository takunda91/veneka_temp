using Common.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;


namespace Veneka.Indigo.RemoteComponentClient
{
    public partial class IndigoRemoteComponent : ServiceBase
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(IndigoRemoteComponent));

        public readonly static string EventSourceName = "Indigo Remote Component";
        public readonly static string EventLogName = "Indigo Remote Component Log";
        private System.Timers.Timer m_cardUpdateTimer;
        private bool m_cardUpdateTimerTaskSuccess;

        public IndigoRemoteComponent()
        {
            InitializeComponent();

            if (!System.Diagnostics.EventLog.SourceExists(EventSourceName))            
                System.Diagnostics.EventLog.CreateEventSource(EventSourceName, EventLogName);            

            InfoEventLog.Source = EventSourceName;
            InfoEventLog.Log = EventLogName;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _log.Info("Starting Indigo Remote Component");
                // Create and start a timer.                
                m_cardUpdateTimer = new System.Timers.Timer();
                //m_cardUpdateTimer.Interval = Configuration.ConfigReader.CardUpdateTimerMili;
                m_cardUpdateTimer.Elapsed += m_cardUpdateTimer_Elapsed;
                m_cardUpdateTimer.AutoReset = false;  // makes it fire only once
                m_cardUpdateTimer.Start(); // Start

                m_cardUpdateTimerTaskSuccess = false;
            }
            catch (Exception ex)
            {
                InfoEventLog.WriteEntry(ex.Message + Environment.NewLine + ex.StackTrace, EventLogEntryType.Error);
            }

            InfoEventLog.WriteEntry("Indigo Remote Component Started", EventLogEntryType.Information);
        }

        void m_cardUpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                _log.Trace(t => t("Timer elapsed"));
                //using (CardUpdateComponent cardUpdate = new CardUpdateComponent())
                //{
                //    cardUpdate.ProcessCardUpdates();
                //}

                m_cardUpdateTimerTaskSuccess = true;
            }
            catch (Exception ex)
            {
                m_cardUpdateTimerTaskSuccess = false;
                _log.Error(ex);
                //InfoEventLog.WriteEntry(ex.Message + Environment.NewLine + ex.StackTrace, EventLogEntryType.Error);
            }
            finally
            {
                if (m_cardUpdateTimerTaskSuccess)
                {
                    m_cardUpdateTimer.Start();
                }
            }
        }

        protected override void OnStop()
        {
            try
            {
                _log.Info("Stopping Indigo Remote Component.");
                // Service stopped. Also stop the timer.
                m_cardUpdateTimer.Stop();
                m_cardUpdateTimer.Dispose();
                m_cardUpdateTimer = null;
            }
            catch (Exception ex)
            {
                InfoEventLog.WriteEntry(ex.Message + Environment.NewLine + ex.StackTrace, EventLogEntryType.Error);
            }

            InfoEventLog.WriteEntry("Indigo Remote Component Stopped", EventLogEntryType.Information);
        }
    }
}
