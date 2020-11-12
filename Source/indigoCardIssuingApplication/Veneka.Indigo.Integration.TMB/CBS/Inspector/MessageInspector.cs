using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml.Linq;

namespace Veneka.Indigo.Integration.TMB.CBS.Inspector
{
   public class MessageInspector : IClientMessageInspector
    {
        #region Private Fields
        private static ILog _log = LogManager.GetLogger(Utils.General.MODULE_LOGGER);
        private readonly bool _useBasicAuth;
        private readonly string _username;
        private readonly string _password;
        #endregion

        #region Constructors
        public MessageInspector(bool useBasicAuth, string username, string password, string logger)
        {
            _useBasicAuth = useBasicAuth;
            _username = username;
            _password = password;
            _log = LogManager.GetLogger(logger);
        }
        #endregion
        #region IClientMessageInspector Members
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Basic Authentication:\t", _useBasicAuth);

            //Basic auth mean we need to set username and password.
            if (_useBasicAuth)
            {
                StringBuilder header = new StringBuilder("basic ")
                                        .Append(Convert.ToBase64String(Encoding.ASCII.GetBytes(_username + ":" + _password)));


                HttpRequestMessageProperty httpRequestMessage;
                object httpRequestMessageObject;
                if (request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out httpRequestMessageObject))
                {
                    httpRequestMessage = httpRequestMessageObject as HttpRequestMessageProperty;
                    if (string.IsNullOrEmpty(httpRequestMessage.Headers[HttpRequestHeader.Authorization]))
                    {
                        httpRequestMessage.Headers[HttpRequestHeader.Authorization] = header.ToString();
                    }
                }
                else
                {
                    var httpRequestProperty = new HttpRequestMessageProperty();
                    httpRequestProperty.Headers[HttpRequestHeader.Authorization] = header.ToString();
                    request.Properties.Add(HttpRequestMessageProperty.Name, httpRequestProperty);
                }
            }

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Request:\t{0}", MaskOutput(request.ToString()));

            return null;
        }

        private string MaskOutput(string xmlstring)
        {
            try
            {
                var xDoc = XDocument.Parse(xmlstring);
                XNamespace ns = "fcub";
                List<string> maskedItems = new List<string> { "username", "password" };

                foreach (var maskedItem in maskedItems)
                {
                    IEnumerable<XElement> list = xDoc.Descendants(ns + maskedItem);
                    foreach (var element in list)
                    {
                        element.Value = string.Empty.PadLeft(element.Value.Length, '*');
                    }
                }

                xmlstring = xDoc.ToString();
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            return xmlstring;
        }
       
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Response:\t{0}", reply.ToString());
        }

        #endregion
    }
}
