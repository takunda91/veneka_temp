using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using IndigoCardIssuanceService.DataContracts;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Integration;
using System.IO;
using Common.Logging;
using IndigoCardIssuanceService;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using Veneka.Indigo.Integration.Remote;
using IndigoCardIssuanceService.COMS;
using Veneka.Indigo.COMS.DataSource;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.External;

namespace Veneka.Indigo.Integration
{
    /// <summary>
    /// Controller provices method for getting the correct integration layer to be used.
    /// Integration layers are lazy loaded and are referenced by their GUID's.
    /// </summary>
    public class IntegrationController : IIntegrationController
    {
        private const string PRODUCT_MISSING_INTERFACE = "{0} interface not configured for product, please check product setup.";
        private const string ISSUER_MISSING_INTERFACE = "{0} interface not configured for issuer, please check issuer setup.";
        private const string MISSING_INTEGRATION_DLL = "Could not find integration library with GUID={0}. Make sure that the integration .dll is present and restart the application.";

        private static readonly ILog log = LogManager.GetLogger(typeof(IntegrationController));
        private static readonly IssuerManagement.IssuerService _issuerService = new IssuerManagement.IssuerService();
        private static readonly CardManagement.CardMangementService _productService = new CardManagement.CardMangementService(new LocalDataSource());
        private readonly Config.ConfigDAL _configDAL;
        private readonly External.ExternalSystemDAL _externalSystemDal;

        private static volatile IntegrationController instance;

        private static object syncRoot = new Object();

        public static bool OnUploadEventSubscribed { get; set; }

