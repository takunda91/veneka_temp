using System;
using System.Collections.Generic;
using IndigoCardIssuanceService.DataContracts;
using IndigoFileLoader.bll;
using Veneka.Indigo.IssuerManagement;
using Veneka.Indigo.IssuerManagement.objects;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Models;
using Common.Logging;
using Veneka.Indigo.Common.Objects;
using Veneka.Indigo.Common.Exceptions;
using Veneka.Licensing.Common;
using IndigoFileLoader;
using Veneka.Indigo.UserManagement;

namespace IndigoCardIssuanceService.bll
{
    public class IssuerManagementController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(IssuerManagementController));
        private readonly BranchService _branchService = new BranchService();
        private readonly FileLoaderService _fileService = new FileLoaderService();
        private readonly IssuerService _issuerService = new IssuerService();

        #region EXPOSED METHODS

        #region Issuer

        /// <summary>
        /// Persist new issuer to the DB.
        /// </summary>
        /// <param name="issuer"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<IssuerResult> CreateIssuer(IssuerResult issuer, string pin_notification_message, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                IssuerResult issuerResult;
                string responseMessage;
                if (_issuerService.CreateIssuer(issuer,  pin_notification_message, languageId, auditUserId, auditWorkstation, out issuerResult, out responseMessage))
                {
                    return new Response<IssuerResult>(issuerResult,
                                                      ResponseType.SUCCESSFUL,
                                                      responseMessage,
                                                      "");
                }

                return new Response<IssuerResult>(null, ResponseType.UNSUCCESSFUL, responseMessage, "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<IssuerResult>(null,
                                                  ResponseType.ERROR,
                                                  "Error processing request, please try again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Persist changes to the DB.
        /// </summary>
        /// <param name="issuer"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal BaseResponse UpdateIssuer(IssuerResult issuer, string pin_notification_message, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_issuerService.UpdateIssuer(issuer, pin_notification_message, languageId, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Fetch all countires
        /// </summary>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<List<country>> GetCountries(long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<country>>(_issuerService.GetCountries(auditUserId, auditWorkstation),
                                                   ResponseType.SUCCESSFUL,
                                                   "",
                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<country>>(null,
                                                  ResponseType.ERROR,
                                                  "Error processing request, please try again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<issuers_Result>> GetAllIssuers(int? LanguageId, int? pageIndex, int? Rowsperpage)
        {
            #region Old Code

            //List<Issuer> issuers = _issuerService.GetAllIssuers();
            //int size = 0;
            //if (issuers != null)
            //    size = issuers.Count;

            //var dtos = new IssuerDTO[size];

            //for (int i = 0; i < size; i++)
            //{
            //    Issuer issuer = issuers[i];
            //    IssuerDTO dto = CreateIssuerDTO(issuer);
            //    dtos[i] = dto;
            //}
            //return dtos; 
            #endregion

            Response<List<issuers_Result>> rtnValue;
            try
            {
                rtnValue = new Response<List<issuers_Result>>(_issuerService.GetAllIssuers(LanguageId, pageIndex, Rowsperpage),
                                                      ResponseType.SUCCESSFUL,
                                                      "",
                                                      "");
            }// end try
            catch (Exception ex)
            {
                log.Error(ex);
                rtnValue = new Response<List<issuers_Result>>(null,
                                                      ResponseType.ERROR,
                                                      "Error processing request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }// end catch (Exception ex)

            return rtnValue;
        }

        /// <summary>
        /// Fetch issuer based on issuer id.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<IssuerResult> GetIssuer(int issuerId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<IssuerResult>(_issuerService.GetIssuer(issuerId, auditUserId, auditWorkstation),
                                                        ResponseType.SUCCESSFUL,
                                                        "",
                                                        "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<IssuerResult>(null,
                                                  ResponseType.ERROR,
                                                  "Error processing request, please try again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<IssuerResult> GetIssuerPinMessage(int issuerId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<IssuerResult>(_issuerService.GetIssuerPinMessage(issuerId, auditUserId, auditWorkstation),
                                                        ResponseType.SUCCESSFUL,
                                                        "",
                                                        "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<IssuerResult>(null,
                                                  ResponseType.ERROR,
                                                  "Error processing request, please try again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        #endregion

        #region LDAPSettings

        /// <summary>
        /// Fetch all ldap settings
        /// </summary>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<List<LdapSettingsResult>> GetLdapSettings(long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<LdapSettingsResult>>(_issuerService.GetLdapSettings(auditUserId, auditWorkstation),
                                                              ResponseType.SUCCESSFUL,
                                                              "",
                                                              "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<LdapSettingsResult>>(null,
                                                              ResponseType.ERROR,
                                                              "Error processing request, please try again.",
                                                              log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }


        /// <summary>
        /// Fetch all ldap settings
        /// </summary>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<List<AuthenticationtypesResult>> GetAuthenticationTypes(long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<AuthenticationtypesResult>>(_issuerService.GetAuthenticationTypes(auditUserId, auditWorkstation),
                                                              ResponseType.SUCCESSFUL,
                                                              "",
                                                              "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<AuthenticationtypesResult>>(null,
                                                              ResponseType.ERROR,
                                                              "Error processing request, please try again.",
                                                              log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        /// <summary>
        /// Persist new LDAP Setting to the database
        /// </summary>
        /// <param name="ldapSetting"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<int> CreateLdapSetting(LdapSettingsResult ldapSetting, int language, long auditUserId, string auditWorkstation)
        {
            try
            {
                int ldapSettingId;
                string responseMessage;
                if (_issuerService.CreateLdapSetting(ldapSetting, language, auditUserId, auditWorkstation, out ldapSettingId, out responseMessage))
                {
                    return new Response<int>(ldapSettingId, ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new Response<int>(0, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<int>(0,
                                         ResponseType.ERROR,
                                         "Error processing request, please try again.",
                                         log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Persist changes to LDAP Setting to DB
        /// </summary>
        /// <param name="ldapSetting"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        internal BaseResponse UpdateLdapSetting(LdapSettingsResult ldapSetting, int language, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_issuerService.UpdateLdapSetting(ldapSetting, language, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Persist changes to LDAP Setting to DB
        /// </summary>
        /// <param name="ldapSetting"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        internal BaseResponse DeleteLdapSetting(int ldap_setting_id, long auditUserId, string auditWorkstation)
        {
            try
            {

                _issuerService.DeleteLdapSetting(ldap_setting_id, auditUserId, auditWorkstation);
                return new BaseResponse(ResponseType.SUCCESSFUL,
                                        "",
                                        "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        #endregion

        #region Connection Params

        /// <summary>
        /// Fetch all connection parameters.
        /// </summary>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<List<ConnectionParamsResult>> GetConnectionParameters(long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<ConnectionParamsResult>>(_issuerService.GetConnectionParameters(auditUserId, auditWorkstation),
                                                                 ResponseType.SUCCESSFUL,
                                                                 "",
                                                                 "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<ConnectionParamsResult>>(null,
                                                                 ResponseType.ERROR,
                                                                 "Error processing request, please try again.",
                                                                 log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        
        internal Response<ConnectionParametersResult> GetConnectionParameter(int connParameterId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<ConnectionParametersResult>(_issuerService.GetConnectionParameter(connParameterId, auditUserId, auditWorkstation),
                                                                 ResponseType.SUCCESSFUL,
                                                                 "",
                                                                 "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<ConnectionParametersResult>(null,
                                                                 ResponseType.ERROR,
                                                                 "Error processing request, please try again.",
                                                                 log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Persist new connection param to the database
        /// </summary>
        /// <param name="connectionParam"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        internal Response<ConnectionParametersResult> CreateConnectionParam(ConnectionParametersResult connectionParam, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<ConnectionParametersResult>(_issuerService.CreateConnectionParam(connectionParam, auditUserId, auditWorkstation),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<ConnectionParametersResult>(null,
                                                            ResponseType.ERROR,
                                                            "Error processing request, please try again.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Persist changes to connection parameter to DB
        /// </summary>
        /// <param name="connectionParam"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        public BaseResponse UpdateConnectionParam(ConnectionParametersResult connectionParam, long auditUserId, string auditWorkstation)
        {
            try
            {
                _issuerService.UpdateConnectionParam(connectionParam, auditUserId, auditWorkstation);
                return new BaseResponse(ResponseType.SUCCESSFUL,
                                        "",
                                        "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Persist changes to Connection_parameter to DB
        /// </summary>
        /// <param name="ldapSetting"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        internal BaseResponse DeleteConnectionParam(int connectionParamId, long auditUserId, string auditWorkstation)
        {
            try
            {

                _issuerService.DeleteConnectionParam(connectionParamId, auditUserId, auditWorkstation);
                return new BaseResponse(ResponseType.SUCCESSFUL,
                                        "",
                                        "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Fetch products still linked to interfaces
        /// </summary>
        /// <param name="ldapSetting"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        internal Response<List<ProductInterfaceResult>> GetProductInterfaces(int connectionParamId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<ProductInterfaceResult>>(_issuerService.GetProductInterfaces(connectionParamId, auditUserId, auditWorkstation),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");

            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<ProductInterfaceResult>>(null,
                                                            ResponseType.ERROR,
                                                            "Error when processing request.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Fetch interface details for issuer.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="interfaceTypes"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        internal Response<ConnectionParamsResult> GetIssuerInterface(int issuerId, int interfaceTypeId, int interfaceAreaId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<ConnectionParamsResult>(_issuerService.GetIssuerInterface(issuerId, interfaceTypeId, interfaceAreaId, auditUserId, auditWorkstation),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");

            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<ConnectionParamsResult>(null,
                                                            ResponseType.ERROR,
                                                            "Error when processing request.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Fetch interface details for issuer.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="interfaceTypes"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        internal Response<List<IssuerInterfaceResult>> GetIssuerConnectionParams(int connectionParamId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<IssuerInterfaceResult>>(_issuerService.GetIssuerConnectionParams(connectionParamId, auditUserId, auditWorkstation),
                                                            ResponseType.SUCCESSFUL,
                                                            "",
                                                            "");

            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<IssuerInterfaceResult>>(null,
                                                            ResponseType.ERROR,
                                                            "Error when processing request.",
                                                            log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        
        ///// <summary>
        ///// Gets list of interfaces based on the issuer_id
        ///// </summary>
        ///// <param name="issuer_id"></param>
        ///// <returns></returns>
        //internal Response<List<InterfaceWrapper>> GetIssuerInterfaces(int issuer_id)
        //{
        //    Response<List<InterfaceWrapper>> rtnValue;
        //    try
        //    {
        //        rtnValue = new Response<List<InterfaceWrapper>>(_issuerService.GetIssuerInterfaces(issuer_id),
        //                                              ResponseType.SUCCESSFUL,
        //                                              "",
        //                                              "");
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex);
        //        rtnValue = new Response<List<InterfaceWrapper>>(null,
        //                                              ResponseType.ERROR,
        //                                              "Error processing request, please try again.",
        //                                              log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
        //    }

        //    return rtnValue;
        //}

        //internal Response<InterfaceWrapper> GetInterfaceDetails(int interface_id)
        //{
        //    Response<InterfaceWrapper> rtnValue;
        //    try
        //    {
        //        rtnValue = new Response<InterfaceWrapper>(_issuerService.GetInterfaceDetails(interface_id),
        //                                              ResponseType.SUCCESSFUL,
        //                                              "",
        //                                              "");
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex);
        //        rtnValue = new Response<InterfaceWrapper>(null,
        //                                              ResponseType.ERROR,
        //                                              "Error processing request, please try again.",
        //                                              log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
        //    }

        //    return rtnValue;
        //}


        //internal Response<ldap_setting> GetIssuerLdapSettings(int issuerId)
        //{
        //    Response<ldap_setting> rtnValue;

        //    try
        //    {
        //        rtnValue = new Response<ldap_setting>(_issuerService.GetIssuerLdapSettings(issuerId),
        //                                              ResponseType.SUCCESSFUL,
        //                                              "",
        //                                              "");
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex);
        //        rtnValue = new Response<ldap_setting>(null,
        //                                        ResponseType.ERROR,
        //                                        "Error processing request, please try again.",
        //                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
        //    }

        //    return rtnValue;
        //}

        ///// <summary>
        ///// Return a liset of issuer who use LDAP to authenticate users.
        ///// </summary>
        ///// <param name="auditUserId"></param>
        ///// <param name="auditWorkstation"></param>
        ///// <returns></returns>
        //internal Response<List<issuer>> GetLdapIssuers(long auditUserId, string auditWorkstation)
        //{
        //    try
        //    {
        //        return new Response<List<issuer>>(_issuerService.GetLdapIssuers(auditUserId, auditWorkstation),
        //                                          ResponseType.SUCCESSFUL, "", "");
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex);
        //        return new Response<List<issuer>>(null,
        //                                          ResponseType.ERROR,
        //                                          "Error processing request, please try again.",
        //                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
        //    }
        //}

        #endregion

        #region Licenses

        /// <summary>
        /// Persist license for issuer to DB.
        /// </summary>
        /// <param name="license"></param>
        /// <param name="key"></param>
        /// <param name="xmlLicenseFile"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        internal BaseResponse LoadIssuerLicense(IndigoComponentLicense license, string key, byte[] xmlLicenseFile, long auditUserId, string auditWorkstation)
        {
            try
            {
                string messages;
                if (_issuerService.LoadLicense(license, key, xmlLicenseFile, auditUserId, auditWorkstation, out messages))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL, "", "");
                }

                return new BaseResponse(ResponseType.UNSUCCESSFUL, messages, messages);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        #endregion

        #region BRANCH MANAGEMENT

        /// <summary>
        /// Persist new branch to the DB.
        /// </summary>
        /// <param name="createBranch"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<int> CreateBranch(branch createBranch, List<int> satellite_branches, int languages, long auditUserId, string auditWorkstation)
        {
            try
            {
                int branchId;
                string responseMessage;
                if (_branchService.CreateBranch(createBranch, satellite_branches, languages, auditUserId, auditWorkstation, out branchId, out responseMessage))
                {
                    return new Response<int>(branchId, ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new Response<int>(branchId, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (BaseIndigoException bex)
            {
                log.Warn(bex);
                return new Response<int>(0, ResponseType.UNSUCCESSFUL, bex.Message,
                                         log.IsDebugEnabled || log.IsTraceEnabled ? bex.ToString() : "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<int>(0,
                                         ResponseType.ERROR,
                                         "An error occured during processing your request, please try again.",
                                         log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        /// <summary>
        /// Persist updates to branch to the DB.
        /// </summary>
        /// <param name="createBranch"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal BaseResponse UpdateBranch(branch updateBranch, List<int> satellite_branches, int language, long auditUserId, string auditWorkstation)
        {
            try
            {
                string responseMessage;
                if (_branchService.UpdateBranch(updateBranch, satellite_branches,language, auditUserId, auditWorkstation, out responseMessage))
                {
                    return new BaseResponse(ResponseType.SUCCESSFUL, responseMessage, "");
                }

                return new BaseResponse(ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (BaseIndigoException bex)
            {
                log.Warn(bex);
                return new BaseResponse(ResponseType.UNSUCCESSFUL, bex.Message,
                                         log.IsDebugEnabled || log.IsTraceEnabled ? bex.ToString() : "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new BaseResponse(ResponseType.ERROR,
                                        "An error occured during processing your request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        internal Response<List<branch>> getBranchesForIssuer(int issuerId, int? cardCentre, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<branch>>(_branchService.getBranchesForIssuer(issuerId, cardCentre, languageId, auditUserId, auditWorkstation),
                                                          ResponseType.SUCCESSFUL,
                                                          "",
                                                          "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<branch>>(null,
                                                          ResponseType.ERROR,
                                                          "Error processing request, please try again.",
                                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Get's all the branches for a user based on issuer, role and all_branches_access
        /// </summary>
        /// <param name="issuer_id"></param>
        /// <param name="userRole"></param>
        /// <param name="encryptedUsername"></param>
        /// <param name="allBranchesAccess"></param>
        /// <returns></returns>
        internal Response<List<BranchesResult>> getAllBranchesForUser(int? issuer_id, long userId, UserRole? userRole, int? cardCentreBranchYN, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<BranchesResult>>(_branchService.getAllBranchesForUser(issuer_id, userId, (int?)userRole, cardCentreBranchYN, languageId, auditUserId, auditWorkstation),
                                                          ResponseType.SUCCESSFUL,
                                                          "",
                                                          "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<BranchesResult>>(null,
                                                          ResponseType.ERROR,
                                                          "Error processing request, please try again.",
                                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Get's all the branches for a user based on issuer, role and all_branches_access
        /// </summary>
        /// <param name="issuer_id"></param>
        /// <param name="userRole"></param>
        /// <param name="encryptedUsername"></param>
        /// <param name="allBranchesAccess"></param>
        /// <returns></returns>
        internal Response<List<BranchesResult>> getAllBranchesForUserAdmin(int? issuer_id, int? branchstatusid, long userId, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<BranchesResult>>(_branchService.getAllBranchesForUserAdmin(issuer_id, branchstatusid, userId, languageId, auditUserId, auditWorkstation),
                                                          ResponseType.SUCCESSFUL,
                                                          "",
                                                          "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<BranchesResult>>(null,
                                                          ResponseType.ERROR,
                                                          "Error processing request, please try again.",
                                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Get's all the branches for a user based on issuer, role and all_branches_access
        /// </summary>
        /// <param name="issuer_id"></param>
        /// <param name="userRole"></param>
        /// <param name="encryptedUsername"></param>
        /// <param name="allBranchesAccess"></param>
        /// <returns></returns>
        internal Response<List<BranchesResult>> getBranchesForUserroles(int? issuer_id, long userId, List<int> userRolesList, bool? branch_type_id, int languageId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<BranchesResult>>(_branchService.getBranchesForUserroles(issuer_id, userId, userRolesList, branch_type_id, languageId, auditUserId, auditWorkstation),
                                                          ResponseType.SUCCESSFUL,
                                                          "",
                                                          "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<BranchesResult>>(null,
                                                          ResponseType.ERROR,
                                                          "Error processing request, please try again.",
                                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Retrieve a list of branches as well as the number of cards they have according to the load batch status.
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="userId"></param>
        /// <param name="userRoleId"></param>
        /// <param name="loadBatchStatusId">May be null to fetch cards in any status</param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<List<BranchLoadCardCountResult>> GetBranchesLoadCardCount(int issuerId, long userId, UserRole userRole, Veneka.Indigo.CardManagement.LoadCardStatus? loadCardStatus, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<List<BranchLoadCardCountResult>>(_branchService.GetBranchesLoadCardCount(issuerId, userId, (int)userRole, (int?)loadCardStatus, auditUserId, auditWorkstation),
                                                                     ResponseType.SUCCESSFUL,
                                                                     "",
                                                                     "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<BranchLoadCardCountResult>>(null,
                                                                     ResponseType.ERROR,
                                                                     "Error processing request, please try again.",
                                                                     log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<int> GetBranchCardCount(int branchId, int productId, int? cardIssueMethidId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<int>(_branchService.GetBranchCardCount(branchId, productId, cardIssueMethidId, auditUserId, auditWorkstation),
                                         ResponseType.SUCCESSFUL,
                                         "",
                                         "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<int>(0,
                                         ResponseType.ERROR,
                                         "Error processing request, please try again.",
                                         log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        internal Response<int> GetDistBatchCount(long batchid, int branchId, int productId, int? cardIssueMethidId, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<int>(_branchService.GetDistBatchCount(batchid, branchId, productId,cardIssueMethidId, auditUserId, auditWorkstation),
                                         ResponseType.SUCCESSFUL,
                                         "",
                                         "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<int>(0,
                                         ResponseType.ERROR,
                                         "Error processing request, please try again.",
                                         log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }
        /// <summary>
        /// Get card count for branch
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="loadBatchStatusId">May be null to fetch cards in any status</param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<int?> GetBranchLoadCardCount(int branchId, Veneka.Indigo.CardManagement.LoadCardStatus? loadCardStatus, long auditUserId, string auditWorkstation)
        {
            try
            {
                return new Response<int?>(_branchService.GetBranchLoadCardCount(branchId, (int?)loadCardStatus, auditUserId, auditWorkstation),
                                          ResponseType.SUCCESSFUL,
                                          "",
                                          "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<int?>(null,
                                          ResponseType.ERROR,
                                          "Error processing request, please try again.",
                                          log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Get a branch by its Id.
        /// </summary>
        /// <param name="branchId"></param>
        /// <returns></returns>
        internal Response<branch> GetBranchById(int branchId)
        {
            Response<branch> rtnValue;
            try
            {
                rtnValue = new Response<branch>(_branchService.getBranchById(branchId),
                                                      ResponseType.SUCCESSFUL,
                                                      "",
                                                      "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                rtnValue = new Response<branch>(null,
                                                      ResponseType.ERROR,
                                                      "Error processing request, please try again.",
                                                      log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }

            return rtnValue;
        }

        #endregion

        #region File Load

        /// <summary>
        /// Search for a list of file loader logs.
        /// </summary>
        /// <param name="fileStatus"></param>
        /// <param name="fileName"></param>
        /// <param name="issuerId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowsPerpage"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<List<GetFileLoderLog_Result>> SearchFileLoadLog(int? fileLoadId, FileStatus? fileStatus, string fileName, int? issuerId,
                                                                          DateTime? dateFrom, DateTime? dateTo, int languageId, int pageIndex, int rowsPerpage,
                                                                          long auditUserId, string auditWorkstation)
        {
            try
            {

                return new Response<List<GetFileLoderLog_Result>>(_fileService.SearchFileLoadLog(fileLoadId, fileStatus, fileName, issuerId,
                                                                  dateFrom, dateTo, languageId, pageIndex, rowsPerpage,
                                                                  auditUserId, auditWorkstation),
                                                                  ResponseType.SUCCESSFUL,
                                                                   "",
                                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<GetFileLoderLog_Result>>(null,
                                                                   ResponseType.ERROR,
                                                                   "Error when processing request.",
                                                                   log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        #endregion

        #endregion
    }
}
