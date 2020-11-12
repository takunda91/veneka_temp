using Common.Logging;
using indigoCardIssuingWeb.CardIssuanceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace indigoCardIssuingWeb.Old_App_Code.service
{
    /// <summary>
    /// Base implementation for the service classes. Provides access to the indigo service as well as methods for checking response.
    /// </summary>
    public abstract class BaseService
    {
          protected readonly Service1SoapClient m_indigoApp = new Service1SoapClient();
                             


        /// <summary>
        /// Check the response from Indigo App, valid response returns true.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="log"></param>
        /// <param name="hardErrorException">Throws an exception for hard errors instead of retuning a message.</param>
        /// <param name="messages"></param>
        /// <returns></returns>
        protected bool CheckResponse(BaseResponse response, ILog log, bool hardErrorException, out string messages)
        {
            messages = response.ResponseMessage;

            switch (response.ResponseType)
            {
                case ResponseType.SUCCESSFUL:
                    return true;
                case ResponseType.UNSUCCESSFUL:
                    return false;
                case ResponseType.SESSION_ERROR:
                    SignOutUser(response, log);
                    break;
                default:
                    messages = log.IsDebugEnabled || log.IsTraceEnabled ? response.ResponseException : response.ResponseMessage;

                    if (hardErrorException)
                        throw new Exception(messages);
                    
                    return false;
            }            

            return false;
        }

        /// <summary>
        /// Returns failed and error responses in the output messages parameter. No exceptions generated
        /// </summary>
        /// <param name="response"></param>
        /// <param name="log"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        protected bool CheckResponse(BaseResponse response, ILog log, out string messages)
        {
            return this.CheckResponse(response, log, false, out messages);
        }

        /// <summary>
        /// Throws Exceptions on hard errors, else returns message in messages output parameter
        /// </summary>
        /// <param name="response"></param>
        /// <param name="log"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        protected bool CheckResponseException(BaseResponse response, ILog log, out string messages)
        {
            return this.CheckResponse(response, log, true, out messages);
        }

        /// <summary>
        /// Checks the response, if anything other than a successful response is received an exception is generated.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        protected bool CheckResponse(BaseResponse response, ILog log)
        {
            string messages = null;
            if (this.CheckResponse(response, log, out messages))
                return true;
            else
            {
               // SignOutUser(response, log);
               throw new Exception(messages);
               
            }
               
        }

        /// <summary>
        /// Only checks for session errors and logs the user out.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="log"></param>
        protected void CheckSession(BaseResponse response, ILog log)
        {
            if (response.ResponseType == ResponseType.SESSION_ERROR)
                SignOutUser(response, log);
        }

        /// <summary>
        /// Method for signing out the user and logging the warning.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="log"></param>
        private void SignOutUser(BaseResponse response, ILog log)
        {
            if (log.IsWarnEnabled) { log.WarnFormat("{0} {1}", response.ResponseMessage, response.ResponseException); }
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
        }
    }
}