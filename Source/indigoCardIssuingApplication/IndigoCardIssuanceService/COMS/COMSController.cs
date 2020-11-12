using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Veneka.Indigo.Common;
using Veneka.Indigo.COMS.Core;
using Veneka.Indigo.COMS.DataSource;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.External;

namespace IndigoCardIssuanceService.COMS
{
    public class COMSController
    {
        private static IComsCore _comsCore;
        private static IComPrepaidSystem _prepaidSystem;

        private static object _lockObj = new object();
        private static readonly ILog log = LogManager.GetLogger(typeof(COMSController));
        public COMSController()
        {

        }

        public static IComsCore ComsCore
        {
            get
            {
                return GetComs();
            }
        }

        private static IComsCore GetComs()
        {
            if (_comsCore == null)
            {
                lock (_lockObj)
                {
                    if (_comsCore != null)
                        return _comsCore;

                    int comsOption = GetComsType();

                    if (comsOption == 0)
                    {
                        _comsCore = new ComsCore(new LocalDataSource());
                    }
                    else if (comsOption == 1)
                    {
                        _comsCore = new WcfComsClient(new Uri(ConfigurationManager.AppSettings["Uri"].ToString()));
                        System.Timers.Timer keepAliveTimer = new System.Timers.Timer(30000);
                        keepAliveTimer.AutoReset = false;
                        keepAliveTimer.Elapsed += new System.Timers.ElapsedEventHandler(keepAliveTimer_Elapsed);
                        keepAliveTimer.Start();
                    }
                }
            }

            return _comsCore;
        }

        public static IComPrepaidSystem PrepaidSystem
        {
            get
            {
                return GetPrepaid();
            }
        }

        private static IComPrepaidSystem GetPrepaid()
        {
            if (_prepaidSystem == null)
            {
                lock (_lockObj)
                {
                    if (_prepaidSystem != null)
                        return _prepaidSystem;


                    _prepaidSystem = new ComPrepaidSystem(new LocalDataSource());
                }
            }

            return _prepaidSystem;
        }

        //public delegate void ConnectionStatusChangeHandler(Object sender, ConnectionStatus connectionStatus);
        //public static event ConnectionStatusChangeHandler OnConnectionStatusChange;
        static void keepAliveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                //Check if the server is still available
                var response = _comsCore.CheckConnection();
                if (response.Value)
                {
                    log.Debug("ComsCore Server is Running");
                    // OnConnectionStatusChange(this, ConnectionStatus.ConnectedToService);
                }
            }
            catch (Exception ex)
            {
                log.Debug("ComsCore Server is Unavailable");
                log.Debug(ex);
                //if (OnConnectionStatusChange != null)
                //{
                //    OnConnectionStatusChange(this, ConnectionStatus.ServerUnavailable);
                //}
            }
            finally
            {
                ((System.Timers.Timer)sender).Start();
            }
        }
        private static int GetComsType()
        {
            var typeStr = ConfigurationManager.AppSettings["ComsType"].ToString();

            int comsType;

            if (int.TryParse(typeStr, out comsType))
            {
                return comsType;
            }

            throw new ArgumentException("Type incorrect: " + typeStr, "ComsType");

        }
    }
}