using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.Old_App_Code.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using Veneka.Indigo.UX.NativeAppAPI;

namespace Veneka.Indigo.UX.NativeAppAPI
{
    [ServiceBehavior(Namespace = Constants.NativeAppApiUrl)]
    // [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class NativeAPI : IGeneral, IPINOperations, ICardPrinting, INativeAppRest
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(NativeAPI));
        private readonly NativeAPIController _apiController = new NativeAPIController();
        private readonly Action _pinSelectAction = Action.PINSelect;
        private readonly Action _printSelection = Action.PrintCard;
        public Response<string> CheckStatus(string guid)
        {
            throw new NotImplementedException();
        }

        public int CheckStatusRest(string guid)
        {
            try
            {
                Guid sessionGuid;
                if (Guid.TryParse(guid, out sessionGuid))
                {

                    var status = NativeAPIController.CheckStatusSession(sessionGuid);
                    _log.Debug(d => d("Status for " + guid + " is " + status));

                    return status;
                }
                else
                {
                    _log.Warn("GUID Format incorrect.");
                    HttpContext.Current.Response.TrySkipIisCustomErrors = true;
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.StatusCode = 500;
                    throw new HttpException(500, "GUID format incorrect");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                HttpContext.Current.Response.TrySkipIisCustomErrors = true;
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.StatusCode = 500;
                throw new HttpException(500, ex.Message);
            }
        }


        public string LogonPINRest(string number)
        {
            throw new NotImplementedException();
            //return "REST";
        }

        public string LogonPrintingRest(string number)
        {
            throw new NotFiniteNumberException();
        }


        // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
        // To create an operation that returns XML,
        //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
        //     and include the following line in the operation body:
        //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";

        public Response<string> Logon(string username, string password)
        {
            throw new NotImplementedException();
            //return new Response<string>
            //{
            //    AdditionalInfo = "",
            //    Session = Jose.JWT.Encode("payload", Base64UrlDecode("key"), Jose.JwsAlgorithm.HS512),
            //    Success = true,
            //    Value = ""
            //};
        }

        #region Pin
        public Response<string[]> GetWorkingKey(Token token)
        {
            string message = String.Empty;

            try
            {
                Guid checkGuid; string sessionKey; long cardId; int branchId; bool isPinReissue;string printJobId, nextToken, productBin;
                if (NativeAPIController.ValidateToken(token.Session, _pinSelectAction, 0, out checkGuid, out sessionKey, out cardId, out branchId, out isPinReissue, out printJobId ,out productBin, out nextToken))
                {
                    var keys = TerminalService.Instance.GetTerminalSessionKey(token.DeviceID, sessionKey);
                    if (!string.IsNullOrWhiteSpace(keys))
                    {
                        return new Response<string[]>(true, message, new string[] { keys }, nextToken);
                    }
                }
                else
                {
                    message = "Invalid Token";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                _log.Error(ex);
            }

            return new Response<string[]>(false, message, null, "");
        }

        public Response<ProductSettings> GetProductConfig(CardData cardData, Token token)
        {
            string message = String.Empty;

            try
            {
                Guid checkGuid; string sessionKey; long cardId; int branchId; bool isPinReissue; string nextToken; TerminalParametersResult parameters;
                if (NativeAPIController.ValidateToken(token.Session, _pinSelectAction, 1, out checkGuid, out sessionKey, out cardId, out branchId, out isPinReissue, out string printJobId,out string productBin, out nextToken))
                {
                    if (TerminalService.Instance.LoadParameters(0, token.DeviceID, ConvertToTerminalCardData(cardData), isPinReissue, sessionKey, out parameters, out message))
                    {
                        ProductSettings prodSettings = new ProductSettings
                        {
                            ProductId = parameters.product_id,
                            MinPINLength = parameters.min_pin_length,
                            MaxPINLength = parameters.max_pin_length
                        };

                        return new Response<ProductSettings>(true, message, prodSettings, nextToken);
                    }
                }
                else
                {
                    message = "Invalid Token";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                _log.Error(ex);
            }

            return new Response<ProductSettings>(false, message, null, "");
        }

        public Response<string> Complete(CardData cardData, Token token)
        {
            string message = String.Empty;

            try
            {
                Guid checkGuid; string sessionKey; long cardId; int branchId; bool isPinReissue; string nextToken; PINResponse output;
                if (NativeAPIController.ValidateToken(token.Session, _pinSelectAction, 2, out checkGuid, out sessionKey, out cardId, out branchId, out isPinReissue, out string printJobId , out string productBin, out nextToken))
                {
                    var termCardData = ConvertToTerminalCardData(cardData);
                    termCardData.ReissueBranchId = branchId;
                    PINResponse pinResp;
                    if (TerminalService.Instance.UpdatePin(token.DeviceID, termCardData, sessionKey, out output, out message))
                    {
                        _log.Debug("Updating Status for " + checkGuid.ToString() + " to " + 2);
                        NativeAPIController.UpdateStatusSession(checkGuid, 2);
                        return new Response<string>(true, message, "", "DONE");
                    }
                }
                else
                {
                    message = "Invalid Token";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                _log.Error(ex);
            }

            return new Response<string>(false, message, "", "");
        }
        #endregion

        #region Helpers
        private TerminalCardData ConvertToTerminalCardData(CardData cardData)
        {
            TerminalCardData rsp = new TerminalCardData
            {
                IsPANEncrypted = cardData.IsPANEncrypted,
                IsTrack2Encrypted = cardData.IsTrack2Encrypted,
                PAN = cardData.PAN,
                PINBlock = cardData.PINBlock,
                //PINBlockFormat = (int)cardData.PINBlockFormat,
                Track2 = cardData.Track2,
                ProductId = cardData.ProductId,
                ReissueBranchId = cardData.BranchId

            };

            return rsp;
        }

        private byte[] Base64UrlDecode(string arg)
        {
            string s = arg;
            s = s.Replace('-', '+'); // 62nd char of encoding
            s = s.Replace('_', '/'); // 63rd char of encoding
            switch (s.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: s += "=="; break; // Two pad chars
                case 3: s += "="; break; // One pad char
                default:
                    throw new System.Exception(
             "Illegal base64url string!");
            }
            return Convert.FromBase64String(s); // Standard base64 decoder
        }
        #endregion

        #region Printer
        public Response<string> PrinterAuditDetails(Token printToken, PrinterInfo printer)
        {
            string message = string.Empty;

            try
            {
                Guid checkGuid; long cardId; int branchId; bool isPinReissue; string nextToken, sessionKey; ;
                if (NativeAPIController.ValidateToken(printToken.Session, _printSelection, 0, out checkGuid, out sessionKey, out cardId, out branchId, out isPinReissue, out string printJobId, out string productBin, out nextToken))
                {

                    var response = TerminalService.Instance.UpdatePrintCount(sessionKey, printer.SerialNo, printer.TotalPrints);

                    if (response)
                    {

                        return new Response<string>(true, message, null, nextToken);
                    }
                }
                else
                {
                    message = "Invalid Token";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                _log.Error(ex);
            }

            return new Response<string>(false, message, null, "");
        }

        public Response<PrintJob> GetPrintJob(Token printToken, PrinterInfo _printer)
        {
            string message = string.Empty;

            try
            {
                Guid checkGuid; string sessionKey; long cardId; int branchId; bool isPinReissue; string nextToken;
                if (NativeAPIController.ValidateToken(printToken.Session, _printSelection, 0, out checkGuid, out sessionKey, out cardId, out branchId, out isPinReissue, out string printJobId, out string productBin,out nextToken))
                {
                    // 
                    _printer.BranchId = branchId;
                    var response = TerminalService.Instance.RegisterPrinter(sessionKey, _printer, printJobId, cardId);
                    
                    if (response)
                    {
                        PrintJob printJob = new PrintJob();
                        printJob.PrintJobId = printJobId;
                        printJob.ProductBin = productBin;
                      //  printJob.ProductBin = "422764";// line is added for testing.                       
                        printJob.MustReturnCardData = true;
                        //need to remove logic
                         var prinfields=    TerminalService.Instance.GetProductFieldsByCardId(cardId, sessionKey);
                        
                        printJob.ProductFields = prinfields;
                         List<AppOptions>_options= new List<AppOptions>();
                        AppOptions _option1 = new AppOptions();
                        _option1.Key = "Indigo.Printer.SkipPrint";
                        _option1.Value = "false";
                        _options.Add(_option1);
                        // _option1 = new AppOptions();
                        //_option1.Key = "Zebra.ZC.CardSource";
                        //_option1.Value = "_Feeder";
                        //_options.Add(_option1);
                        
                        printJob.ApplicationOptions = _options.ToArray();
                        
                        return new Response<PrintJob>(true, message, printJob, nextToken);
                    }
                }
                else
                {
                    message = "Invalid Token";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                _log.Error(ex);
            }

            return new Response<PrintJob>(false, message, null, "");
        }

       

        public Response<string> PrintingComplete(Token printToken, CardData cardData)
        {

            string message = string.Empty;

            try
            {
                Guid checkGuid; string sessionKey; long cardId; int branchId; bool isPinReissue; string nextToken;
                if (NativeAPIController.ValidateToken(printToken.Session, _printSelection, 0, out checkGuid, out sessionKey, out cardId, out branchId, out isPinReissue, out string printJobId, out string productBin, out nextToken))
                {
                    TerminalCardData terminalCardData = ConvertToTerminalCardData(cardData);
                    var response = TerminalService.Instance.PrintingCompleted(sessionKey, cardId, printJobId, (int)PrintJobStatuses.PRINTED, terminalCardData);

                    if (response)
                    {
                        message = "Updating Status for " + checkGuid.ToString() + " to " + 2;
                        _log.Debug(message);
                        NativeAPIController.UpdateStatusSession(checkGuid, 2);
                        return new Response<string>(true, message, "", "DONE");
                    }
                }
                else
                {
                    message = "Invalid Token";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                _log.Error(ex);
            }

            return new Response<string>(false, message, null, "");
        }

       
    

        public Response<string> PrinterAnalytics(Token printToken)
        {
            throw new NotImplementedException();
        }

        public Response<string> PrintFailed(Token printToken, string comments)
        {
            string message = string.Empty;

            try
            {
                Guid checkGuid; string sessionKey; long cardId; int branchId; bool isPinReissue; string nextToken;
                if (NativeAPIController.ValidateToken(printToken.Session, _printSelection, 0, out checkGuid, out sessionKey, out cardId, out branchId, out isPinReissue, out string printJobId, out string productBin, out nextToken))
                {
                    // 
                    var response = TerminalService.Instance.UpdatePrintJobStatus(sessionKey, printJobId, (int)PrintJobStatuses.FAILED, comments, cardId);

                    if (response)
                    {
                        _log.Debug("Updating Status for " + checkGuid.ToString() + " to " + 1);
                        NativeAPIController.UpdateStatusSession(checkGuid,1);
                        return new Response<string>(true, comments, null, nextToken);
                    }
                }
                else
                {
                    message = "Invalid Token";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                _log.Error(ex);
            }

            return new Response<string>(false, message, null, "");
        }

        public Response<string> SendToPrinter(Token printToken)
        {
            string message = string.Empty;

            try
            {
                Guid checkGuid; string sessionKey; long cardId; int branchId; bool isPinReissue; string nextToken;
                if (NativeAPIController.ValidateToken(printToken.Session, _printSelection, 0, out checkGuid, out sessionKey, out cardId, out branchId, out isPinReissue, out string printJobId, out string productBin, out nextToken))
                {
                    // 
                    var response = TerminalService.Instance.UpdatePrintJobStatus(sessionKey, printJobId, (int)PrintJobStatuses.FAILED,string.Empty, cardId);

                    if (response)
                    {

                        return new Response<string>(true, message, null, nextToken);
                    }
                }
                else
                {
                    message = "Invalid Token";
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                _log.Error(ex);
            }

            return new Response<string>(false, message, null, "");
        }
        #endregion

        // Add more operations here and mark them with [OperationContract]
    }
}
