using System;
using System.Collections.Generic;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.Integration.Common;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.Caching;
using System.Linq;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.COMS.Core;
using Veneka.Indigo.Security;

using Veneka.Indigo.COMS.Core.Integration;
using Veneka.Indigo.COMS.Core.Indigo;

namespace Veneka.Indigo.COMS.Core.Terminal
{
    public class TerminalController
    {
        private enum IsoResponseCodes
        {
            Success = 0,
            IncorrectOrExpiredOperatorKey = 20,
            DeviceNotFoundOrAuthorised = 50,
            ProductNotFoundOrAuthorised = 51,
            TPKGenerationFailed = 70,
            PVVFailed = 71,
            IndigoRequestFailed = 80,
            NotImplemented = 90,        
            UnexpectedError = 99

        }


        //private static readonly ILog log = LogManager.GetLogger(typeof(TerminalController));
        private readonly ITerminalCallback _terminalManager;
        private readonly IPINReissueCallback _pinReissue;
        private readonly IDataSource _dataSource;
        
        //private SessionManager _sessionManager = SessionManager.GetInstance();
        private static readonly Random rndOperatorKey = new Random();

        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        //private static readonly Cache m_secureCache = new Cache();
        private static readonly ObjectCache _secureCache = new MemoryCache("secureCache");
        private static readonly SecureCache _secureSessionCache = new SecureCache("secureSessionCache", new TimeSpan(12, 0, 0));
        private static readonly ObjectCache _operatorKeys = new MemoryCache("operatorKeys");


        public TerminalController(IDataSource dataSource)
        {
            _terminalManager = null;
            _pinReissue = null;
            _dataSource = dataSource;
        }


        #region EXPOSED METHODS

        internal static Tuple<Track2, byte[]> FetchIndex(byte[] index)
        {
            if (_secureCache.Contains(Encoding.UTF8.GetString(index)))
                return (Tuple<Track2, byte[]>)_secureCache.Get(Encoding.UTF8.GetString(index));

            return null;
        }
        #endregion


        #region PIN SET GOOD


        #region Operator Session Key
        ///// <summary>
        ///// Generates a random key for the operator to use when doing PIN reset.
        ///// </summary>
        ///// <param name="languageId"></param>
        ///// <param name="auditUserId"></param>
        ///// <param name="auditWorkstation"></param>
        ///// <returns></returns>
        //internal ComsResponse<string> GetOperatorSessionKey(int languageId, long auditUserId, string auditWorkstation)
        //{
        //    CacheItemPolicy policy = new CacheItemPolicy();

        //    //check if the operator has a key assigned to him/her, if he/she does remove it and assign new key
        //    foreach (var operatorKey in _operatorKeys.Where(w => (long)w.Value == auditUserId))
        //        _operatorKeys.Remove(operatorKey.Key);

        //    //100 tries to generate a random key not assigned to someone else.
        //    //The key is only 4 digits long therefore the more operators the more there might be clashes.
        //    for (int count = 0; count < 100; count++)
        //    {
        //        // Build a random key for the operator
        //        StringBuilder randomKey = new StringBuilder();

        //        for (int i = 0; i < 4; i++)
        //            randomKey.Append(rndOperatorKey.Next(0, 9));

        //        //Make sure we dont end up with the same key
        //        if (!_operatorKeys.Contains(randomKey.ToString()))
        //        {
        //            //Key cached for 15 minutes
        //            policy.AbsoluteExpiration = DateTime.Now.AddMinutes(15);
        //            _operatorKeys.Add(randomKey.ToString(), auditUserId, policy);

        //            //Encrypt the keys before we send them out for additional layer of security
        //            var encrypted = EncryptionManager.EncryptString(randomKey.ToString(),
        //                                                                    StaticFields.USE_HASHING_FOR_ENCRYPTION,
        //                                                                    StaticFields.EXTERNAL_SECURITY_KEY);

        //            return new Response<string>(encrypted, ResponseType.SUCCESSFUL, "", "");
        //        }
        //    }

        //    return new Response<string>(null, ResponseType.UNSUCCESSFUL, "Could not generate key, please try again.", "Could not generate key, please try again.");
        //}
        #endregion

