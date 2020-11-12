using System;
using Common.Logging;
using Veneka.Indigo.PINManagement;
using Veneka.Indigo.Common;
using IndigoCardIssuanceService.DataContracts;
using Veneka.Indigo.Common.Models;
using System.Collections.Generic;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Security;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.Common.Utilities;
using Veneka.Indigo.Integration.Common;
using System.Web.Caching;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.Caching;
using Veneka.Indigo.CardManagement;
using System.Linq;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.COMS.Core;
using IndigoCardIssuanceService.COMS;
using Veneka.Indigo.COMS.DataSource;
using Veneka.Indigo.Common.Models.IssuerManagement;

namespace IndigoCardIssuanceService.bll
{
    public class TerminalManagementController
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


        private static readonly ILog log = LogManager.GetLogger(typeof(TerminalManagementController));
        private readonly TerminalManagementService _terminalManager = new TerminalManagementService();
        private readonly LocalDataSource _dataSource = new LocalDataSource();
        private readonly CardMangementService _cardManService;
        private SessionManager _sessionManager = SessionManager.GetInstance();
        private static readonly Random rndOperatorKey = new Random();

        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        //private static readonly Cache m_secureCache = new Cache();
        private static readonly ObjectCache _secureCache = new MemoryCache("secureCache");
        private static readonly SecureCache _secureSessionCache = new SecureCache("secureSessionCache", new TimeSpan(12, 0, 0));

        private static readonly ObjectCache _operatorKeys = new MemoryCache("operatorKeys");

        public TerminalManagementController()
        {
            _cardManService = new CardMangementService(_dataSource);
        }


        #region EXPOSED METHODS

        internal static Tuple<Track2, byte[]> FetchIndex(byte[] index)
        {
            if (_secureCache.Contains(Encoding.UTF8.GetString(index)))
                return (Tuple<Track2, byte[]>)_secureCache.Get(Encoding.UTF8.GetString(index));

            return null;
        }

        internal static void Remove(byte[] index)
        {
            if (_secureCache.Contains(Encoding.UTF8.GetString(index)))
                _secureCache.Remove(Encoding.UTF8.GetString(index));
        }







        //internal Response<TerminalParametersResult> LoadParameters(int issuerId, string deviceId, bool reissue, string encryptedKeyUnderLMK, TerminalCardData cardData, int languageId, long auditUserId, string auditWorkstation)
        //{
        //    try
        //    {
        //        //Read the track data
        //        var track = this.ReadTrackData(deviceId, encryptedKeyUnderLMK, cardData, languageId, auditUserId, auditWorkstation, out issuerId);
        //        if(track.ResponseType != ResponseType.SUCCESSFUL)
        //            return new Response<TerminalParametersResult>(null, track.ResponseType, track.ResponseMessage, track.ResponseException);

        //        //TODO: If product ID is empty try and lookup the product



        //        //Validate track data
        //        if(track == null)
        //            return new Response<TerminalParametersResult>(null, ResponseType.UNSUCCESSFUL, "Could not read track data from card, please contact support.", "Could not read track data from card, please contact support.");

        //        if(String.IsNullOrWhiteSpace(track.Value.PAN))
        //            return new Response<TerminalParametersResult>(null, ResponseType.UNSUCCESSFUL, "Could not read PAN from track data on card, please contact support.", "Could not read PAN from track data on card, please contact support.");

        //        Veneka.Indigo.Integration.DAL.ProductDAL prodDAL = new Veneka.Indigo.Integration.DAL.ProductDAL(Veneka.Indigo.Common.Database.DatabaseConnectionObject.Instance.SQLConnectionString);
        //        var product = prodDAL.FindBestMatch(issuerId, track.Value.PAN, true, auditUserId, auditWorkstation);

        //        if (product == null || product.ProductId <= 0)
        //            return new Response<TerminalParametersResult>(null, ResponseType.UNSUCCESSFUL, "No card product for card found in system.", "No card product for card found in system.");

        //        //Fetch Terminal Parameters
        //        var terminal = _terminalManager.LoadParameters(product.ProductId, languageId, auditUserId, auditWorkstation);

        //        //Do some validations
        //        if(terminal == null)
        //            return new Response<TerminalParametersResult>(null, ResponseType.UNSUCCESSFUL, "No card product for card found in system.", "No card product for card found in system.");

        //        if (!track.Value.PAN.StartsWith(terminal.product_bin_code.Trim()))
        //            return new Response<TerminalParametersResult>(null, ResponseType.UNSUCCESSFUL, "Card number does not match product in system.", "Card number does not match product in system.");

        //        if(terminal.issuer_status_id != 0)
        //            return new Response<TerminalParametersResult>(null, ResponseType.UNSUCCESSFUL, "Issuer disabled, cannot complete action.", "Issuer disabled, cannot complete action.");

        //        if(terminal.DeletedYN == true)
        //            return new Response<TerminalParametersResult>(terminal, 
        //                                                            ResponseType.UNSUCCESSFUL, 
        //                                                            "Card product has been deleted, cannot complete action.", "Card product has been deleted, cannot complete action.");

        //        if (!reissue && !(terminal.issuer_enable_instant_pin_YN && terminal.product_enable_instant_pin_YN))
        //            return new Response<TerminalParametersResult>(terminal, 
        //                                                            ResponseType.UNSUCCESSFUL, 
        //                                                            "Card product and Issuer not setup for instant PIN, cannot complete action.", "Card product and Issuer not setup for instant PIN, cannot complete action.");

        //        if (reissue && !(terminal.issuer_enable_instant_pin_YN || terminal.enable_instant_pin_reissue_YN))
        //            return new Response<TerminalParametersResult>(terminal, 
        //                                                            ResponseType.UNSUCCESSFUL, 
        //                                                            "Card product and Issuer not setup for instant PIN reissue, cannot complete action.", "Card product and Issuer not setup for instant PIN reissue, cannot complete action.");

