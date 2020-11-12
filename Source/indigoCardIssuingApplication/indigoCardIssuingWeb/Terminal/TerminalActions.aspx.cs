using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.CCO;
using indigoCardIssuingWeb.Old_App_Code.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace indigoCardIssuingWeb.Terminal
{
    public partial class TerminalActions : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TerminalActions));

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        [WebMethod]
        // [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetSessionKey(string deviceId)
        {
            try
            {
                throw new NotImplementedException();
                //SessionWrapper.PinIndex = null;
                //string key = TerminalService.Instance.GetTerminalSessionKey(deviceId, null);
                //return key.Substring(1);
            }
            catch(Exception ex)
            {
                log.Error(ex);
                HttpContext.Current.Response.TrySkipIisCustomErrors = true;
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.StatusCode = 500;
                throw new HttpException(500, ex.Message);
            }
        }

        [WebMethod]
        public static string[] LoadParameters(string deviceId, string track2)
        {
            try
            {
                throw new NotImplementedException();
                //string[] parameters;
                //string message;
                //SessionWrapper.TerminalProductId = null;

                //TerminalCardData termCardData = new TerminalCardData
                //{
                //    Track2 = track2
                //};

                //if (!TerminalService.Instance.LoadParameters(SessionWrapper.TerminalIssuerId, deviceId, termCardData, SessionWrapper.PINReissue, null, out TerminalParametersResult parameters, out message))
                //{
                //    HttpContext.Current.Response.TrySkipIisCustomErrors = true;
                //    HttpContext.Current.Response.Clear();
                //    HttpContext.Current.Response.StatusCode = 500;
                //    throw new HttpException(500, message);
                //}

                //SessionWrapper.TerminalProductId = parameters.product_id;//  int.Parse(parameters[0]);

                //return new string[0];                
            }
            catch (Exception ex)
            {
                log.Error(ex);
                HttpContext.Current.Response.TrySkipIisCustomErrors = true;
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.StatusCode = 500;
                throw new HttpException(500, ex.Message);
            }
        }
        
        [WebMethod]
        public static bool UpdatePin(string deviceId, string track2, string pinBlock)
        {
            try
            {
                throw new NotImplementedException();

                //SessionWrapper.PinIndex = null;

                //PINResponse pinIndex;
                //string message;

                //TerminalCardData termCardData = new TerminalCardData
                //{
                //    Track2 = track2,
                //    PINBlock = pinBlock,
                //    PinIssueCardId = SessionWrapper.PinIssueCardId,
                //    ReissueBranchId = SessionWrapper.ReissueBranchId,
                //    ProductId = SessionWrapper.TerminalProductId,
                //};


                //if (!TerminalService.Instance.UpdatePin(deviceId, termCardData, null, out pinIndex, out message))
                //{
                //    HttpContext.Current.Response.TrySkipIisCustomErrors = true;
                //    HttpContext.Current.Response.Clear();
                //    HttpContext.Current.Response.StatusCode = 500;
                //    throw new HttpException(500, message);
                //}

                ////SessionWrapper.PinIndex = pinIndex;

                //return true;
            }
            catch(Exception ex)
            {
                log.Error(ex);
                HttpContext.Current.Response.TrySkipIisCustomErrors = true;
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.StatusCode = 500;
                throw new HttpException(500, ex.Message);
            }
        }
    }
}