        #region Terminal Session Key
        ///// <summary>
        ///// Generates Session Key for the Terminal and encrypts it for transport over Indigo App API
        ///// </summary>
        ///// <param name="deviceId"></param>
        ///// <param name="languageId"></param>
        ///// <param name="auditUserId"></param>
        ///// <param name="auditWorkstation"></param>
        ///// <returns></returns>
        //internal ComsResponse<TerminalSessionKey> GetTerminalSessionKey(string deviceId, int languageId, long auditUserId, string auditWorkstation)
        //{
        //    var resp = this.GetTerminalSessionKeyClear(deviceId, languageId, auditUserId, auditWorkstation);

        //    if (resp.ResponseType == ResponseType.SUCCESSFUL)
        //    {
        //        //Encrypt the keys before we send them out for additional layer of security
        //        resp.Value.RandomKey = EncryptionManager.EncryptString(resp.Value.RandomKey,
        //                                                                        StaticFields.USE_HASHING_FOR_ENCRYPTION,
        //                                                                        StaticFields.EXTERNAL_SECURITY_KEY);

        //        resp.Value.RandomKeyUnderLMK = EncryptionManager.EncryptString(resp.Value.RandomKeyUnderLMK,
        //                                                                        StaticFields.USE_HASHING_FOR_ENCRYPTION,
        //                                                                        StaticFields.EXTERNAL_SECURITY_KEY);
        //    }

        //    return resp;
        //}

        /// <summary>
        /// Generate the TPK for the terminal.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public TerminalResponse<TerminalSessionKey> GetTerminalSessionKeyClear(string deviceId, int languageId, long auditUserId, string auditWorkstation)
        {            
            string responseMessage;
            TerminalDetails terminal;

            var auditInfo = new AuditInfo
            {
                AuditUserId = auditUserId,
                AuditWorkStation = auditWorkstation,
                LanguageId = languageId
            };
            
            //Fetch device info, also checks if user has access to use the device.
            if (!GetDevice(deviceId, auditInfo, out terminal, out responseMessage))
            {
                return TerminalResponse<TerminalSessionKey>.Failed(responseMessage, ((int)IsoResponseCodes.DeviceNotFoundOrAuthorised).ToString("00"), null);
            }

            try
            {
                TerminalSessionKey terminalSessionKey;

                
                var hsmResp = IntegrationController.Instance.HardwareSecurityModule(terminal.HSMConfig, _dataSource)
                                .GenerateRandomKey(terminal.IssuerId, terminal.TerminalMasterKey, deviceId, terminal.HSMConfig.Config,
                                                auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out responseMessage, out terminalSessionKey);

                if (hsmResp)
                {
                    _secureSessionCache.SetCacheItem<TerminalSessionKey>(deviceId, terminalSessionKey);

                    return TerminalResponse<TerminalSessionKey>.Success(responseMessage, (int)IsoResponseCodes.Success, terminalSessionKey);
                }
                else
                {
                    //log.Error(responseMessage);

                    return TerminalResponse<TerminalSessionKey>.Failed(responseMessage, (int)IsoResponseCodes.TPKGenerationFailed, null);
                }

            }
            catch (NotImplementedException nie)
            {
                //log.Warn(deviceId + " : GetTerminalSessionKey() method in HSM module not implemented.", nie);

                responseMessage = "HSM module not implemented.";
                return TerminalResponse<TerminalSessionKey>.Failed(responseMessage, (int)IsoResponseCodes.NotImplemented, null);                
            }
            catch (Exception ex)
            {
                //log.Error(deviceId + " : Unexpected error.", ex);
                return TerminalResponse<TerminalSessionKey>.Failed("Error when processing request.", (int)IsoResponseCodes.UnexpectedError, null);
            }
        }
        #endregion

        #region Fetch Product Parameters
        //internal TerminalResponse<TerminalParametersResult> GetProductParameters(int issuerId, string deviceId, TerminalCardData cardData, int languageId, long auditUserId, string auditWorkstation)
        //{
        //    if (!String.IsNullOrWhiteSpace(cardData.PAN))
        //    {
        //        cardData.PAN = EncryptionManager.DecryptString(cardData.PAN,
        //                                                            StaticFields.USE_HASHING_FOR_ENCRYPTION,
        //                                                            StaticFields.EXTERNAL_SECURITY_KEY);
        //    }

