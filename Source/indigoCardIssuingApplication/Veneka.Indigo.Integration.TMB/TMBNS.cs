using System;
using System.Collections.Generic;
using Common.Logging;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.Integration.WebServices;
using Veneka.Indigo.Integration.Email;
using System.IO;
using Veneka.Indigo.Integration.Common;

namespace Veneka.Indigo.Integration.TMB
{
    [IntegrationExport("TMBNS", "D163EDB8-3302-4E37-8DA7-2E427AD22073", typeof(INotificationSystem))]
   public class TMBNS : INotificationSystem
    {
        private static readonly ILog _nsLog = LogManager.GetLogger(General.CPS_LOGGER);
        
        public DirectoryInfo IntegrationFolder
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public IDataSource DataSource
        {
            get;
            set;

        }
        public string SQLConnectionString { get; set; }

        public bool Email(ref List<Notification> notifications, IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage)
        {
            responseMessage = String.Empty;
            if (config is Config.SMTPConfig)
            {
                var cbsParms = (Config.SMTPConfig)config;
                foreach (var item in notifications)
                {
                    item.IsSetn = EmailSender.Send(cbsParms.SMTPServer,cbsParms.Port,item.FromAddress,item.Address,item.Subject,item.Text, cbsParms.SMTPUsername, cbsParms.SMTPPassword,out responseMessage);
                }
            }
            

           
            return true;
        }

        public bool SMS(ref List<Notification> notifications, IConfig config, int languageId, long auditUserId, string auditWorkStation, out string responseMessage)
        {
          
            if (config is Config.WebServiceConfig)
            {
                var cbsParms = (Config.WebServiceConfig)config;
                WebServices.Protocol protocol = cbsParms.Protocol == Config.Protocol.HTTP ?WebServices.Protocol.HTTP : WebServices.Protocol.HTTPS;
                Authentication auth = cbsParms.AuthType == Config.AuthType.None ? Authentication.NONE : Authentication.BASIC;
                AccountDetails accDetails = new AccountDetails();
                TagSMS.TagSMSService smsService = new TagSMS.TagSMSService(this.SQLConnectionString);
               
                foreach (var item in notifications)
                {
                    item.IsSetn = smsService.SendSMS(cbsParms,item.Text, item.Address, out responseMessage);
                }
            }
            responseMessage = String.Empty;
            return true;
        }
    }
}
