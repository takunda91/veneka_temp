using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Notifications.smsservice;

namespace Veneka.Indigo.Notifications.WebServiceConfigs
{
    public class NotificationsEndPointCreator: WebServiceBindings
    {
        private const string LOGGER = "SMSWebservice";
        public Protocol protocol;
        public string address;
        public int port;
        public string path;
        public int? timeoutMilliSeconds;
        public Authentication authentication;
        public string username;
        public string password;
        protected readonly RIBMiddleWareAccountEnquirySoapClient service_one;
        private static readonly ILog log = LogManager.GetLogger(typeof(NotificationsEndPointCreator));
        public NotificationsEndPointCreator(Protocol protocol, string address, int port, string path, int? timeoutMilliSeconds,
                                Authentication authentication, string username, string password
                                 ) : base(protocol, address, port, path, timeoutMilliSeconds, authentication, username, password,
                                                         LOGGER)
        {
            this.protocol = protocol;
            this.address = address;
            this.port = port;
            this.path = path;
            this.timeoutMilliSeconds = timeoutMilliSeconds;
            this.authentication = authentication;
            this.username = username;
            this.password = password;

            this.service_one = new RIBMiddleWareAccountEnquirySoapClient(base.BuildBindings("RIBMiddleWare - Account EnquirySoap", protocol, timeoutMilliSeconds),
                                           base.BuildEndpointAddress(protocol, address, port, path));

            this.service_one.Endpoint.Behaviors.Add(new LogClientBehaviour(authentication == Authentication.BASIC ? true : false,
                                                                                username, password, true, LOGGER));

            _log.Trace(m => m("Creating Notifications Service."));
            IgnoreUntrustedSSL = true;
        }

        public bool sendSMS(string destination, string message_body, string source, out int responseCode)
        {
            responseCode = 99;
            try
            {

                var sendsms = this.service_one.sendSMS(destination, message_body, source);

                var ticket_id = sendsms.ticketId;
                var response_code = sendsms.response.Code;
                var response_message = sendsms.response.Message;
                log.InfoFormat("Response: ticket id = {0}, response_code = {1}, response_message = {2}.", ticket_id, response_code, response_message);
                if (int.TryParse(response_code, out responseCode))
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }

        }

    }
}