        //    if (!String.IsNullOrWhiteSpace(cardData.Track2))
        //    {
        //        cardData.Track2 = EncryptionManager.DecryptString(cardData.Track2,
        //                                                            StaticFields.USE_HASHING_FOR_ENCRYPTION,
        //                                                            StaticFields.EXTERNAL_SECURITY_KEY);
        //    }

        //    string responseMessage;
        //    DecryptedFields decryptedFields;

        //    try
        //    {
        //        //Get TPK for device for use when decrypting fields from terminal
        //        var deviceTPK = GetDeviceTPK(deviceId);

        //        //Fetch device info, also checks if user has access to use the device.
        //        if (!GetDevice(deviceId, auditUserId, auditWorkstation, out TerminalTMKResult device, out responseMessage))
        //        {
        //            return new Response<TerminalParametersResult>(null, ResponseType.UNSUCCESSFUL, responseMessage, ((int)IsoResponseCodes.DeviceNotFoundOrAuthorised).ToString("00"));
        //        }

                
        //        ProductDAL prodDAL = new ProductDAL(Veneka.Indigo.Common.Database.DatabaseConnectionObject.Instance.SQLConnectionString);
        //        Product product = null;

        //        // Search for the product, if PAN is in the clear and supplied
        //        if (!cardData.IsPANEncrypted && !String.IsNullOrWhiteSpace(cardData.PAN))
        //        {
        //            product = prodDAL.FindBestMatch(device.issuer_id, cardData.PAN, true, auditUserId, auditWorkstation);

        //            if (product == null)
        //                log.Warn("Product not found");
        //        }                

        //        if (!FieldDecryption(deviceId, device, cardData, deviceTPK, String.Empty, product, languageId, auditUserId, auditWorkstation, out decryptedFields, out responseMessage))
        //            return new Response<TerminalParametersResult>(null, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);

        //        if (product == null)
        //        {
        //            product = prodDAL.FindBestMatch(device.issuer_id, decryptedFields.TrackData.PAN, true, auditUserId, auditWorkstation);
        //        }

        //        if (product == null || product.ProductId <= 0)
        //            return new Response<TerminalParametersResult>(null, ResponseType.UNSUCCESSFUL, "No card product for card found in system.", "No card product for card found in system.");
                                
        //        // Fetch Terminal Parameters
        //        if(GetTerminalParameters(languageId, auditUserId, auditWorkstation, product, out TerminalParametersResult terminalParameters, out responseMessage))
        //        {
        //            return new Response<TerminalParametersResult>(terminalParameters, ResponseType.SUCCESSFUL, responseMessage, responseMessage);
        //        }

        //        return new Response<TerminalParametersResult>(null, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
        //    }
        //    catch (NotImplementedException nie)
        //    {
        //        log.Error(nie);
        //        return new Response<TerminalParametersResult>(null,
        //                                                        ResponseType.ERROR,
        //                                                        "HSM not implemented.",
        //                                                        log.IsDebugEnabled || log.IsTraceEnabled ? nie.ToString() : "");                
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex);
        //        return new Response<TerminalParametersResult>(null,
        //                                                        ResponseType.ERROR,
        //                                                        "Error when processing request.",
        //                                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
        //    }
        //}

        
        #endregion

        #region PIN Reset
        ///// <summary>
        ///// PinPad App implementation
        ///// </summary>
        ///// <param name="deviceId"></param>
        ///// <param name="encryptedTpkUnderLMK"></param>
        ///// <param name="encryptedPinBlock"></param>
        ///// <param name="encryptedTrackData"></param>
        ///// <param name="cardId"></param>
        ///// <param name="branchId"></param>
        ///// <param name="productId"></param>
        ///// <param name="languageId"></param>
        ///// <param name="auditUserId"></param>
        ///// <param name="auditWorkstation"></param>
        ///// <returns></returns>
        //internal ComsResponse<PINResponse> TerminalPINReset(string deviceId, TerminalCardData cardData, int languageId, long auditUserId, string auditWorkstation)
        //{
        //    if (!String.IsNullOrWhiteSpace(cardData.PAN))
        //    {
        //        cardData.PAN = EncryptionManager.DecryptString(cardData.PAN,
        //                                                            StaticFields.USE_HASHING_FOR_ENCRYPTION,
        //                                                            StaticFields.EXTERNAL_SECURITY_KEY);
        //    }