        public static IntegrationController Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new IntegrationController();


                    }
                }

                return instance;
            }
        }

        #region MEF Imports

        [ImportMany]
        private Lazy<ICardManagementSystem, IIntegrationCapabilities>[] iCardManagementSystems;

        //[ImportMany]
        private Lazy<ICardProductionSystem, IIntegrationCapabilities>[] iCardProductionSystems;

        //[ImportMany]
        //private Lazy<IHardwareSecurityModule, IIntegrationCapabilities>[] iHardwareSecurityModules;

        //[ImportMany]
        private Lazy<ICoreBankingSystem, IIntegrationCapabilities>[] iCoreBankingSystems;

        //[ImportMany]
        private Lazy<ICardFileProcessor, IIntegrationCapabilities>[] iCardFileProcessors;

        //[ImportMany]
        private Lazy<INotificationSystem, IIntegrationCapabilities>[] iNotificationSystems;

        //[ImportMany]
        //private Lazy<IFeeScheme, IIntegrationCapabilities>[] iFeeSchemes;

        //[ImportMany]
        private Lazy<IExternalAuthentication, IIntegrationCapabilities>[] iExternalAuthentication;

        private Lazy<IMutiFactorAuthentication, IIntegrationCapabilities>[] iMultiFactorAuthentication;


        private Lazy<IRemoteCMS, IIntegrationCapabilities>[] iRemoteCMS;

        private Lazy<IPrepaidAccountProcessor, IIntegrationCapabilities>[] IPrepaidAccountSystems;

        #endregion

        #region Private Constructor

        private IntegrationController()
        {
            try
            {
                _configDAL = new Config.ConfigDAL(DatabaseConnectionObject.Instance.SQLConnectionString);
                _externalSystemDal = new External.ExternalSystemDAL(DatabaseConnectionObject.Instance.SQLConnectionString);

                //DirectoryCatalog catalog = new DirectoryCatalog(Path.Combine(SystemConfiguration.Instance.GetBaseConfigDir(), @"integration"), "*.dll");
                //var container = new CompositionContainer(catalog);
                //container.SatisfyImportsOnce(this);

                //use MEF to find and load all indigo integration dll's.
                AggregateCatalog catalog = GetIntegrationCatalog();

                var container = new CompositionContainer(catalog);
                container.ComposeParts(container);

                //Setup lookupdir for integration dll's, may be needed in the integration
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.AssemblyResolve += new ResolveEventHandler(IntegrationResolveEventHandler);

                //Load up CMS
                //var cmsImports = container.GetExports<ICardManagementSystem, IIntegrationCapabilities>();
                //iCardManagementSystems = iCardManagementSystems.ToArray();

                //Load up CPS
                //var cpsImports = container.GetExports<ICardProductionSystem, IIntegrationCapabilities>();
                //iCardProductionSystems = cpsImports.ToArray();

                //Load up HSM
                //var hsmImports = container.GetExports<IHardwareSecurityModule, IIntegrationCapabilities>();
                //iHardwareSecurityModules = hsmImports.ToArray();

                //Load up CBS
                //var cbsImports = container.GetExports<ICoreBankingSystem, IIntegrationCapabilities>();
                //iCoreBankingSystems = cbsImports.ToArray();

                //Load up FileProcessor
                //var cfpImports = container.GetExports<ICardFileProcessor, IIntegrationCapabilities>();
                //iCardFileProcessors = cfpImports.ToArray();

                //var externalImports = container.GetExports<IExternalAuthentication, IIntegrationCapabilities>();
                //iExternalAuthentication = externalImports.ToArray();


                //var authenticationImports = container.GetExports<IMutiFactorAuthentication, IIntegrationCapabilities>();
                //iMultiFactorAuthentication = authenticationImports.ToArray();

                //var remoteCMSExports = container.GetExports<IRemoteCMS, IIntegrationCapabilities>();
                //iRemoteCMS = remoteCMSExports.ToArray();


                //var notificationsystems = container.GetExports<INotificationSystem, IIntegrationCapabilities>();
                //iNotificationSystems = notificationsystems.ToArray();
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }

                log.Error(sb.ToString());
                throw new Exception(sb.ToString());
            }
        }

        /// <summary>
        /// Finds the latest integration dll's for this version of indigo
        /// </summary>
        /// <returns></returns>
        private static AggregateCatalog GetIntegrationCatalog()
        {
            AggregateCatalog catalog = new AggregateCatalog();
            //catalog.Catalogs.Add(new DirectoryCatalog(Path.Combine(SystemConfiguration.Instance.GetBaseConfigDir(), @"integration"), "*.dll"));

            DirectoryInfo dirInfo = new DirectoryInfo(Path.Combine(SystemConfiguration.Instance.GetBaseConfigDir(), @"integration"));

            if (!dirInfo.Exists)
            {
                log.WarnFormat("Directory {0} does not exist, cannot load integrations.", dirInfo.FullName);
                return catalog;
            }

            //Only find files that are integrationdll's for indigo            
            var fileVersionInfos = dirInfo
                                    .GetFiles("Veneka.Indigo.Integration.*.dll", SearchOption.TopDirectoryOnly)
                                    .Select(s => FileVersionInfo.GetVersionInfo(s.FullName));

            //Get version info of Indigo app
            var currentIndigoVersion = FileVersionInfo.GetVersionInfo(typeof(IntegrationController).Assembly.Location);

            //Group dll's by name then find the one that matches indigo product version and then the latest file version
            foreach (var groups in fileVersionInfos.GroupBy(g => g.OriginalFilename))
            {
                FileVersionInfo latestFile = null;

                foreach (var fileVersionInfo in groups)
                {
                    //does dll have same product version as indigo
                    if (fileVersionInfo.ProductVersion.Equals(currentIndigoVersion.ProductVersion))
                    {
                        //find out if this is the latest file version of the integration dll 
                        if ((latestFile == null) ||
                            (Version.Parse(fileVersionInfo.FileVersion).CompareTo(Version.Parse(latestFile.FileVersion)) == 1))
                        {
                            latestFile = fileVersionInfo;
                        }
                    }
                }

                if (latestFile != null)
                    catalog.Catalogs.Add(new AssemblyCatalog(latestFile.FileName));
            }

            return catalog;
        }

        #endregion

        #region AppDomain
        private Assembly IntegrationResolveEventHandler(object sender, ResolveEventArgs args)
        {
            //This handler is called only when the common language runtime tries to bind to the assembly and fails.

            //Retrieve the list of referenced assemblies in an array of AssemblyName.
            //Assembly MyAssembly, objExecutingAssembly;
            string strTempAssmbPath = "";

            //objExecutingAssembly = Assembly.GetExecutingAssembly();
            //AssemblyName[] arrReferencedAssmbNames = objExecutingAssembly.GetReferencedAssemblies();

            //Loop through the array of referenced assembly names.
            //foreach (AssemblyName strAssmbName in arrReferencedAssmbNames)
            //{
            //    //Check for the assembly names that have raised the "AssemblyResolve" event.
            //    if (strAssmbName.FullName.Substring(0, strAssmbName.FullName.IndexOf(",")) == args.Name.Substring(0, args.Name.IndexOf(",")))
            //    {
            //        //Build the path of the assembly from where it has to be loaded.                
            //        strTempAssmbPath = Path.Combine(SystemConfiguration.Instance.GetBaseConfigDir(), @"integration") + args.Name.Substring(0, args.Name.IndexOf(",")) + ".dll";
            //        break;
            //    }
            //}

            strTempAssmbPath = Path.Combine(SystemConfiguration.Instance.GetBaseConfigDir(), @"integration\") + args.Name.Substring(0, args.Name.IndexOf(",")) + ".dll";
            //Load the assembly from the specified path.                    
            var MyAssembly = Assembly.LoadFrom(strTempAssmbPath);

            //Return the loaded assembly.
            return MyAssembly;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Lists all the available integration interfaces found in the Indigo integration folder.
        /// </summary>
        /// <returns></returns>
        internal Response<List<IntegrationInterface>> GetIntegrationInterfaces()
        {
            try
            {
                return new Response<List<IntegrationInterface>>(ListIntegrationInterfaces(),
                                                                ResponseType.SUCCESSFUL,
                                                                "",
                                                                "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<IntegrationInterface>>(null,
                                                                ResponseType.ERROR,
                                                                "Error when processing request.",
                                                                log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }


        /// <summary>
        /// Lists all the available integration interfaces found in the Indigo integration folder.
        /// </summary>
        /// <returns></returns>
        internal Response<List<IntegrationInterface>> GetIntegrationInterfacesbyInterfaceid(int interfacetypeid)
        {
            try
            {
                return new Response<List<IntegrationInterface>>(ListIntegrationInterfaces().Where(i => i.InterfaceTypeId == interfacetypeid).ToList(),
                                                                ResponseType.SUCCESSFUL,
                                                                "",
                                                                "");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Response<List<IntegrationInterface>>(null,
                                                                ResponseType.ERROR,
                                                                "Error when processing request.",
                                                                log.IsDebugEnabled || log.IsTraceEnabled ? ex.Message : "");
            }
        }

        /// <summary>
        /// Returns an instance of the card management system integration object with the specified <see cref="System.Guid"/>
        /// </summary>
        /// <param name="guid">
        /// The <see cref="System.Guid"/> of the integration layer, this may not be an empty string or <see langword="null"/>.
        /// </param>
        /// <returns></returns>
        public void CardManagementSystem(int productId, InterfaceArea interfaceArea, out External.ExternalSystemFields externalFields, out Config.IConfig config)
        {
            config = _configDAL.GetProductInterfaceConfig(productId,
                                                            (int)InterfaceTypes.CARD_MANAGEMENT_SYSTEM,
                                                            (int)interfaceArea,
                                                            null,
                                                            SystemConfiguration.SYSTEM_USER_ID,
                                                            SystemConfiguration.SYSTEM_WORKSTATION);

            if (config == null)
                throw new Exception(String.Format(PRODUCT_MISSING_INTERFACE, InterfaceTypes.CARD_MANAGEMENT_SYSTEM));

            externalFields = _externalSystemDal.GetExternalSystemFields((int)External.ExternalSystemType.CardManagementSystem,
                                                        productId, 0, SystemConfiguration.SYSTEM_USER_ID,
                                                        SystemConfiguration.SYSTEM_WORKSTATION);


            //var cmsInterface = iCardManagementSystems.Where(w => Guid.Parse(w.Metadata.IntegrationGUID) == interfaceConfig.InterfaceGuid).SingleOrDefault();

            //if (cmsInterface != null && cmsInterface.Value != null)
            //{
            //    externalFields = _externalSystemDal.GetExternalSystemFields((int)External.ExternalSystemType.CardManagementSystem,
            //                                                productId, 0, SystemConfiguration.SYSTEM_USER_ID,
            //                                                SystemConfiguration.SYSTEM_WORKSTATION);

            //    cmsInterface.Value.SQLConnectionString = DatabaseConnectionObject.Instance.SQLConnectionString;
            //    config = interfaceConfig;
            //    return cmsInterface.Value;
            //}

            //throw new Exception(String.Format(MISSING_INTEGRATION_DLL, interfaceConfig.InterfaceGuid));
        }


        /// <summary>
        /// Returns an instance of the card production system integration object with the specified <see cref="System.Guid"/>
        /// </summary>
        /// <param name="guid">
        /// The <see cref="System.Guid"/> of the integration layer, this may not be an empty string or <see langword="null"/>.
        /// </param>
        /// <returns></returns>
        public ICardProductionSystem CardProductionSystem(int productId, InterfaceArea interfaceArea, out External.ExternalSystemFields externalFields, out Config.IConfig config)
        {
            var interfaceConfig = _configDAL.GetProductInterfaceConfig(productId,
                                                            (int)InterfaceTypes.CARD_PRODUCTION,
                                                            (int)interfaceArea,
                                                            null,
                                                            SystemConfiguration.SYSTEM_USER_ID,
                                                            SystemConfiguration.SYSTEM_WORKSTATION);

            if (interfaceConfig == null)
                throw new Exception(String.Format(PRODUCT_MISSING_INTERFACE, InterfaceTypes.CARD_PRODUCTION));

            var cpsInterface = iCardProductionSystems.Where(w => Guid.Parse(w.Metadata.IntegrationGUID) == interfaceConfig.InterfaceGuid).SingleOrDefault();

            if (cpsInterface != null && cpsInterface.Value != null)
            {
                externalFields = _externalSystemDal.GetExternalSystemFields((int)External.ExternalSystemType.CardProductionSystem,
                                                            productId, 0, SystemConfiguration.SYSTEM_USER_ID,
                                                            SystemConfiguration.SYSTEM_WORKSTATION);

                cpsInterface.Value.DataSource = new LocalDataSource(); // = DatabaseConnectionObject.Instance.SQLConnectionString;
                config = interfaceConfig;
                return cpsInterface.Value;
            }

            throw new Exception(String.Format(MISSING_INTEGRATION_DLL, interfaceConfig.InterfaceGuid));
        }

        /// <summary>
        /// Returns an instance of the Hardware Security Module integration object with the specified <see cref="System.Guid"/>
        /// </summary>
        /// <param name="guid">
        /// The <see cref="System.Guid"/> of the integration layer, this may not be an empty string or <see langword="null"/>.
        /// </param>
        /// <returns></returns>
        public void HardwareSecurityModule(int issuerId, InterfaceArea interfaceArea, out Config.IConfig config)
        {
            config = _configDAL.GetIssuerInterfaceConfig(issuerId,
                                                            (int)InterfaceTypes.HARDWARE_SECURITY_MODULE,
                                                            (int)interfaceArea,
                                                            null,
                                                            SystemConfiguration.SYSTEM_USER_ID,
                                                            SystemConfiguration.SYSTEM_WORKSTATION);

            if (config == null)
                throw new Exception(String.Format(ISSUER_MISSING_INTERFACE, InterfaceTypes.HARDWARE_SECURITY_MODULE));

            //var hsmInterface = iHardwareSecurityModules.Where(w => Guid.Parse(w.Metadata.IntegrationGUID) == interfaceConfig.InterfaceGuid).SingleOrDefault();

            //if (hsmInterface != null && hsmInterface.Value != null)
            //{
            //    hsmInterface.Value.SQLConnectionString = DatabaseConnectionObject.Instance.SQLConnectionString;
            //    config = interfaceConfig;
            //    return hsmInterface.Value;
            //}

            //throw new Exception(String.Format(MISSING_INTEGRATION_DLL, interfaceConfig.InterfaceGuid));
        }

        /// <summary>
        /// Returns an instance of the Core Banking System integration object with the specified <see cref="System.Guid"/>
        /// </summary>
        /// <param name="issuerId">
        /// The issuer id used to to do a lookup for the which integration layer to use.
        /// </param>
        /// <returns></returns>
        public void CoreBankingSystem(int productId, InterfaceArea interfaceArea, out External.ExternalSystemFields externalFields, out Config.IConfig config)
        {
            config = _configDAL.GetProductInterfaceConfig(productId,
                                                            (int)InterfaceTypes.CORE_BANKING_SYSTEM,
                                                            (int)interfaceArea,
                                                            null,
                                                            SystemConfiguration.SYSTEM_USER_ID,
                                                            SystemConfiguration.SYSTEM_WORKSTATION);



            if (config == null)
                throw new Exception(String.Format(PRODUCT_MISSING_INTERFACE, InterfaceTypes.CORE_BANKING_SYSTEM));

            //string guid = issuerInterfaces.First().interface_guid;
            //Guid guidOut = ValidateGuidArgument(guid);

            externalFields = _externalSystemDal.GetExternalSystemFields((int)External.ExternalSystemType.CoreBankingSystem,
                                                            productId, 0, SystemConfiguration.SYSTEM_USER_ID,
                                                            SystemConfiguration.SYSTEM_WORKSTATION);



            //  throw new Exception(String.Format(MISSING_INTEGRATION_DLL, interfaceConfig.InterfaceGuid));
        }

        public void FundsLoadCoreBankingSystem(int productId, out External.ExternalSystemFields externalFields, out Config.IConfig config)
        {
            config = _configDAL.GetProductInterfaceConfig(productId,
                                                            (int)InterfaceTypes.PREPAID_FUNDS_LOAD,
                                                            (int)InterfaceArea.PRODUCTION,
                                                            null,
                                                            SystemConfiguration.SYSTEM_USER_ID,
                                                            SystemConfiguration.SYSTEM_WORKSTATION);



            if (config == null)
                throw new Exception(String.Format(PRODUCT_MISSING_INTERFACE, InterfaceTypes.PREPAID_FUNDS_LOAD));

            externalFields = _externalSystemDal.GetExternalSystemFields((int)External.ExternalSystemType.CoreBankingSystem,
                                                           productId, 0, SystemConfiguration.SYSTEM_USER_ID,
                                                           SystemConfiguration.SYSTEM_WORKSTATION);
        }

        public void FundsLoadPrepaidSystem(int productId, out External.ExternalSystemFields externalFields, out Config.IConfig config)
        {
            config = _configDAL.GetProductInterfaceConfig(productId,
                                                            (int)InterfaceTypes.PREPAID_FUNDS_LOAD,
                                                            (int)InterfaceArea.ISSUING,
                                                            null,
                                                            SystemConfiguration.SYSTEM_USER_ID,
                                                            SystemConfiguration.SYSTEM_WORKSTATION);



            if (config == null)
                throw new Exception(String.Format(PRODUCT_MISSING_INTERFACE, InterfaceTypes.PREPAID_FUNDS_LOAD));

        }

        public void FeeSchemeInterface(int productId, InterfaceArea interfaceArea, out External.ExternalSystemFields externalFields, out Config.IConfig config)
        {
            config = _configDAL.GetProductInterfaceConfig(productId,
                                                            (int)InterfaceTypes.FEE_SCHEME,
                                                            (int)interfaceArea,
                                                            null,
                                                            SystemConfiguration.SYSTEM_USER_ID,
                                                            SystemConfiguration.SYSTEM_WORKSTATION);



            if (config == null)
                log.Debug("Fee Scheme is not Configured.");



            externalFields = _externalSystemDal.GetExternalSystemFields((int)External.ExternalSystemType.CoreBankingSystem,
                                                            productId, 0, SystemConfiguration.SYSTEM_USER_ID,
                                                            SystemConfiguration.SYSTEM_WORKSTATION);



        }

        public IExternalAuthentication ExternalAuthentication(string guid)
        {
            var cbsInterface = iExternalAuthentication.Where(w => w.Metadata.IntegrationGUID == guid).SingleOrDefault();

            if (cbsInterface != null && cbsInterface.Value != null)
            {
                cbsInterface.Value.DataSource = new LocalDataSource();  // = DatabaseConnectionObject.Instance.SQLConnectionString;
                return cbsInterface.Value;
            }
            throw new Exception(String.Format(MISSING_INTEGRATION_DLL, guid));

        }
        #endregion

        #region Private Methods

        /// <summary>
        /// List all the available interfaces found in the integration folder for Indigo
        /// </summary>
        /// <returns></returns>
        private List<IntegrationInterface> ListIntegrationInterfaces()
        {
            List<IntegrationInterface> availableInterfaces = new List<IntegrationInterface>();

            var cardmangementsystem = COMSController.ComsCore.GetIntegrationInterfacesbyInterfaceid((int)InterfaceTypes.CARD_MANAGEMENT_SYSTEM);
            foreach (var item in cardmangementsystem.Value)
            {
                availableInterfaces.Add(new IntegrationInterface
                {
                    IntegrationName = item.IntegrationName,
                    IntegrationGUID = item.IntegrationGUID,
                    InterfaceTypeId = (int)InterfaceTypes.CARD_MANAGEMENT_SYSTEM
                });
            }

            var cardproductionSystem = COMSController.ComsCore.GetIntegrationInterfacesbyInterfaceid((int)InterfaceTypes.CARD_PRODUCTION);
            foreach (var item in cardproductionSystem.Value)
            {
                availableInterfaces.Add(new IntegrationInterface
                {
                    IntegrationName = item.IntegrationName,
                    IntegrationGUID = item.IntegrationGUID,
                    InterfaceTypeId = (int)InterfaceTypes.CARD_PRODUCTION
                });
            }
            var corebankingsystem = COMSController.ComsCore.GetIntegrationInterfacesbyInterfaceid((int)InterfaceTypes.CORE_BANKING_SYSTEM);

            foreach (var item in corebankingsystem.Value)
            {
                availableInterfaces.Add(new IntegrationInterface
                {
                    IntegrationName = item.IntegrationName,
                    IntegrationGUID = item.IntegrationGUID,
                    InterfaceTypeId = (int)InterfaceTypes.CORE_BANKING_SYSTEM
                });
            }
            var ThreedSecureFileGenerator = COMSController.ComsCore.GetIntegrationInterfacesbyInterfaceid((int)InterfaceTypes.THREED_SECURE);
            foreach (var item in ThreedSecureFileGenerator.Value)
            {
                availableInterfaces.Add(new IntegrationInterface
                {
                    IntegrationName = item.IntegrationName,
                    IntegrationGUID = item.IntegrationGUID,
                    InterfaceTypeId = (int)InterfaceTypes.THREED_SECURE
                });
            }

            //foreach (var item in iHardwareSecurityModules)
            //{
            //    availableInterfaces.Add(new IntegrationInterface
            //    {
            //        IntegrationName = item.Metadata.IntegrationName,
            //        IntegrationGUID = item.Metadata.IntegrationGUID,
            //        InterfaceTypeId = (int)InterfaceTypes.HARDWARE_SECURITY_MODULE
            //    });
            //}

            var CardFileProcessors = COMSController.ComsCore.GetIntegrationInterfacesbyInterfaceid((int)InterfaceTypes.FILE_LOADER);

            foreach (var item in CardFileProcessors.Value)
            {
                availableInterfaces.Add(new IntegrationInterface
                {
                    IntegrationName = item.IntegrationName,
                    IntegrationGUID = item.IntegrationGUID,
                    InterfaceTypeId = (int)InterfaceTypes.FILE_LOADER
                });
            }

            foreach (var item in corebankingsystem.Value)
            {
                availableInterfaces.Add(new IntegrationInterface
                {
                    IntegrationName = item.IntegrationName,
                    IntegrationGUID = item.IntegrationGUID,
                    InterfaceTypeId = (int)InterfaceTypes.FEE_SCHEME
                });
            }
            var ExternalAuthentication = COMSController.ComsCore.GetIntegrationInterfacesbyInterfaceid((int)InterfaceTypes.EXTERNAL_AUTHENTICATION);

            foreach (var item in ExternalAuthentication.Value)
            {
                availableInterfaces.Add(new IntegrationInterface
                {
                    IntegrationName = item.IntegrationName,
                    IntegrationGUID = item.IntegrationGUID,
                    InterfaceTypeId = (int)InterfaceTypes.EXTERNAL_AUTHENTICATION
                });
            }
            var RemoteCMS = COMSController.ComsCore.GetIntegrationInterfacesbyInterfaceid((int)InterfaceTypes.REMOTE_CMS);

            foreach (var item in RemoteCMS.Value)
            {
                availableInterfaces.Add(new IntegrationInterface
                {
                    IntegrationName = item.IntegrationName,
                    IntegrationGUID = item.IntegrationGUID,
                    InterfaceTypeId = (int)InterfaceTypes.REMOTE_CMS
                });
            }
            var NotificationSystems = COMSController.ComsCore.GetIntegrationInterfacesbyInterfaceid((int)InterfaceTypes.NOTIFICATIONS);

            foreach (var item in NotificationSystems.Value)
            {
                availableInterfaces.Add(new IntegrationInterface
                {
                    IntegrationName = item.IntegrationName,
                    IntegrationGUID = item.IntegrationGUID,
                    InterfaceTypeId = (int)InterfaceTypes.NOTIFICATIONS
                });
            }
            var MultiFactorAuthentication = COMSController.ComsCore.GetIntegrationInterfacesbyInterfaceid((int)InterfaceTypes.MULTIFACTOR);

            foreach (var item in MultiFactorAuthentication.Value)
            {
                availableInterfaces.Add(new IntegrationInterface
                {
                    IntegrationName = item.IntegrationName,
                    IntegrationGUID = item.IntegrationGUID,
                    InterfaceTypeId = (int)InterfaceTypes.MULTIFACTOR
                });
            }

            var prePaidInterfaces = COMSController.ComsCore.GetIntegrationInterfacesbyInterfaceid((int)InterfaceTypes.PREPAID_FUNDS_LOAD);
            foreach (var item in prePaidInterfaces.Value)
            {
                availableInterfaces.Add(new IntegrationInterface
                {
                    IntegrationName = item.IntegrationName,
                    IntegrationGUID = item.IntegrationGUID,
                    InterfaceTypeId = (int)InterfaceTypes.PREPAID_FUNDS_LOAD
                });
            }

            var hsmInterface = COMSController.ComsCore.GetIntegrationInterfacesbyInterfaceid((int)InterfaceTypes.HARDWARE_SECURITY_MODULE);
            foreach (var item in hsmInterface.Value)
            {
                availableInterfaces.Add(new IntegrationInterface
                {
                    IntegrationName = item.IntegrationName,
                    IntegrationGUID = item.IntegrationGUID,
                    InterfaceTypeId = (int)InterfaceTypes.HARDWARE_SECURITY_MODULE
                });
            }

            return availableInterfaces;
        }

        /// <summary>
        /// This method validates that the string passed in is a valid Guid.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        private Guid ValidateGuidArgument(string guid)
        {
            if (String.IsNullOrWhiteSpace(guid))
                throw new ArgumentNullException("The guid argument cannot be null or empty.");

            Guid guidOut;
            if (!Guid.TryParse(guid, out guidOut))
                throw new ArgumentException("Value of guid argument is not a valid GUID.");

            return guidOut;
        }





        #endregion

        #region Structs

        public struct IntegrationInterface
        {
            public string IntegrationName { get; set; }
            public string IntegrationGUID { get; set; }
            public int InterfaceTypeId { get; set; }
        }

        #endregion
    }
}