        //        return new Response<TerminalParametersResult>(terminal,
        //                                                        ResponseType.SUCCESSFUL,
        //                                                        "", "");
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex);
        //        return new Response<TerminalParametersResult>(null,
        //                                    ResponseType.ERROR,
        //                                    "Error when processing request.",
        //                                    log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
        //    }
        //}

        //private Response<Track2> ReadTrackData(string deviceId, string encryptedKeyUnderLMK, TerminalCardData cardData, int languageId, long auditUserId, string auditWorkstation, out int issuerId)
        //{
        //    issuerId = 0;

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

        //    string keyUnderLMK = EncryptionManager.DecryptString(encryptedKeyUnderLMK,
        //                                                        StaticFields.USE_HASHING_FOR_ENCRYPTION,
        //                                                        StaticFields.EXTERNAL_SECURITY_KEY); 

        //    //Fetch device info, also checks if user has access to use the device.
        //    var result = _terminalManager.GetTMKForTerminal(deviceId, auditUserId, auditWorkstation);

        //    if (result == null)
        //        throw new Exception("Terminal device not found or not authorised to use terminal device.");

        //    issuerId = result.issuer_id;

        //    IntegrationController _integration = IntegrationController.Instance;
        //    string responseMessage;
        //    Track2 track2;
        //    Veneka.Indigo.Integration.Config.IConfig config;

        //    try
        //    {
        //        if (_integration.HardwareSecurityModule(result.issuer_id, InterfaceArea.ISSUING, out config).ReadTrackData(result.issuer_id, keyUnderLMK, cardData, deviceId, config, languageId, auditUserId, auditWorkstation, out responseMessage, out track2))
        //        {
        //            //string encryptedPAN = EncryptionManager.EncryptString(track2.PAN,
        //            //                                        StaticFields.USE_HASHING_FOR_ENCRYPTION,
        //            //                                        StaticFields.EXTERNAL_SECURITY_KEY);

        //            return new Response<Track2>(track2, ResponseType.SUCCESSFUL, responseMessage, responseMessage);
        //        }
        //        else
        //        {
        //            return new Response<Track2>(null, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
        //        }

        //    }
        //    catch (NotImplementedException nie)
        //    {
        //        log.Warn("ReadTrackData() method in HSM module not implemented.", nie);

        //        responseMessage = "HSM module not implemented.";
        //        return new Response<Track2>(null, ResponseType.ERROR, responseMessage, nie.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex);
        //        return new Response<Track2>(null,
        //                                    ResponseType.ERROR,
        //                                    "Error when processing request.",
        //                                    log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
        //    }
        //}









        #endregion


        #region PIN SET GOOD


        #region Operator Session Key
        /// <summary>
        /// Generates a random key for the operator to use when doing PIN reset.
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<string> GetOperatorSessionKey(int languageId, long auditUserId, string auditWorkstation)
        {
            CacheItemPolicy policy = new CacheItemPolicy();

            //check if the operator has a key assigned to him/her, if he/she does remove it and assign new key
            foreach (var operatorKey in _operatorKeys.Where(w => (long)w.Value == auditUserId))
                _operatorKeys.Remove(operatorKey.Key);

            //100 tries to generate a random key not assigned to someone else.
            //The key is only 4 digits long therefore the more operators the more there might be clashes.
            for (int count = 0; count < 100; count++)
            {
                // Build a random key for the operator
                StringBuilder randomKey = new StringBuilder();

                for (int i = 0; i < 4; i++)
                    randomKey.Append(rndOperatorKey.Next(0, 9));

                //Make sure we dont end up with the same key
                if (!_operatorKeys.Contains(randomKey.ToString()))
                {
                    //Key cached for 15 minutes
                    policy.AbsoluteExpiration = DateTime.Now.AddMinutes(15);
                    _operatorKeys.Add(randomKey.ToString(), auditUserId, policy);

                    //Encrypt the keys before we send them out for additional layer of security
                    var encrypted = EncryptionManager.EncryptString(randomKey.ToString(),
                                                                            StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                            StaticFields.EXTERNAL_SECURITY_KEY);

                    return new Response<string>(encrypted, ResponseType.SUCCESSFUL, "", "");
                }
            }

            return new Response<string>(null, ResponseType.UNSUCCESSFUL, "Could not generate key, please try again.", "Could not generate key, please try again.");
        }
        #endregion

        #region Terminal Session Key
        /// <summary>
        /// Generates Session Key for the Terminal and encrypts it for transport over Indigo App API
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<TerminalSessionKey> GetTerminalSessionKey(string deviceId, int languageId, long auditUserId, string auditWorkstation)
        {
            var resp = this.GetTerminalSessionKeyClear(deviceId, languageId, auditUserId, auditWorkstation);

            if (resp.ResponseType == ResponseType.SUCCESSFUL)
            {
                //Encrypt the keys before we send them out for additional layer of security
                resp.Value.RandomKey = EncryptionManager.EncryptString(resp.Value.RandomKey,
                                                                                StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                                StaticFields.EXTERNAL_SECURITY_KEY);

                resp.Value.RandomKeyUnderLMK = EncryptionManager.EncryptString(resp.Value.RandomKeyUnderLMK,
                                                                                StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                                StaticFields.EXTERNAL_SECURITY_KEY);
            }

            return resp;
        }

        /// <summary>
        /// Generate the TPK for the terminal.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<TerminalSessionKey> GetTerminalSessionKeyClear(string deviceId, int languageId, long auditUserId, string auditWorkstation)
        {
            IntegrationController _integration = IntegrationController.Instance;
            string responseMessage;
            //TerminalSessionKey randomKeys;

            ////Fetch device info, also checks if user has access to use the device.
            //var result = _terminalManager.GetTMKForTerminal(deviceId, auditUserId, auditWorkstation);
            
            //Fetch device info, also checks if user has access to use the device.
            TerminalTMKResult device;
            if (!GetDevice(deviceId, auditUserId, auditWorkstation, out  device, out responseMessage))
            {
                return new Response<TerminalSessionKey>(null, ResponseType.UNSUCCESSFUL, responseMessage, ((int)IsoResponseCodes.DeviceNotFoundOrAuthorised).ToString("00"));
            }

            try
            {
                Veneka.Indigo.Integration.Config.IConfig config;
                _integration.HardwareSecurityModule(device.issuer_id, InterfaceArea.ISSUING, out config);

                InterfaceInfo interfaceInfo = new InterfaceInfo
                {
                    Config = config,
                    InterfaceGuid = config.InterfaceGuid.ToString()
                };

                AuditInfo auditInfo = new AuditInfo
                {
                    AuditUserId = auditUserId,
                    AuditWorkStation = auditWorkstation,
                    LanguageId = languageId
                };


                var response = COMSController.ComsCore.GenerateRandomKey(device.issuer_id, device.masterkey, deviceId, interfaceInfo, auditInfo);

                //if (_integration.HardwareSecurityModule(device.issuer_id, InterfaceArea.ISSUING, out config).GenerateRandomKey(device.issuer_id, device.masterkey, deviceId, config, languageId, auditUserId, auditWorkstation, out responseMessage, out randomKeys))
                if(response.ResponseCode == 0)
                {                    
                    _secureSessionCache.SetCacheItem<TerminalSessionKey>(deviceId, response.Value);
                    return new Response<TerminalSessionKey>(response.Value, ResponseType.SUCCESSFUL, response.ResponseMessage, ((int)IsoResponseCodes.Success).ToString("00"));
                }
                else
                {
                    log.Error(responseMessage);
                    return new Response<TerminalSessionKey>(null, ResponseType.UNSUCCESSFUL, response.ResponseMessage, ((int)IsoResponseCodes.TPKGenerationFailed).ToString("00"));
                }

            }
            catch (NotImplementedException nie)
            {
                log.Warn(deviceId + " : GetTerminalSessionKey() method in HSM module not implemented.", nie);

                responseMessage = "HSM module not implemented.";
                return new Response<TerminalSessionKey>(null, ResponseType.ERROR, responseMessage, ((int)IsoResponseCodes.NotImplemented).ToString("00"));
            }
            catch (Exception ex)
            {
                log.Error(deviceId + " : Unexpected error.", ex);
                return new Response<TerminalSessionKey>(null,
                                                        ResponseType.ERROR,
                                                        "Error when processing request.",
                                                        ((int)IsoResponseCodes.UnexpectedError).ToString("00"));
            }
        }
        #endregion

        #region Fetch Product Parameters
        internal Response<TerminalParametersResult> GetProductParameters(int issuerId, string deviceId, TerminalCardData cardData, int languageId, long auditUserId, string auditWorkstation)
        {
            if (!String.IsNullOrWhiteSpace(cardData.PAN))
            {
                cardData.PAN = EncryptionManager.DecryptString(cardData.PAN,
                                                                    StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                    StaticFields.EXTERNAL_SECURITY_KEY);
            }

            if (!String.IsNullOrWhiteSpace(cardData.Track2))
            {
                cardData.Track2 = EncryptionManager.DecryptString(cardData.Track2,
                                                                    StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                    StaticFields.EXTERNAL_SECURITY_KEY);
            }

            string responseMessage;
            DecryptedFields decryptedFields;

            try
            {
                //Get TPK for device for use when decrypting fields from terminal
                var deviceTPK = GetDeviceTPK(deviceId);
                //Fetch device info, also checks if user has access to use the device.
                if (!GetDevice(deviceId, auditUserId, auditWorkstation, out TerminalTMKResult device, out responseMessage))
                {
                    return new Response<TerminalParametersResult>(null, ResponseType.UNSUCCESSFUL, responseMessage, ((int)IsoResponseCodes.DeviceNotFoundOrAuthorised).ToString("00"));
                }


                IProductDAL prodDAL = _dataSource.ProductDAL;
                Product product = null;

                // Search for the product, if PAN is in the clear and supplied
                if (!cardData.IsPANEncrypted && !String.IsNullOrWhiteSpace(cardData.PAN))
                {
                    product = prodDAL.FindBestMatch(device.issuer_id, cardData.PAN, true, auditUserId, auditWorkstation);

                    if (product == null)
                        log.Warn("Product not found");
                }                

                if (!FieldDecryption(deviceId, device, cardData, deviceTPK, String.Empty, product, languageId, auditUserId, auditWorkstation, out decryptedFields, out responseMessage))
                    return new Response<TerminalParametersResult>(null, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);

                if (product == null)
                {
                    product = prodDAL.FindBestMatch(device.issuer_id, decryptedFields.TrackData.PAN, true, auditUserId, auditWorkstation);
                }

                if (product == null || product.ProductId <= 0)
                    return new Response<TerminalParametersResult>(null, ResponseType.UNSUCCESSFUL, "No card product for card found in system.", "No card product for card found in system.");
                                
                // Fetch Terminal Parameters
                if(GetTerminalParameters(languageId, auditUserId, auditWorkstation, product, out TerminalParametersResult terminalParameters, out responseMessage))
                {
                    return new Response<TerminalParametersResult>(terminalParameters, ResponseType.SUCCESSFUL, responseMessage, responseMessage);
                }

                return new Response<TerminalParametersResult>(null, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
            }
            catch (NotImplementedException nie)
            {
                log.Error(nie);
                return new Response<TerminalParametersResult>(null,
                                                                ResponseType.ERROR,
                                                                "HSM not implemented.",
                                                                log.IsDebugEnabled || log.IsTraceEnabled ? nie.ToString() : "");                
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<TerminalParametersResult>(null,
                                                                ResponseType.ERROR,
                                                                "Error when processing request.",
                                                                log.IsDebugEnabled || log.IsTraceEnabled ? ex.ToString() : "");
            }
        }

        
        #endregion

        #region PIN Reset
        /// <summary>
        /// PinPad App implementation
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="encryptedTpkUnderLMK"></param>
        /// <param name="encryptedPinBlock"></param>
        /// <param name="encryptedTrackData"></param>
        /// <param name="cardId"></param>
        /// <param name="branchId"></param>
        /// <param name="productId"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        internal Response<PINResponse> TerminalPINReset(string deviceId, TerminalCardData cardData, int languageId, long auditUserId, string auditWorkstation)
        {
            if (!String.IsNullOrWhiteSpace(cardData.PAN))
            {
                cardData.PAN = EncryptionManager.DecryptString(cardData.PAN,
                                                                    StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                    StaticFields.EXTERNAL_SECURITY_KEY);
            }

            if (!String.IsNullOrWhiteSpace(cardData.Track2))
            {
                cardData.Track2 = EncryptionManager.DecryptString(cardData.Track2,
                                                                    StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                    StaticFields.EXTERNAL_SECURITY_KEY);
            }

            if (!String.IsNullOrWhiteSpace(cardData.PINBlock))
            {
                cardData.PINBlock = EncryptionManager.DecryptString(cardData.PINBlock,
                                                                    StaticFields.USE_HASHING_FOR_ENCRYPTION,
                                                                    StaticFields.EXTERNAL_SECURITY_KEY);
            }

            string responseMessage;

            //Get TPK for device for use when decrypting fields from terminal
            var deviceTPK = GetDeviceTPK(deviceId);
            //Fetch device info, also checks if user has access to use the device.
            if (!GetDevice(deviceId, auditUserId, auditWorkstation, out TerminalTMKResult device, out responseMessage))
            {
                return new Response<PINResponse>(null, ResponseType.UNSUCCESSFUL, responseMessage, ((int)IsoResponseCodes.DeviceNotFoundOrAuthorised).ToString("00"));
            }            

            //Decrypt all the fields
            try
            {
                //Integration
                IntegrationController _integration = IntegrationController.Instance;

                Veneka.Indigo.Integration.Config.IConfig config;
                _integration.HardwareSecurityModule(device.issuer_id, InterfaceArea.ISSUING, out config);

                InterfaceInfo interfaceInfo = new InterfaceInfo
                {
                    Config = config,
                    InterfaceGuid = config.InterfaceGuid.ToString()
                };

                AuditInfo auditInfo = new AuditInfo
                {
                    AuditUserId = auditUserId,
                    AuditWorkStation = auditWorkstation,
                    LanguageId = languageId
                };


                var hsm = COMSController.ComsCore;

                //Lookup card product
                IProductDAL prodDAL = _dataSource.ProductDAL;

                //Get product details from product id
                var product = prodDAL.GetProduct(cardData.ProductId.Value, true, auditUserId, auditWorkstation);

                if (product == null)
                    throw new Exception(String.Format("Could not find product matching the supplied product id {0}.", cardData.ProductId ?? -1));

                log.Debug(d => d("{0} : Using product {1}{2}", deviceId, product.ProductCode, product.ProductName));

                DecryptedFields decryptedFields;                

                if (FieldDecryption(deviceId, device, cardData, deviceTPK, String.Empty, product, languageId, auditUserId, auditWorkstation, out decryptedFields, out responseMessage))                    
                {
                    string pvv;

                    // Calculate PVV
                    if (PINCalculation(device, deviceId, product, decryptedFields, hsm, interfaceInfo, auditInfo, out responseMessage, out pvv))
                    {
                        //Now do something with the PVV/Pin/PinBlock
                        return UpdatePvv(device.issuer_id, device.branch_id, product.ProductId, decryptedFields, pvv, languageId, auditUserId, auditWorkstation);
                    }
                    else
                    {
                        return new Response<PINResponse>(null, ResponseType.UNSUCCESSFUL, responseMessage, ((int)IsoResponseCodes.PVVFailed).ToString("00"));
                    }
                }
            }
            catch (NotImplementedException nie)
            {
                log.Error(nie);
                responseMessage = "Method not implement.";
            }

            return new Response<PINResponse>(null, ResponseType.UNSUCCESSFUL, responseMessage, ((int)IsoResponseCodes.UnexpectedError).ToString("00"));            
        }

        /// <summary>
        /// POS ISO8583 PIN Set and Reset
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="operatorCode"></param>
        /// <param name="cardData"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public Response<byte[]> PINResetISO8583(string deviceId, string operatorCode, TerminalCardData cardData, long auditUserId, string auditWorkstation)
        {
            string responseMessage;

            //Get TPK for device for use when decrypting fields from terminal
            var deviceTPK = GetDeviceTPK(deviceId);                   

            //Fetch device info, also checks if user has access to use the device.
            if(!GetDevice(deviceId, auditUserId, auditWorkstation, out TerminalTMKResult device, out responseMessage))
            {
                return new Response<byte[]>(null, ResponseType.UNSUCCESSFUL, responseMessage, ((int)IsoResponseCodes.DeviceNotFoundOrAuthorised).ToString("00"));
            }

            //Get ZMK for issuer
            //ZoneKeyResult zmk = GetZMK(deviceId, device.issuer_id, auditUserId, auditWorkstation);                     

            //Decrypt all the fields
            try
            {
                //Integration
                IntegrationController _integration = IntegrationController.Instance;               

                Veneka.Indigo.Integration.Config.IConfig config;

                _integration.HardwareSecurityModule(device.issuer_id, InterfaceArea.ISSUING, out config);

                InterfaceInfo interfaceInfo = new InterfaceInfo
                {
                    Config = config,
                    InterfaceGuid = config.InterfaceGuid.ToString()
                };

                AuditInfo auditInfo = new AuditInfo
                {
                    AuditUserId = auditUserId,
                    AuditWorkStation = auditWorkstation,
                    LanguageId = 0
                };


                var hsm = COMSController.ComsCore;

                //var hsm = _integration.HardwareSecurityModule(device.issuer_id, InterfaceArea.ISSUING, out config);

                //Lookup card product
                IProductDAL prodDAL = _dataSource.ProductDAL;
                var product = prodDAL.FindBestMatch(device.issuer_id, cardData.PAN, true, auditUserId, auditWorkstation);

                if (product == null)
                    throw new Exception(String.Format("Could not find product matching the supplied card details {0}.", cardData.PAN.Substring(0, 6)));

                log.Debug(d => d("{0} : Using product {1}{2}", deviceId, product.ProductCode, product.ProductName));                

                DecryptedFields decryptedFields;
                //if (hsm.DecryptFields(new TerminalDAL.ZoneMasterKey(zmk.zone, zmk.final), deviceTPK, product, cardData, operatorCode, config, 0, -2, auditWorkstation, out responseMessage, out decryptedFields))
                if(FieldDecryption(deviceId, device, cardData, deviceTPK, operatorCode, product, 0, -2, auditWorkstation, out decryptedFields, out responseMessage))
                {
                    if(String.IsNullOrWhiteSpace(decryptedFields.OperatorCode))
                    {
                        log.WarnFormat("{0}: Failed to extract operator code.", deviceId);
                        return new Response<byte[]>(null, ResponseType.UNSUCCESSFUL, "Failed to extract operator code", ((int)IsoResponseCodes.UnexpectedError).ToString("00"));
                    }

                    //Validate operator code
                    //Check that a correct key has been sent
                    if (!_operatorKeys.Contains(decryptedFields.OperatorCode))
                        return new Response<byte[]>(null, ResponseType.UNSUCCESSFUL, "Incorrect or expired operator key", ((int)IsoResponseCodes.IncorrectOrExpiredOperatorKey).ToString("00"));

                    var operatorId = (long)_operatorKeys.Get(decryptedFields.OperatorCode);
                    _operatorKeys.Remove(decryptedFields.OperatorCode);
                    decryptedFields.OperatorCode = String.Empty;

                    // Check if the parameters
                    if (!GetTerminalParameters(0, operatorId, auditWorkstation, product, out TerminalParametersResult terminalParameters, out responseMessage))
                    {
                        return new Response<byte[]>(null, ResponseType.UNSUCCESSFUL, responseMessage, ((int)IsoResponseCodes.ProductNotFoundOrAuthorised).ToString("00"));
                    }

                    string pvv;
                   
                    // Calculate PVV
                    if (PINCalculation(device, deviceId, product, decryptedFields, hsm, interfaceInfo, auditInfo, out responseMessage, out pvv))
                    {
                        //Now do something with the PVV/Pin/PinBlock                        
                        var resultUpdatePvv = UpdatePvv(device.issuer_id, device.branch_id, product.ProductId, decryptedFields, pvv, 0, operatorId, auditWorkstation);

                        if (resultUpdatePvv.ResponseType == ResponseType.SUCCESSFUL)
                        {
                            return new Response<byte[]>(resultUpdatePvv.Value.PinIndex, resultUpdatePvv.ResponseType, resultUpdatePvv.ResponseMessage, resultUpdatePvv.ResponseException);
                        }
                        else
                        {
                            return new Response<byte[]>(null, resultUpdatePvv.ResponseType, resultUpdatePvv.ResponseMessage, resultUpdatePvv.ResponseException);
                        }
                    }
                    else
                    {
                        return new Response<byte[]>(null, ResponseType.UNSUCCESSFUL, responseMessage, ((int)IsoResponseCodes.PVVFailed).ToString("00"));
                    }                    
                }
            }
            catch (NotImplementedException nie)
            {
                log.Error(nie);
                responseMessage = "Method not implement.";
            }

            return new Response<byte[]>(null, ResponseType.UNSUCCESSFUL, responseMessage, ((int)IsoResponseCodes.UnexpectedError).ToString("00"));
        }
        #endregion

        public Response<DecryptedFields> FieldDecryptionWithZoneMaster(int issuer_id, ZoneMasterKey zmk ,  TerminalCardData cardData, TerminalSessionKey deviceTPK, string operatorCode, Product product, int languageId, long auditUserId, string auditWorkstation)
        {
          //  decryptedFields = null;
           // responseMessage = String.Empty;

            //Get ZMK for issuer

            //Decrypt all the fields
            IntegrationController _integration = IntegrationController.Instance;

            Veneka.Indigo.Integration.Config.IConfig config;
            _integration.HardwareSecurityModule(issuer_id, InterfaceArea.ISSUING, out config);

            InterfaceInfo interfaceInfo = new InterfaceInfo
            {
                Config = config,
                InterfaceGuid = config.InterfaceGuid.ToString()
            };

            AuditInfo auditInfo = new AuditInfo
            {
                AuditUserId = auditUserId,
                AuditWorkStation = auditWorkstation,
                LanguageId = 0
            };


            var hsm = COMSController.ComsCore;



            if (hsm == null)
                log.Debug("HSM Interface is null!!!");

            if (zmk == null)
                log.Debug("ZMK is null!!!!!");


            log.Debug("deviceTPK" + deviceTPK);
            log.Debug("product" + product);
            log.Debug("cardData" + cardData);
            log.Debug("operatorCode" + operatorCode);

            var decrptResp = hsm.DecryptFields(zmk, deviceTPK, product, cardData, operatorCode, interfaceInfo, auditInfo);

            if (decrptResp.ResponseCode != 0)
            return new Response<DecryptedFields>(null, ResponseType.UNSUCCESSFUL, decrptResp.ResponseMessage, String.Empty);

            var decryptedFields = decrptResp.Value;

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
                   var  responseMessage = "No card data received.";
                    return new Response<DecryptedFields>(null, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
                }
            }

            if (String.IsNullOrWhiteSpace(decryptedFields.PinBlock) && !String.IsNullOrWhiteSpace(cardData.PINBlock))
            {
                decryptedFields.PinBlock = cardData.PINBlock;
            }

            return new Response<DecryptedFields>(decryptedFields, ResponseType.SUCCESSFUL, decrptResp.ResponseMessage, "");
        }

        public Response<DecryptedFields> FieldDecryptionWithZoneMasterWithMessaging(int issuer_id, ZoneMasterKey zmk, TerminalCardData cardData, TerminalSessionKey deviceTPK, string operatorCode, Product product, CustomerDetailsResult customer, Messaging message_params, int languageId, long auditUserId, string auditWorkstation)
        {
            //  decryptedFields = null;
            // responseMessage = String.Empty;

            //Get ZMK for issuer

            //Decrypt all the fields
            IntegrationController _integration = IntegrationController.Instance;

            Veneka.Indigo.Integration.Config.IConfig config;
            _integration.HardwareSecurityModule(issuer_id, InterfaceArea.ISSUING, out config);

            InterfaceInfo interfaceInfo = new InterfaceInfo
            {
                Config = config,
                InterfaceGuid = config.InterfaceGuid.ToString()
            };

            AuditInfo auditInfo = new AuditInfo
            {
                AuditUserId = auditUserId,
                AuditWorkStation = auditWorkstation,
                LanguageId = 0
            };


            var hsm = COMSController.ComsCore;



            if (hsm == null)
                log.Debug("HSM Interface is null!!!");

            if (zmk == null)
                log.Debug("ZMK is null!!!!!");


            log.Debug("deviceTPK" + deviceTPK);
            log.Debug("product" + product);
            log.Debug("cardData" + cardData);
            log.Debug("operatorCode" + operatorCode);

            var decrptResp = hsm.DecryptFieldsWithMessaging(zmk, deviceTPK, product, cardData, operatorCode, customer, message_params, interfaceInfo, auditInfo);

            if (decrptResp.ResponseCode != 0)
                return new Response<DecryptedFields>(null, ResponseType.UNSUCCESSFUL, decrptResp.ResponseMessage, String.Empty);

            var decryptedFields = decrptResp.Value;

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
                    var responseMessage = "No card data received.";
                    return new Response<DecryptedFields>(null, ResponseType.UNSUCCESSFUL, responseMessage, responseMessage);
                }
            }

            if (String.IsNullOrWhiteSpace(decryptedFields.PinBlock) && !String.IsNullOrWhiteSpace(cardData.PINBlock))
            {
                decryptedFields.PinBlock = cardData.PINBlock;
            }

            return new Response<DecryptedFields>(decryptedFields, ResponseType.SUCCESSFUL, decrptResp.ResponseMessage, "");
        }

        private bool FieldDecryption(string deviceId, TerminalTMKResult device, TerminalCardData cardData, TerminalSessionKey deviceTPK, string operatorCode, Product product, int languageId, long auditUserId, string auditWorkstation, out DecryptedFields decryptedFields, out string responseMessage)
        {
            decryptedFields = null;
            responseMessage = String.Empty;

            //Get ZMK for issuer
            ZoneKeyResult zmk = GetZMK(deviceId, device.issuer_id, auditUserId, auditWorkstation);

            //Decrypt all the fields
            IntegrationController _integration = IntegrationController.Instance;

            Veneka.Indigo.Integration.Config.IConfig config;
            _integration.HardwareSecurityModule(device.issuer_id, InterfaceArea.ISSUING, out config);

            InterfaceInfo interfaceInfo = new InterfaceInfo
            {
                Config = config,
                InterfaceGuid = config.InterfaceGuid.ToString()
            };

            AuditInfo auditInfo = new AuditInfo
            {
                AuditUserId = auditUserId,
                AuditWorkStation = auditWorkstation,
                LanguageId = 0
            };


            var hsm = COMSController.ComsCore;



            if (hsm == null)
                log.Debug("HSM Interface is null!!!");

            if (zmk == null)
                log.Debug("ZMK is null!!!!!");


            log.Debug("deviceTPK" + deviceTPK);
            log.Debug("product" + product);
            log.Debug("cardData" + cardData);
            log.Debug("operatorCode" + operatorCode);

            var decrptResp = hsm.DecryptFields(new ZoneMasterKey(zmk.zone, zmk.final), deviceTPK, product, cardData, operatorCode, interfaceInfo, auditInfo);

            responseMessage = decrptResp.ResponseMessage;

            if (decrptResp.ResponseCode != 0)
                return false;

            decryptedFields = decrptResp.Value;

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

        private bool GetTerminalParameters(int languageId, long auditUserId, string auditWorkstation, Product product, out TerminalParametersResult terminalParameters, out string responseMessage)
        {
            terminalParameters = null;
            responseMessage = String.Empty;

            var terminal = _terminalManager.LoadParameters(product.ProductId, languageId, auditUserId, auditWorkstation);

            //Do some validations
            if (terminal == null)
            {
                responseMessage = "No card product for card found in system.";
                return false;
            }

            //if (!track.Value.PAN.StartsWith(terminal.product_bin_code.Trim()))
            //    return new Response<TerminalParametersResult>(null, ResponseType.UNSUCCESSFUL, "Card number does not match product in system.", "Card number does not match product in system.");

            if (terminal.issuer_status_id != 0)
            {
                responseMessage = "Issuer disabled, cannot complete action.";
                return false;
            }


            if (terminal.DeletedYN == true)
            {
                responseMessage = "Card product has been deleted, cannot complete action.";
                return false;
            }


            if (terminal.issuer_enable_instant_pin_YN && terminal.enable_instant_pin_reissue_YN)
            {
                terminalParameters = terminal;
                return true;
            }
            else
            {
                responseMessage = "Card product and Issuer not setup for instant PIN, cannot complete action.";
            }

            return false;
        }

        private bool PINCalculation(TerminalTMKResult device, string deviceId, Product product, DecryptedFields decryptedFields, IComsCore hsm, InterfaceInfo interfaceInfo, AuditInfo auditInfo, out string responseMessage, out string pvv)
        {
            responseMessage = String.Empty;

            
            switch (product.PinCalcMethodId)
            {
                case 0: // VISA
                    log.Trace(t => t("{0} : using VISA pvv method.", deviceId));
                    var visaPvvresp = hsm.GenerateVisaPVV(device.issuer_id, product, decryptedFields, deviceId, interfaceInfo, auditInfo);

                    responseMessage = visaPvvresp.ResponseMessage;
                    pvv = visaPvvresp.Value;

                    if (visaPvvresp.ResponseCode != 0)
                        return false;

                    //if (!hsm.GenerateVisaPVV(device.issuer_id, product, decryptedFields, deviceId, config, languageId, auditUserId, auditWorkstation, out responseMessage, out pvv))
                    //    return false;
                    break;
                case 1: // IBM
                    log.Trace(t => t("{0} : using IBM pvv method.", deviceId));

                    var ibmPvvResp = hsm.GenerateIBMPVV(device.issuer_id, product, decryptedFields, deviceId, interfaceInfo, auditInfo);

                    responseMessage = ibmPvvResp.ResponseMessage;
                    pvv = ibmPvvResp.Value;

                    if (ibmPvvResp.ResponseCode != 0)
                        return false;

                    //if (!hsm.GenerateIBMPVV(device.issuer_id, product, decryptedFields, deviceId, config, languageId, auditUserId, auditWorkstation, out responseMessage, out pvv))
                    //    return false;
                    break;
                case 2: // Dont do any calculation
                    log.Trace(t => t("{0} : using no calculation method. Passing PINBlock from terminal.", deviceId));
                    pvv = decryptedFields.PinBlock;
                    break;
                case 3: // Extract PIN or PIN Offset from PIN Block
                    log.Trace(t => t("{0} : using extract PIN method.", deviceId));

                    var custPinResp = hsm.PinFromPinBlock(device.issuer_id, product, decryptedFields, deviceId, interfaceInfo, auditInfo);

                    responseMessage = custPinResp.ResponseMessage;
                    pvv = custPinResp.Value;

                    if (custPinResp.ResponseCode != 0)
                        return false;

                    //if (!hsm.PinFromPinBlock(device.issuer_id, product, decryptedFields, deviceId, config, languageId, auditUserId, auditWorkstation, out responseMessage, out pvv))
                    //    return false;
                    break;
                default:
                    throw new Exception("Unknwon PIN Calculation Method: " + product.PinBlockFormatId);
            }

            return true;
        }

        private Response<PINResponse> UpdatePvv(int issuerId, int branchId, int productId, DecryptedFields decryptedFields, string pvv, int languageId, long operatorId, string auditWorkstation)
        {
            byte[] id = new byte[16];
            rng.GetNonZeroBytes(id);
            string strId = Encoding.UTF8.GetString(id);
            byte[] encryptedId = EncryptionManager.EncryptData(id, StaticFields.PIN_SECURITY_KEY);
            DateTimeOffset expiry = DateTimeOffset.Now.AddMinutes(10.0);

            string responseMessage = String.Empty;

            //Create Reissue Request or update card record
            PinReissueResult req;
            if (_cardManService.RequestPINReissue(issuerId, branchId, productId, decryptedFields.TrackData.PAN, encryptedId, null, 0, languageId, operatorId, auditWorkstation, out req, out responseMessage))
                expiry = req.request_expiry.Value;
            else
                return new Response<PINResponse>(null, ResponseType.UNSUCCESSFUL, responseMessage, ((int)IsoResponseCodes.IndigoRequestFailed).ToString("00"));            


            var encryptedPin = EncryptionManager.EncryptDataFromString(pvv, StaticFields.PIN_SECURITY_KEY);

            if (_secureCache.Contains(strId))
                _secureCache.Remove(strId);

            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = expiry;

            _secureCache.Add(strId, Tuple.Create(decryptedFields.TrackData, encryptedPin), policy);

            decryptedFields.TrackData = null;

            var resp = new PINResponse
            {
                PinIndex = encryptedId,
                PinReissueId = req.pin_reissue_id
            };

            return new Response<PINResponse>(resp, ResponseType.SUCCESSFUL, responseMessage, ((int)IsoResponseCodes.Success).ToString("00"));
        }

        private ZoneKeyResult GetZMK(string deviceId, int issuerId, long auditUserId, string auditWorkstation)
        {
            var zmk = _terminalManager.GetZoneKey(issuerId, auditUserId, auditWorkstation);

            if (zmk == null)
                throw new ArgumentNullException(String.Format("Zone Key parameters not set for issuer {0}", issuerId));

            log.Debug(d => d("{0} : ZMK={1}", deviceId, zmk.zone));
            return zmk;
        }

        private bool GetDevice(string deviceId, long auditUserId, string auditWorkstation, out TerminalTMKResult terminal, out string responseMessage)
        {
            responseMessage = String.Empty;
            terminal = null;

            terminal = _terminalManager.GetTMKForTerminal(deviceId, auditUserId, auditWorkstation);

            if (terminal == null)
            {
                responseMessage = String.Format("Terminal device {0} not found or not authorised to use terminal device.", deviceId);
                return false;
            }

            return true;
        }

        private TerminalSessionKey GetDeviceTPK(string deviceId)
        {
            TerminalSessionKey keys = _secureSessionCache.GetCachedItem<TerminalSessionKey>(deviceId);

            if (keys == null)
                throw new Exception(String.Format("No terminal session key for device {0}.", deviceId));

            log.Debug(d => d("{0} : Session TPK={1}/{2}", deviceId, keys.RandomKey, keys.RandomKeyUnderLMK));
            return keys;
        }

        #endregion

        #region Terminal Management
        internal Response<int> CreateTerminal(string TerminalName, string TerminalModel, string DeviceId,
             int BranchId, int TerminalMasterkeyId, string password, bool IsMacUsed, int LanguageId, long AuditUserId, string AuditWorkstation)
        {
            try
            {
                string responseMessage = ""; int Terminalid = 0;
                if (_terminalManager.CreateTerminal(TerminalName, TerminalModel, DeviceId, BranchId, TerminalMasterkeyId, password, IsMacUsed,
                AuditUserId,AuditWorkstation, LanguageId, out Terminalid, out responseMessage))
                {
                    return new Response<int>(Terminalid, ResponseType.SUCCESSFUL, responseMessage, "");
            }
                return new Response<int>(0, ResponseType.UNSUCCESSFUL, responseMessage, "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<int>(0, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal BaseResponse UpdateTerminal(int TerminalId, string TerminalName, string TerminalModel, string DeviceId,
            int BranchId, int TerminalMasterkeyId, string password, bool IsMacUsed, int LanguageId, long AuditUserId, string AuditWorkstation)
        {
            try
            {
                string responseMessage = "";

                if (_terminalManager.UpdateTerminal(TerminalId, TerminalName, TerminalModel, DeviceId, BranchId, TerminalMasterkeyId, password, IsMacUsed, AuditUserId, AuditWorkstation, LanguageId, out responseMessage))
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

        internal BaseResponse DeleteTerminal(int TerminalId, int LanguageId, long AuditUserId, string AuditWorkstation)
        {
            try
            {
                string responseMessage = "";

                if (_terminalManager.DeleteTerminal(TerminalId,AuditUserId, AuditWorkstation, LanguageId, out responseMessage))
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

        internal BaseResponse DeleteMasterkey(int MasterkeyId, int LanguageId, long AuditUserId, string AuditWorkstation)
        {
            try
            {
                string responseMessage = "";

                if (_terminalManager.DeleteMasterkey(MasterkeyId,  AuditUserId, AuditWorkstation, LanguageId, out responseMessage))
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

        internal Response<TerminalResult> GetTerminals(int TerminalId)
        {
            try
            {
                return new Response<TerminalResult>(_terminalManager.GetTerminals(TerminalId),
                                                   ResponseType.SUCCESSFUL,
                                                   "",
                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<TerminalResult>(null,
                                                  ResponseType.ERROR,
                                                  "Error processing request, please try again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<MasterkeyResult> GetMasterkey(int MasterkeyId)
        {
            try
            {
                return new Response<MasterkeyResult>(_terminalManager.GetMasterkey(MasterkeyId),
                                                   ResponseType.SUCCESSFUL,
                                                   "",
                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<MasterkeyResult>(null,
                                                  ResponseType.ERROR,
                                                  "Error processing request, please try again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<TerminalListResult>> GetTerminalsList(int? IssuerId,int? BranchId, int LanguageId, int PageIndex, int RowsPerPage)
        {
            try
            {
                return new Response<List<TerminalListResult>>(_terminalManager.GetTerminalsList(IssuerId,BranchId, LanguageId, PageIndex, RowsPerPage),
                                                   ResponseType.SUCCESSFUL,
                                                   "",
                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<TerminalListResult>>(null,
                                                  ResponseType.ERROR,
                                                  "Error processing request, please try again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<TerminalListResult>> SearchTerminals(int? IssuerId, int? BranchId, string terminalname, string deviceid, string terminalmodel, int PageIndex, int RowsPerPage)
        {
            try
            {
                return new Response<List<TerminalListResult>>(_terminalManager.SearchTerminals(IssuerId, BranchId, terminalname, deviceid, terminalmodel,  PageIndex,  RowsPerPage),
                                                   ResponseType.SUCCESSFUL,
                                                   "",
                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<TerminalListResult>>(null,
                                                  ResponseType.ERROR,
                                                  "Error processing request, please try again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<List<TerminalTMKIssuerResult>> GetTMKByIssuer(int IssuerId, int PageIndex,int rowsperpage,long AuditUserId, string AuditWorkstation)
        {
            try
            {
                return new Response<List<TerminalTMKIssuerResult>>(_terminalManager.GetTMKByIssuer(IssuerId,PageIndex,rowsperpage, AuditUserId, AuditWorkstation),
                                                   ResponseType.SUCCESSFUL,
                                                   "",
                                                   "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<TerminalTMKIssuerResult>>(null,
                                                  ResponseType.ERROR,
                                                  "Error processing request, please try again.",
                                                  log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal Response<int> CreateMasterkey(string Masterkey, string MasterkeyName, int IssuerId, long AuditUserId,string AuditWorkstation,int LanguageId)
        {
            try
            {

                string responseMessage = ""; int masterkeyid = 0;
                if (_terminalManager.CreateMasterkey(Masterkey, MasterkeyName, IssuerId, AuditUserId, AuditWorkstation, LanguageId, out masterkeyid, out responseMessage))
                {
                    return new Response<int>(masterkeyid, ResponseType.SUCCESSFUL, responseMessage, "");
                }
                return new Response<int>(masterkeyid, ResponseType.UNSUCCESSFUL, responseMessage, "");

              
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<int>(0, ResponseType.ERROR,
                                        "Error processing request, please try again.",
                                        log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        internal BaseResponse UpdateMasterkey(int MasterkeyId, string Masterkey, string MasterkeyName, int IssuerId, long AuditUserId, string AuditWorkstation, int languageId)
        {
            try
            {
                string responseMessage = "";
                if (_terminalManager.UpdateMasterkey(MasterkeyId, Masterkey, MasterkeyName, IssuerId, AuditUserId, AuditWorkstation, languageId,out responseMessage))
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
        
        #endregion
    }

    public sealed class PINResponse
    {
        public long? PinReissueId { get; set; }
        public byte[] PinIndex { get; set; }
    }
}