        //    if (!String.IsNullOrWhiteSpace(cardData.Track2))
        //    {
        //        cardData.Track2 = EncryptionManager.DecryptString(cardData.Track2,
        //                                                            StaticFields.USE_HASHING_FOR_ENCRYPTION,
        //                                                            StaticFields.EXTERNAL_SECURITY_KEY);
        //    }

        //    if (!String.IsNullOrWhiteSpace(cardData.PINBlock))
        //    {
        //        cardData.PINBlock = EncryptionManager.DecryptString(cardData.PINBlock,
        //                                                            StaticFields.USE_HASHING_FOR_ENCRYPTION,
        //                                                            StaticFields.EXTERNAL_SECURITY_KEY);
        //    }

        //    string responseMessage;

        //    //Get TPK for device for use when decrypting fields from terminal
        //    var deviceTPK = GetDeviceTPK(deviceId);

        //    //Fetch device info, also checks if user has access to use the device.
        //    if (!GetDevice(deviceId, auditUserId, auditWorkstation, out TerminalTMKResult device, out responseMessage))
        //    {
        //        return new Response<PINResponse>(null, ResponseType.UNSUCCESSFUL, responseMessage, ((int)IsoResponseCodes.DeviceNotFoundOrAuthorised).ToString("00"));
        //    }            

        //    //Decrypt all the fields
        //    try
        //    {
        //        //Integration
        //        IntegrationController _integration = IntegrationController.Instance;

        //        Veneka.Indigo.Integration.Config.IConfig config;
        //        _integration.HardwareSecurityModule(device.issuer_id, InterfaceArea.ISSUING, out config);

        //        InterfaceInfo interfaceInfo = new InterfaceInfo
        //        {
        //            Config = config,
        //            InterfaceGuid = config.InterfaceGuid.ToString()
        //        };

        //        AuditInfo auditInfo = new AuditInfo
        //        {
        //            AuditUserId = auditUserId,
        //            AuditWorkStation = auditWorkstation,
        //            LanguageId = languageId
        //        };


        //        var hsm = COMSController.ComsCore;

        //        //Lookup card product
        //        ProductDAL prodDAL = new ProductDAL(Veneka.Indigo.Common.Database.DatabaseConnectionObject.Instance.SQLConnectionString);
                
        //        //Get product details from product id
        //        var product = prodDAL.GetProduct(cardData.ProductId.Value, true, auditUserId, auditWorkstation);

        //        if (product == null)
        //            throw new Exception(String.Format("Could not find product matching the supplied product id {0}.", cardData.ProductId ?? -1));

        //        log.Debug(d => d("{0} : Using product {1}{2}", deviceId, product.ProductCode, product.ProductName));

        //        DecryptedFields decryptedFields;                

        //        if (FieldDecryption(deviceId, device, cardData, deviceTPK, String.Empty, product, languageId, auditUserId, auditWorkstation, out decryptedFields, out responseMessage))                    
        //        {
        //            string pvv;

        //            // Calculate PVV
        //            if (PINCalculation(device, deviceId, product, decryptedFields, hsm, interfaceInfo, auditInfo, out responseMessage, out pvv))
        //            {
        //                //Now do something with the PVV/Pin/PinBlock
        //                return UpdatePvv(device.issuer_id, device.branch_id, product.ProductId, decryptedFields, pvv, languageId, auditUserId, auditWorkstation);
        //            }
        //            else
        //            {
        //                return new Response<PINResponse>(null, ResponseType.UNSUCCESSFUL, responseMessage, ((int)IsoResponseCodes.PVVFailed).ToString("00"));
        //            }
        //        }
        //    }
        //    catch (NotImplementedException nie)
        //    {
        //        log.Error(nie);
        //        responseMessage = "Method not implement.";
        //    }

        //    return new Response<PINResponse>(null, ResponseType.UNSUCCESSFUL, responseMessage, ((int)IsoResponseCodes.UnexpectedError).ToString("00"));            
        //}

