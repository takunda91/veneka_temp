using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using Veneka.Indigo.Integration.ProductPrinting;
using Veneka.Indigo.UX.NativeAppAPI;

namespace IndigoDesktopApp.NativeAppAPI
{
    public class NativeAppClient : IPINOperations, ICardPrinting
    {
        private readonly ICardPrinting _printProxy;
        private readonly IPINOperations _pinProxy;
        private readonly string urlPath = "NativeAPI.svc/soap";

        //public NativeAppClient(string url)
        //{
        //    Uri uri = null;
        //    if(Uri.TryCreate(url + urlPath, UriKind.RelativeOrAbsolute, out uri))
        //    {               

        //        WSHttpBinding binding = new WSHttpBinding();
        //        binding.Security.Mode = SecurityMode.Transport;
        //        EndpointAddress endpoint = new EndpointAddress(uri);

        //        ChannelFactory<IPINOperations> factory = new ChannelFactory<IPINOperations>(binding, endpoint);
        //        _proxy = factory.CreateChannel();

        //        IgnoreUntrustedSSL();

        //        //var result = _proxy.Logon("abcd", "efgh");
        //    }
        //    else
        //    {
        //        throw new ArgumentException("URL not in correct format: " + url);
        //    }

            

        //    //_allowUntrustedSSL = ConfigReader.AllowUntrustedSSL;
        //}

        public NativeAppClient(Uri url)
        {            
            Uri uri = null;
            if (Uri.TryCreate(url, urlPath, out uri))
            {
                WSHttpBinding binding = new WSHttpBinding();
                binding.Security.Mode = SecurityMode.Transport;
                EndpointAddress endpoint = new EndpointAddress(uri);

                ChannelFactory<ICardPrinting> cardFactory = new ChannelFactory<ICardPrinting>(binding, endpoint);
                _printProxy = cardFactory.CreateChannel();

                ChannelFactory<IPINOperations> pinFactory = new ChannelFactory<IPINOperations>(binding, endpoint);
                _pinProxy = pinFactory.CreateChannel();

                IgnoreUntrustedSSL();
            }
            else
            {
                throw new ArgumentException("URL not in correct format: " + url);
            }
        }

        public NativeAppClient(ICardPrinting cardPrinting, IPINOperations remoteComponent)
        {
            _printProxy = cardPrinting;
            _pinProxy = remoteComponent;
        }

        #region CardPrinting
        public Response<PrintJob> GetPrintJob(Token printToken, PrinterInfo printerInfo)
        {
            return _printProxy.GetPrintJob(printToken, printerInfo);
        }

        public Response<string> PrintFailed(Token printToken,string comments)
        {
            return _printProxy.PrintFailed(printToken,comments);
        }

        public Response<string> SendToPrinter(Token printToken)
        {
            return _printProxy.SendToPrinter(printToken);
        }
        public Response<string> PrintingComplete(Token printToken, CardData cardData)
        {
            return _printProxy.PrintingComplete(printToken, cardData);
        }

        public Response<string> PrinterAuditDetails(Token printToken, PrinterInfo printer)
        {
            return _printProxy.PrinterAuditDetails(printToken, printer);
        }

        public Response<string> PrinterAnalytics(Token printToken)
        {
            return _printProxy.PrinterAnalytics(printToken);
        }
        #endregion

        #region IPINOperations
        public Response<string> PINSignOn(string username, string password)
        {
            var result = _pinProxy.Logon(username, password);

            return result;
        }

        public Response<string[]> GetWorkingKey(Token token)
        {
            var result = _pinProxy.GetWorkingKey(token);

            if (result.Success)
                token.Session = result.Session;

            return result;
        }

        public Response<ProductSettings> GetProductConfig(CardData cardData, Token token)
        {
            var result = _pinProxy.GetProductConfig(cardData, token);

            if (result.Success)
                token.Session = result.Session;

            return result;
        }

        public Response<string> Complete(CardData cardData, Token token)
        {
            var result = _pinProxy.Complete(cardData, token);

            if (result.Success)
                token.Session = result.Session;

            return result;
        }
        #endregion

        private string HashPassword(string password)
        {            
            // Generate the hash, with an automatic 32 byte salt
            Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, 32);
            rfc2898DeriveBytes.IterationCount = 10000;
            byte[] hash = rfc2898DeriveBytes.GetBytes(20);
            byte[] salt = rfc2898DeriveBytes.Salt;
            //Return the salt and the hash
            return Convert.ToBase64String(salt) + "|" + Convert.ToBase64String(hash);
        }

        private void IgnoreUntrustedSSL()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | (SecurityProtocolType)3072 | SecurityProtocolType.Ssl3;

            ServicePointManager.ServerCertificateValidationCallback = (object s, X509Certificate certificate,
                                                                        X509Chain chain,
                                                                        SslPolicyErrors sslPolicyErrors) => true;
        }

        public Response<string> Logon(string username, string password)
        {
            throw new NotImplementedException();
        }        
    }
}
