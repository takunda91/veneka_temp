using Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Notifications.smsservice;
using Veneka.Indigo.Notifications.WebServiceConfigs;
using static Veneka.Indigo.Notifications.WebServiceConfigs.WebServiceBindings;

namespace Veneka.Indigo.Notifications.NotificationChannels
{
    public class SMSController
    {
        public string destination { get; set; }
        public string source { get; set; }
        public string message_body { get; set; }
        public string sms_url { get; set; }
        private static readonly ILog log = LogManager.GetLogger(typeof(SMSController));
     

       

        public SMSController(string destination, string source, string message_body)
        {

            this.destination = destination;
            this.source = source;
            this.message_body = message_body;       
      
        }



        public bool sendSMS(out int responseCode)
        {
            responseCode = 0;
            try
            {
                Protocol protocol = Protocol.HTTP;
                string address = "10.242.8.23";
                int port = 1001;
                string path = "Services/AccountEnquiry.asmx";
                int? timeoutMilliSeconds = null;
                Authentication authentication = Authentication.NONE;
                string username = "Takunda";
                string password = "19381038129038";
                

                NotificationsEndPointCreator new_sms = new NotificationsEndPointCreator(protocol,address,port,path,timeoutMilliSeconds,authentication,username,password);
                var sendsms = new_sms.sendSMS(this.destination, this.message_body, this.source, out responseCode);
                if(sendsms)
                return true;

                return false;
            }
            catch(Exception ex)
            {
                log.Error(ex);
                return false;
            }
           
        }
    }
}