        /// <summary>
        /// POS ISO8583 PIN Set and Reset
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="operatorCode"></param>
        /// <param name="cardData"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public TerminalResponse<byte[]> PINResetISO8583(string deviceId, string operatorCode, TerminalCardData cardData, long auditUserId, string auditWorkstation)
        {
            string responseMessage;

            //Get TPK for device for use when decrypting fields from terminal
            var deviceTPK = GetDeviceTPK(deviceId);

            TerminalDetails device;

            AuditInfo auditInfo = new AuditInfo
            {
                AuditUserId = auditUserId,
                AuditWorkStation = auditWorkstation,
                LanguageId = 0
            };

            //Fetch device info, also checks if user has access to use the device.
            if(!GetDevice(deviceId, auditInfo, out device, out responseMessage))
            {
                return TerminalResponse<byte[]>.Failed(responseMessage, (int)IsoResponseCodes.DeviceNotFoundOrAuthorised, null);
            }

            //Get ZMK for issuer
            //ZoneKeyResult zmk = GetZMK(deviceId, device.issuer_id, auditUserId, auditWorkstation);                     

            //Decrypt all the fields
            try
            {
                //Integration
                //IntegrationController _integration = IntegrationController.Instance;               

                //Veneka.Indigo.Integration.Config.IConfig config;

                //_integration.HardwareSecurityModule(device.issuer_id, InterfaceArea.ISSUING, out config);

                //InterfaceInfo interfaceInfo = new InterfaceInfo
                //{
                //    Config = config,
                //    InterfaceGuid = config.InterfaceGuid.ToString()
                //};

                //AuditInfo auditInfo = new AuditInfo
                //{
                //    AuditUserId = auditUserId,
                //    AuditWorkStation = auditWorkstation,
                //    LanguageId = 0
                //};


                //var hsm = COMSController.ComsCore;

                //var hsm = _integration.HardwareSecurityModule(device.issuer_id, InterfaceArea.ISSUING, out config);


                var hsm = IntegrationController.Instance.HardwareSecurityModule(device.HSMConfig, _dataSource);


                //ProductDAL prodDAL = new ProductDAL(Veneka.Indigo.Common.Database.DatabaseConnectionObject.Instance.SQLConnectionString);
                //var product = prodDAL.FindBestMatch(device.issuer_id, cardData.PAN, true, auditUserId, auditWorkstation);

                //Lookup card product
                var prodResp = _terminalManager.GetProduct(device.IssuerId, cardData.PAN, auditInfo);                

                if(!prodResp.ResponseEquals(ComsResponseCodes.SUCCESS))
                {
                    throw new Exception(prodResp.ResponseMessage);
                }

                if (prodResp.Value == null)
                    throw new Exception(String.Format("Could not find product matching the supplied card details {0}.", cardData.PAN.Substring(0, 6)));

                //log.Debug(d => d("{0} : Using product {1}{2}", deviceId, prodResp.Value.ProductCode, prodResp.Value.ProductName));                

                DecryptedFields decryptedFields;
                //if (hsm.DecryptFields(new TerminalDAL.ZoneMasterKey(zmk.zone, zmk.final), deviceTPK, product, cardData, operatorCode, config, 0, -2, auditWorkstation, out responseMessage, out decryptedFields))
                if(FieldDecryption(deviceId, device, cardData, deviceTPK, operatorCode, prodResp.Value, hsm, auditInfo, out decryptedFields, out responseMessage))
                {
                    if(String.IsNullOrWhiteSpace(decryptedFields.OperatorCode))
                    {
                        //log.WarnFormat("{0}: Failed to extract operator code.", deviceId);
                        return TerminalResponse<byte[]>.Failed("Failed to extract operator code", (int)IsoResponseCodes.UnexpectedError, null);
                        //return new Response<byte[]>(null, ResponseType.UNSUCCESSFUL, "Failed to extract operator code", ((int)IsoResponseCodes.UnexpectedError).ToString("00"));
                    }

                    //Validate operator code
                    //Check that a correct key has been sent
                    if (!_operatorKeys.Contains(decryptedFields.OperatorCode))
                    {
                        return TerminalResponse<byte[]>.Failed("Incorrect or expired operator key", (int)IsoResponseCodes.IncorrectOrExpiredOperatorKey, null);
                        //return new Response<byte[]>(null, ResponseType.UNSUCCESSFUL, "Incorrect or expired operator key", ((int)IsoResponseCodes.IncorrectOrExpiredOperatorKey).ToString("00"));
                    }

                    var operatorId = (long)_operatorKeys.Get(decryptedFields.OperatorCode);
                    _operatorKeys.Remove(decryptedFields.OperatorCode);
                    decryptedFields.OperatorCode = String.Empty;

                    auditInfo.AuditUserId = operatorId;
                                        

                    // Check if the parameters
                    //if (!GetTerminalParameters(0, operatorId, auditWorkstation, prodResp.Value, out TerminalParametersResult terminalParameters, out responseMessage))
                    //{
                    //    return TerminalResponse<byte[]>.Failed(responseMessage, (int)IsoResponseCodes.ProductNotFoundOrAuthorised, null);
                    //    //return new Response<byte[]>(null, ResponseType.UNSUCCESSFUL, responseMessage, ((int)IsoResponseCodes.ProductNotFoundOrAuthorised).ToString("00"));
                    //}

                    string pvv;

                    // Calculate PVV
                    // TODO: Fix Me!
                    //if (PINCalculation(device, deviceId, prodResp.Value, decryptedFields, hsm, auditInfo, out responseMessage, out pvv))
                    //{
                    //    //Now do something with the PVV/Pin/PinBlock                        
                    //    var resultUpdatePvv = UpdatePvv(device.IssuerId, device.BranchId, prodResp.Value.ProductId, decryptedFields, pvv, 0, operatorId, auditWorkstation);

                    //    if (resultUpdatePvv.ResponseType == ResponseType.SUCCESSFUL)
                    //    {                            
                    //        return new Response<byte[]>(resultUpdatePvv.Value.PinIndex, resultUpdatePvv.ResponseType, resultUpdatePvv.ResponseMessage, resultUpdatePvv.ResponseException);
                    //    }
                    //    else
                    //    {
                    //        return new Response<byte[]>(null, resultUpdatePvv.ResponseType, resultUpdatePvv.ResponseMessage, resultUpdatePvv.ResponseException);
                    //    }
                    //}
                    //else
                    //{
                    //    return new Response<byte[]>(null, ResponseType.UNSUCCESSFUL, responseMessage, ((int)IsoResponseCodes.PVVFailed).ToString("00"));
                    //}                    
                }
            }
            catch (NotImplementedException nie)
            {
                //log.Error(nie);
                responseMessage = "Method not implement.";
            }

            throw new NotImplementedException();
            //return new Response<byte[]>(null, ResponseType.UNSUCCESSFUL, responseMessage, ((int)IsoResponseCodes.UnexpectedError).ToString("00"));
        }
        #endregion


