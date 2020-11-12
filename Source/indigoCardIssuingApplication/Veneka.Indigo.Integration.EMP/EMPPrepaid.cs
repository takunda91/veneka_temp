using Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.EMP.DAL;
using Veneka.Indigo.Integration.EMP.FIMI;
using Veneka.Indigo.Integration.External;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.Integration.EMP
{
    [IntegrationExport("EMPPrepaid", "932D72BA-F6B8-429B-9348-5D6388A0846F", typeof(IPrepaidAccountProcessor))]
    public class EMPPrepaid : IPrepaidAccountProcessor
    {
        private static readonly ILog _cmsLog = LogManager.GetLogger(General.CBS_LOGGER);

        public IDataSource DataSource { get; set; }
        public DirectoryInfo IntegrationFolder { get; set; }

        public bool CreditPrepaidAccount(ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, decimal amount, string destinationAccountNumber, out PrepaidCreditResponse creditResponse, out string responseMessage)
        {
            responseMessage = String.Empty;
            creditResponse = new PrepaidCreditResponse();

            _cmsLog.Trace("CreditPrepaidAccount");
            try
            {
                WebServiceConfig webConfig = null;
                if (config is WebServiceConfig)
                {
                    webConfig = (WebServiceConfig)config;
                    _cmsLog.Trace("WebServiceConfig found and translated.");
                }
                else
                {
                    _cmsLog.Trace("Config parameters must be for Webservice.");
                    throw new ArgumentException("Config parameters must be for Webservice.");
                }
                FIMIWebService fimiService = new FIMIWebService(webConfig, new DefaultDataDAL(this.DataSource));

                if (String.IsNullOrWhiteSpace(destinationAccountNumber))
                {
                    _cmsLog.Trace("Funds load does not have destination account number.");
                    throw new ArgumentNullException("EMPPrepaidCredit", "Funds load does not have destination account number.");
                }

                if (fimiService.CreditPrepaidAccount(amount, destinationAccountNumber, auditUserId, auditWorkStation, out responseMessage))
                {
                    _cmsLog.Trace($"fimiService.CreditPrepaidAccount: Result is TRUE;  {responseMessage}");
                    return true;
                }
                else
                {
                    _cmsLog.Trace($"fimiService.CreditPrepaidAccount: Result is FALSE;  {responseMessage}");
                    return false;
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException endpointException)
            {
                _cmsLog.Error(endpointException);
                responseMessage = "Unable to connect to FIMI, please try again or contact support.";
                return false;
            }
            catch (Exception ex)
            {
                _cmsLog.Error(ex);
                responseMessage = ex.Message;
            }

            return false;
        }

        public bool GetPrepaidAccountDetail(ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, string pan, int mbr, out PrepaidAccountDetail prepaidAccountDetail, out string responseMessage)
        {
            responseMessage = String.Empty;
            prepaidAccountDetail = new PrepaidAccountDetail();
            bool checkComplete = false;

            _cmsLog.Trace($"GetPrepaidAccountDetail(.,.,.,.)");

            try
            {
                do
                {
                    checkComplete = GetPrepaidAccountDetailPass(externalFields, config, languageId, auditUserId, auditWorkStation, pan, mbr, out prepaidAccountDetail, out responseMessage);
                    if (checkComplete == false)
                    {
                        mbr++;
                        if (mbr > 9)
                        {
                            _cmsLog.Trace($"GetPrepaidAccountDetail: mbr = {mbr}, set checkComplete = true;");
                            checkComplete = true;
                        }
                    }
                } while (checkComplete == false);
            }
            catch (Exception exp)
            {
                _cmsLog.Error(exp);
                checkComplete = false;
            }
            return checkComplete;
        }

        private bool GetPrepaidAccountDetailPass(ExternalSystemFields externalFields, IConfig config, int languageId, long auditUserId, string auditWorkStation, string pan, int mbr, out PrepaidAccountDetail prepaidAccountDetail, out string responseMessage)
        {
            responseMessage = String.Empty;
            prepaidAccountDetail = new PrepaidAccountDetail();
            try
            {
                WebServiceConfig webConfig = null;
                if (config is WebServiceConfig)
                {
                    _cmsLog.Trace("GetPrepaidAccountDetailPass: config is WebServiceConfig");
                    webConfig = (WebServiceConfig)config;
                }
                else
                {
                    _cmsLog.Trace("Config parameters must be for Webservice.");
                    throw new ArgumentException("Config parameters must be for Webservice.");
                }

                FIMIWebService fimiService = new FIMIWebService(webConfig, new DefaultDataDAL(this.DataSource));

                if (String.IsNullOrWhiteSpace(pan))
                {
                    _cmsLog.Trace("PAN not specified.");
                    throw new ArgumentNullException("EMPPrepaidGetAccount", "PAN not specified.");
                }
                if (fimiService.GetPrepaidAccountDetail(pan, mbr, auditUserId, auditWorkStation, out prepaidAccountDetail, out responseMessage))
                {
                    _cmsLog.Trace("fimiService.GetPrepaidAccountDetail(....,....,...) returned TRUE");
                    return true;
                }
                else
                {
                    _cmsLog.Trace("fimiService.GetPrepaidAccountDetail(....,....,...) returned FALSE");
                    return false;
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException endpointException)
            {
                _cmsLog.Error(endpointException);
                responseMessage = "Unable to connect to FIMI, please try again or contact support.";
                throw;
            }
            catch (Exception ex)
            {
                _cmsLog.Error(ex);
                responseMessage = ex.Message;
                throw;
            }
        }
    }
}
