using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;

namespace indigoCardIssuingWeb.Old_App_Code.service
{
    public sealed class TerminalService : BaseService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TerminalService));        

        private static volatile TerminalService instance;
        private static object syncRoot = new Object();

        public static TerminalService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new TerminalService();
                    }
                }

                return instance;
            }
        }


        internal string GetTerminalSessionKey(string deviceId, string sessionKey)
        {
            if (String.IsNullOrWhiteSpace(sessionKey))
                sessionKey = HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey;


            string encryptedSessionKey = EncryptionManager.EncryptString(sessionKey,
                                                                            SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                            SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.GetTerminalSessionKey(deviceId, encryptedSessionKey);

            base.CheckResponse(response, log);

            //Store encrypted keys in the users session.
            TerminalSessionKey tsk = response.Value;
            //SetTerminalKeys(sessionKey, tsk);

            //SessionWrapper.TerminalSessionKeys = tsk;

            //Decrypt the random key for terminal use
            return EncryptionManager.DecryptString(tsk.RandomKey,
                                                    SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                    SecurityParameters.EXTERNAL_SECURITY_KEY);
        }

        internal bool LoadParameters(int issuerId, string deviceId, TerminalCardData termCardData, bool isPinreissue, string sessionKey, out TerminalParametersResult parameters, out string message)
        {
            if (String.IsNullOrWhiteSpace(sessionKey))
                sessionKey = HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey;

            parameters = null;
            message = String.Empty;

            if (!String.IsNullOrWhiteSpace(termCardData.PAN))
            {
                termCardData.PAN = EncryptionManager.EncryptString(termCardData.PAN,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);
            }

            if (!String.IsNullOrWhiteSpace(termCardData.Track2))
            {
                termCardData.Track2 = EncryptionManager.EncryptString(termCardData.Track2,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);
            }

            string encryptedSessionKey = EncryptionManager.EncryptString(sessionKey,
                                                                            SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                            SecurityParameters.EXTERNAL_SECURITY_KEY);

            //if(!GetTerminalKeys(sessionKey, out TerminalSessionKey termkeys))
            //{
            //    message = "Keys expired, please restart.";
            //    return false;
            //}

            //isPinreissue, termkeys.RandomKeyUnderLMK,
            var response = m_indigoApp.LoadDeviceParameters(issuerId, deviceId, termCardData, encryptedSessionKey);

            if(base.CheckResponseException(response, log, out message))
            {
                parameters = response.Value;
                message = "";
                return true;
            }
            else
            {
                if (response.Value != null)
                    parameters = response.Value;
                else
                    parameters = null;
            }

            return false;
        }

        internal bool UpdatePin(string deviceId, TerminalCardData termCardData, string sessionKey, out PINResponse output, out string message)
        {
            if (String.IsNullOrWhiteSpace(sessionKey))
                sessionKey = HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey;

            output = null;
            message = String.Empty;

            string encryptedSessionKey = EncryptionManager.EncryptString(sessionKey,
                                                                            SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                            SecurityParameters.EXTERNAL_SECURITY_KEY);

            if (!String.IsNullOrWhiteSpace(termCardData.PAN))
            {
                termCardData.PAN = EncryptionManager.EncryptString(termCardData.PAN,
                                                                         SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                         SecurityParameters.EXTERNAL_SECURITY_KEY);
            }

            if (!String.IsNullOrWhiteSpace(termCardData.Track2))
            {
                termCardData.Track2 = EncryptionManager.EncryptString(termCardData.Track2,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);
            }

            if (!String.IsNullOrWhiteSpace(termCardData.PINBlock))
            {
                termCardData.PINBlock = EncryptionManager.EncryptString(termCardData.PINBlock,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);
            }

            //if (!GetTerminalKeys(sessionKey, out TerminalSessionKey terminalKeys))
            //{
            //    message = "Terminal keys expired.";
            //    return false;
            //}

            var response = m_indigoApp.UpdatePin(deviceId, 
                                //terminalKeys.RandomKeyUnderLMK, 
                                termCardData,
                                encryptedSessionKey);


            if(base.CheckResponseException(response, log, out message))
            {
                output = response.Value;
                message = "";

                SetPINIndex(sessionKey, response.Value);
                return true;
            }
            else
            {
                if (response.Value != null)
                    output = response.Value;
                else
                    output = null;
            }

            return false;
        }
        public Veneka.Indigo.Integration.ProductPrinting.ProductField[] GetProductFieldsByCardId(long Card_id,string sessionKey)
        {
            if (String.IsNullOrWhiteSpace(sessionKey))
                sessionKey = HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey;


            string encryptedSessionKey = EncryptionManager.EncryptString(sessionKey,
                                                                            SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                            SecurityParameters.EXTERNAL_SECURITY_KEY);

          var response =  m_indigoApp.GetProductFieldsByCardId(Card_id, encryptedSessionKey);
            string message;
            
            if (base.CheckResponseException(response, log, out message))
            {
                return CovertProductField(response.Value).ToArray();
            }
            else
            {
                
                return null;
            }
        }
        public List<Veneka.Indigo.Integration.ProductPrinting.ProductField> CovertProductField(indigoCardIssuingWeb.CardIssuanceService.ProductField[] productFields)
        {
            List<Veneka.Indigo.Integration.ProductPrinting.ProductField> _result = new List<Veneka.Indigo.Integration.ProductPrinting.ProductField>();
            foreach (var item in productFields)
            {
                _result.Add(new Veneka.Indigo.Integration.ProductPrinting.ProductField() { Deleted=item.Deleted, Editable=item.Editable,Font=item.Font,FontColourRGB=item.FontColourRGB,FontSize=item.FontSize,Height=item.Height,Label=item.Label,MappedName=item.MappedName,MaxSize=item.MaxSize, Name=item.Name,Printable=item.Printable,PrintSide=item.ProductPrintFieldId, ProductPrintFieldId=item.ProductPrintFieldTypeId,Value=item.Value,ProductPrintFieldTypeId=item.ProductPrintFieldTypeId,Width=item.Width,X=item.X,Y=item.Y  });
            }
            return _result;
        }
        internal PrintJobStatus GetPrintJobStatus(string printJobId)

        {
            string encryptedSessionKey = EncryptionManager.EncryptString(HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey,
                                                                     SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                     SecurityParameters.EXTERNAL_SECURITY_KEY);


            var response = m_indigoApp.GetPrintJobStatus(printJobId, encryptedSessionKey);

            base.CheckResponse(response, log);

            return response.Value;

        }

            public string InsertPrintJob(string sessionKey,string serialNo,long cardId)
        {
            if (String.IsNullOrWhiteSpace(sessionKey))
                sessionKey = HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey;


            string encryptedSessionKey = EncryptionManager.EncryptString(sessionKey,
                                                                            SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                            SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.InsertPrintJob(serialNo,-1,cardId,0, encryptedSessionKey);

            if (base.CheckResponseException(response, log, out string message))           
            {
                if (response.Value != null)
                   return  response.Value;
                else
                    return  null;
            }
            return null;
        }

        public bool UpdatePrintJobStatus(string sessionKey, string printJobId,int printJobStatuses,string comments, long cardId)
        {
            if (String.IsNullOrWhiteSpace(sessionKey))
                sessionKey = HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey;


            string encryptedSessionKey = EncryptionManager.EncryptString(sessionKey,
                                                                            SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                            SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.UpdatePrintJobStatus(printJobId, printJobStatuses,comments, encryptedSessionKey);

          return  base.CheckResponse(response, log);
        }
        public bool PrintingCompleted(string sessionKey,long cardId,string printJobId,int printJobStatuses, TerminalCardData terminalCarddata)
        {
            if (String.IsNullOrWhiteSpace(sessionKey))
                sessionKey = HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey;


            string encryptedSessionKey = EncryptionManager.EncryptString(sessionKey,
                                                                            SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                            SecurityParameters.EXTERNAL_SECURITY_KEY);

            var response = m_indigoApp.PrintingCompleted(terminalCarddata.PAN,cardId, printJobId, printJobStatuses, encryptedSessionKey);

            return base.CheckResponse(response, log);

          
        }
        public bool UpdatePrintCount(string sessionKey,string serialNo, int totalPrints)
        {
            if (String.IsNullOrWhiteSpace(sessionKey))
                sessionKey = HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey;


            string encryptedSessionKey = EncryptionManager.EncryptString(sessionKey,
                                                                            SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                            SecurityParameters.EXTERNAL_SECURITY_KEY);
            var response = m_indigoApp.UpdatePrintCount(serialNo, totalPrints, encryptedSessionKey);

            return base.CheckResponse(response, log);
        }
        public bool RegisterPrinter(string sessionKey, Veneka.Indigo.UX.NativeAppAPI.PrinterInfo printer,string printJobId,long cardId)
        {
            if (String.IsNullOrWhiteSpace(sessionKey))
                sessionKey = HttpContext.Current.User.ToIndigoPrincipal().IndigoIdentity.SessionKey;


            string encryptedSessionKey = EncryptionManager.EncryptString(sessionKey,
                                                                            SecurityParameters.USE_HASHING_FOR_ENCRYPTION,
                                                                            SecurityParameters.EXTERNAL_SECURITY_KEY);
            Printer _printer = new Printer() { SerialNo = printer.SerialNo, FirmwareVersion = printer.FirmwareVersion, Model = printer.Model, Manufacturer = printer.Manufacturer };
            var response = m_indigoApp.RegisterPrinter(_printer, printJobId, encryptedSessionKey);

           return base.CheckResponse(response, log);
        }
        public void SetTerminalKeys(string sessionKey, TerminalSessionKey terminalKeys)
        {
            // Check if anythings in the cache for this session and remove it
            if (HttpRuntime.Cache.Get(sessionKey) != null)
                HttpRuntime.Cache.Remove(sessionKey);

            HttpRuntime.Cache.Insert(sessionKey, terminalKeys, null, DateTime.UtcNow.Add(new TimeSpan(0, 20, 0)), System.Web.Caching.Cache.NoSlidingExpiration);
        }

        private bool GetTerminalKeys(string sessionKey, out TerminalSessionKey terminalKeys)
        {
            terminalKeys = null;

            var item = HttpRuntime.Cache.Get(sessionKey);

            if (item == null)
                return false;

            if (item is TerminalSessionKey)
            {
                terminalKeys = (TerminalSessionKey)item;
                return true;
            }

            return false;
        }


        private void SetPINIndex(string sessionKey, PINResponse pinResponse)
        {
            // Check if anythings in the cache for this session and remove it
            if (HttpRuntime.Cache.Get(sessionKey) != null)
                HttpRuntime.Cache.Remove(sessionKey);

            HttpRuntime.Cache.Insert(sessionKey, pinResponse, null, DateTime.UtcNow.Add(new TimeSpan(0, 20, 0)), System.Web.Caching.Cache.NoSlidingExpiration);
        }

        public static bool GetPINIndex(string sessionKey, out PINResponse pinResponse)
        {
            pinResponse = null;

            var item = HttpRuntime.Cache.Get(sessionKey);

            if (item == null)
                return false;

            if(item is PINResponse)
            {
                pinResponse = (PINResponse)item;
                return true;
            }

            return false;
        }

        public static void ClearPINIndex(string sessionKey)
        {
            HttpRuntime.Cache.Remove(sessionKey);
        }

    }
}