        private bool FieldDecryption(string deviceId, TerminalDetails device, TerminalCardData cardData, TerminalSessionKey deviceTPK, string operatorCode, Product product, IHardwareSecurityModule hsm, AuditInfo auditInfo, out DecryptedFields decryptedFields, out string responseMessage)
        {
            decryptedFields = null;
            responseMessage = String.Empty;

            //Get ZMK for issuer            
            //ZoneKeyResult zmk = GetZMK(deviceId, device.issuer_id, auditUserId, auditWorkstation);

            //Decrypt all the fields
            //IntegrationController _integration = IntegrationController.Instance;

            //Veneka.Indigo.Integration.Config.IConfig config;
            //_integration.HardwareSecurityModule(device.IssuerId, InterfaceArea.ISSUING, out config);
                        

            //if (hsm == null)
            //    log.Debug("HSM Interface is null!!!");

            //if (device.ZoneMasterKey == null)
            //    log.Debug("ZMK is null!!!!!");


            //log.Debug("deviceTPK" + deviceTPK);
            //log.Debug("product" + product);
            //log.Debug("cardData" + cardData);
            //log.Debug("operatorCode" + operatorCode);


            if (!hsm.DecryptFields(new ZoneMasterKey(device.ZoneMasterKey, device.ZoneMasterKey2), deviceTPK, product, cardData, operatorCode, device.HSMConfig.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out responseMessage, out decryptedFields))
                return false;

            if (decryptedFields == null)
                decryptedFields = new DecryptedFields();

            // If the hsm decrypt fields was called but track data not set then we check if we need to set from an unencrypted field
            if (decryptedFields.TrackData == null)
            {
                if (!String.IsNullOrWhiteSpace(cardData.Track2) && !cardData.IsTrack2Encrypted)
                {
                    decryptedFields.TrackData = new Track2(cardData.Track2);                    
                }
                else if (!String.IsNullOrWhiteSpace(cardData.PAN) && !cardData.IsPANEncrypted)
                {                    
                    decryptedFields.TrackData = new Track2(cardData.PAN, String.Empty, String.Empty, String.Empty, String.Empty);                    
                }
                else
                {
                    responseMessage = "No card data received.";
                    return false;
                }
            }

            if(String.IsNullOrWhiteSpace(decryptedFields.PinBlock) && !String.IsNullOrWhiteSpace(cardData.PINBlock))
            {
                decryptedFields.PinBlock = cardData.PINBlock;
            }

            return true;
        }

