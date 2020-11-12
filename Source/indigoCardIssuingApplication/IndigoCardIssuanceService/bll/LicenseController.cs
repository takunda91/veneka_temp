using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IndigoCardIssuanceService.DataContracts;
using Veneka.Indigo.Common.License;
using Veneka.Licensing.Common;
using Common.Logging;

namespace IndigoCardIssuanceService.bll
{
    /// <summary>
    /// This class contains all the mothods for dealing with indigo license management.
    /// </summary>
    public class LicenseController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LicenseController));
        private readonly LicenseManager _licenseManager = new LicenseManager();

        /// <summary>
        /// Retrieve the machine ID
        /// </summary>
        /// <returns></returns>
        internal Response<string> GetMachineId()
        {            
            try
            {
                return new Response<string>(Convert.ToBase64String(_licenseManager.MachineId),
                                            ResponseType.SUCCESSFUL,
                                            "",
                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<string>(null,
                                            ResponseType.ERROR,
                                            "Error processing request, please try again.",
                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }            
        }

        /// <summary>
        /// Load and validate license file for an issuer and persist to the DB.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="licenseFileLocation"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstaion"></param>
        /// <returns></returns>
        internal Response<IndigoComponentLicense> LoadIssuerLicense(byte[] licenseFileBytes, long auditUserId, string auditWorkstaion)
        {
            try
            {
                //TODO: Remove this.
                //licenseFileLocation = @"C:\veneka\indigo_group\license\CompLicTest.xml";

                string licenseKey = "";


                var issuerLicense = _licenseManager.ValidateAffiliateLicense(licenseFileBytes, out licenseKey);

                if (issuerLicense != null)
                {                    
                    IssuerManagementController issuerMan = new IssuerManagementController();
                    var response = issuerMan.LoadIssuerLicense(issuerLicense, licenseKey, licenseFileBytes, auditUserId, auditWorkstaion);

                    return new Response<IndigoComponentLicense>(issuerLicense,
                                                                response.ResponseType,
                                                                response.ResponseMessage,
                                                                response.ResponseException);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<IndigoComponentLicense>(null,
                                                            ResponseType.ERROR,
                                                            "Error processing request, please try again.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }


            return new Response<IndigoComponentLicense>(null,
                                                        ResponseType.UNSUCCESSFUL,
                                                        "There was an issue processing the license file.",
                                                        "");
        }

        internal Response<List<IndigoComponentLicense>> GetLicenseListIssuers(bool? licensedIssuers, long auditUserId, string auditWorkstation)
        {
            try
            {

                var issuerLicense = _licenseManager.GetLicenseListIssuers(licensedIssuers, auditUserId, auditWorkstation);

                return new Response<List<IndigoComponentLicense>>(issuerLicense,
                                                                  ResponseType.SUCCESSFUL, "", "");


            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<IndigoComponentLicense>>(null,
                                                                  ResponseType.ERROR,
                                                                  "Error processing request, please try again.",
                                                                  log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
    }
}