        //private bool GetTerminalParameters(int languageId, long auditUserId, string auditWorkstation, Product product, out TerminalParametersResult terminalParameters, out string responseMessage)
        //{
        //    terminalParameters = null;
        //    responseMessage = String.Empty;

            
        //    var terminal = _terminalManager.LoadParameters(product.ProductId, languageId, auditUserId, auditWorkstation);

        //    //Do some validations
        //    if (terminal == null)
        //    {
        //        responseMessage = "No card product for card found in system.";
        //        return false;
        //    }

        //    //if (!track.Value.PAN.StartsWith(terminal.product_bin_code.Trim()))
        //    //    return new Response<TerminalParametersResult>(null, ResponseType.UNSUCCESSFUL, "Card number does not match product in system.", "Card number does not match product in system.");

        //    if (terminal.issuer_status_id != 0)
        //    {
        //        responseMessage = "Issuer disabled, cannot complete action.";
        //        return false;
        //    }


        //    if (terminal.DeletedYN == true)
        //    {
        //        responseMessage = "Card product has been deleted, cannot complete action.";
        //        return false;
        //    }


        //    if (terminal.issuer_enable_instant_pin_YN && terminal.enable_instant_pin_reissue_YN)
        //    {
        //        terminalParameters = terminal;
        //        return true;
        //    }
        //    else
        //    {
        //        responseMessage = "Card product and Issuer not setup for instant PIN, cannot complete action.";
        //    }

        //    return false;
        //}

        private bool PINCalculation(TerminalDetails device, string deviceId, Product product, DecryptedFields decryptedFields, IHardwareSecurityModule hsm, AuditInfo auditInfo, out string responseMessage, out string pvv)
        {
            responseMessage = String.Empty;

            
            switch (product.PinCalcMethodId)
            {
                case 0: // VISA
                    //log.Trace(t => t("{0} : using VISA pvv method.", deviceId));
                    if (!hsm.GenerateVisaPVV(device.IssuerId, product, decryptedFields, deviceId, device.HSMConfig.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out responseMessage, out pvv))
                    {
                        return false;
                    }
                    break;
                case 1: // IBM
                    //log.Trace(t => t("{0} : using IBM pvv method.", deviceId));
                    if (!hsm.GenerateIBMPVV(device.IssuerId, product, decryptedFields, deviceId, device.HSMConfig.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out responseMessage, out pvv))
                    {
                        return false;
                    }
                    break;
                case 2: // Dont do any calculation
                    //log.Trace(t => t("{0} : using no calculation method. Passing PINBlock from terminal.", deviceId));
                    pvv = decryptedFields.PinBlock;
                    break;
                case 3: // Extract PIN or PIN Offset from PIN Block
                    //log.Trace(t => t("{0} : using extract PIN method.", deviceId));

                    if(!hsm.PinFromPinBlock(device.IssuerId, product, decryptedFields, deviceId, device.HSMConfig.Config, auditInfo.LanguageId, auditInfo.AuditUserId, auditInfo.AuditWorkStation, out responseMessage, out pvv))
                    {
                        return false;
                    }
 
                    break;
                default:
                    throw new Exception("Unknwon PIN Calculation Method: " + product.PinBlockFormatId);
            }

            return true;
        }

        private TerminalResponse<PINResponse> UpdatePvv(int issuerId, int branchId, int productId, DecryptedFields decryptedFields, string pvv, long operatorId, AuditInfo auditInfo)
        {
            byte[] id = new byte[16];
            rng.GetNonZeroBytes(id);
            string strId = Encoding.UTF8.GetString(id);
            byte[] encryptedId = EncryptionManager.EncryptData(id, "");
            DateTimeOffset expiry = DateTimeOffset.Now.AddMinutes(10.0);

            string responseMessage = String.Empty;

            //Create Reissue Request or update card record

            var pinReqResp = _pinReissue.RequestPINReissue(issuerId, branchId, productId, decryptedFields.TrackData.PAN, id, operatorId, auditInfo);

            if(pinReqResp.ResponseEquals(ComsResponseCodes.SUCCESS))
            {
                expiry = pinReqResp.Value.RequestExpiry;
            }
            else
            {
                return TerminalResponse<PINResponse>.Failed(pinReqResp.ResponseMessage, (int)IsoResponseCodes.IndigoRequestFailed, null);
            }


            //PinReissueResult req;
            //if (_cardManService.RequestPINReissue(issuerId, branchId, productId, decryptedFields.TrackData.PAN, id, operatorId, auditInfo))
            //    expiry = req.request_expiry.Value;
            //else
            //    return new Response<PINResponse>(null, ResponseType.UNSUCCESSFUL, responseMessage, ((int)IsoResponseCodes.IndigoRequestFailed).ToString("00"));            


            var encryptedPin = EncryptionManager.EncryptDataFromString(pvv, "");

            if (_secureCache.Contains(strId))
                _secureCache.Remove(strId);

            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = expiry;

            _secureCache.Add(strId, Tuple.Create(decryptedFields.TrackData, encryptedPin), policy);

            decryptedFields.TrackData = null;

            var resp = new PINResponse
            {
                PinIndex = encryptedId,
                //PinReissueId = req.pin_reissue_id
            };


            return TerminalResponse<PINResponse>.Failed(pinReqResp.ResponseMessage, (int)IsoResponseCodes.IndigoRequestFailed, null);
        }

        //private ZoneKeyResult GetZMK(string deviceId, int issuerId, long auditUserId, string auditWorkstation)
        //{
        //    var zmk = _terminalManager.GetZoneKey(issuerId, auditUserId, auditWorkstation);

        //    if (zmk == null)
        //        throw new ArgumentNullException(String.Format("Zone Key parameters not set for issuer {0}", issuerId));

        //    log.Debug(d => d("{0} : ZMK={1}", deviceId, zmk.zone));
        //    return zmk;
        //}

        private bool GetDevice(string deviceId, AuditInfo auditInfo, out TerminalDetails terminal, out string responseMessage)
        {
            responseMessage = String.Empty;
            terminal = null;

            var response = _terminalManager.GetDevice(deviceId, auditInfo);

            responseMessage = response.ResponseMessage;

            if (response.ResponseCode == (int)ComsResponseCodes.SUCCESS)
            {
                terminal = response.Value;

                if (terminal == null)
                {
                    responseMessage = String.Format("Terminal device {0} not found or not authorised to use terminal device.", deviceId);
                    return false;
                }

                return true;
            }

            return false;
        }

        private TerminalSessionKey GetDeviceTPK(string deviceId)
        {
            TerminalSessionKey keys = _secureSessionCache.GetCachedItem<TerminalSessionKey>(deviceId);

            if (keys == null)
                throw new Exception(String.Format("No terminal session key for device {0}.", deviceId));

            //log.Debug(d => d("{0} : Session TPK={1}/{2}", deviceId, keys.RandomKey, keys.RandomKeyUnderLMK));
            return keys;
        }

        #endregion

    }

    public sealed class PINResponse
    {
        public long? PinReissueId { get; set; }
        public byte[] PinIndex { get; set; }
    }
}