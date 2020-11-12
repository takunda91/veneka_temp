using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using Veneka.Indigo.Common;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.Remote;
using System.IO.Compression;
using Common.Logging;

namespace Veneka.Indigo.COMS.Core.Integration
{
    public class IntegrationController : IIntegrationController
    {
        private const string MISSING_INTEGRATION_DLL = "Could not find integration library with GUID={0}. Make sure that the integration .dll is present and restart the application.";


        private static volatile IntegrationController instance;
        private static object syncRoot = new Object();

        //private static readonly ILog log = LogManager.GetLogger(typeof(IntegrationController));
        //private readonly Config _configDAL;
        //private readonly Veneka.Indigo.Integration.External.ExternalSystemDAL _externalSystemDal;

        public static IntegrationController Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new IntegrationController();
                        }
                    }
                }

                return instance;
            }
        }


        #region MEF Exports

        public Lazy<ICardManagementSystem, IIntegrationCapabilities>[] iCardManagementSystems;
        private Lazy<ICardProductionSystem, IIntegrationCapabilities>[] iCardProductionSystems;
        private Lazy<IHardwareSecurityModule, IIntegrationCapabilities>[] iHardwareSecurityModules;
        public Lazy<ICoreBankingSystem, IIntegrationCapabilities>[] iCoreBankingSystems;
        private Lazy<ICardFileProcessor, IIntegrationCapabilities>[] iCardFileProcessors;
        private Lazy<INotificationSystem, IIntegrationCapabilities>[] iNotificationSystems;
        private Lazy<I3DSecureRegistration, IIntegrationCapabilities>[] iThreedSecureFileGenerator;
        private Lazy<IPrepaidAccountProcessor, IIntegrationCapabilities>[] iPrepaidAccountProcessors;

        //[ImportMany]
        //private Lazy<IFeeScheme, IIntegrationCapabilities>[] iFeeSchemes;

        //[ImportMany]
        private Lazy<IExternalAuthentication, IIntegrationCapabilities>[] iExternalAuthentication;

        private Lazy<IMutiFactorAuthentication, IIntegrationCapabilities>[] iMultiFactorAuthentication;


        private Lazy<IRemoteCMS, IIntegrationCapabilities>[] iRemoteCMS;
        #endregion

        #region AppDomain
        private Assembly IntegrationResolveEventHandler(object sender, ResolveEventArgs args)
        {
            //This handler is called only when the common language runtime tries to bind to the assembly and fails.

            //Retrieve the list of referenced assemblies in an array of AssemblyName.
            string strTempAssmbPath = GetIntegrationDir();

            strTempAssmbPath += args.Name.Substring(0, args.Name.IndexOf(",")) + ".dll";
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
        public ComsResponse<List<IntegrationInterface>> GetIntegrationInterfaces()
        {
            try
            {
                return ComsResponse<List<IntegrationInterface>>.Success(string.Empty, ListIntegrationInterfaces());
            }
            catch (Exception ex)
            {
                //log.Error(ex);
                return ComsResponse<List<IntegrationInterface>>.Failed(ex.Message, null);
            }
        }

        /// <summary>
        /// Lists all the available integration interfaces found in the Indigo integration folder.
        /// </summary>
        /// <returns></returns>
        public ComsResponse<List<IntegrationInterface>> GetIntegrationInterfacesbyInterfaceid(int interfacetypeid)
        {
            try
            {

                var interfacelst = ListIntegrationInterfaces().Where(i => i.InterfaceTypeId == interfacetypeid).Select(i => new IntegrationInterface() { IntegrationGUID = i.IntegrationGUID, IntegrationName = i.IntegrationName }).ToList();
                return ComsResponse<List<IntegrationInterface>>.Success(string.Empty, (interfacelst));
            }
            catch (Exception ex)
            {
                //log.Error(ex);
                return ComsResponse<List<IntegrationInterface>>.Failed(ex.Message, null);
            }
        }

        /// <summary>
        /// Returns an instance of the card management system integration object with the specified <see cref="System.Guid"/>
        /// </summary>
        /// <param name="guid">
        /// The <see cref="System.Guid"/> of the integration layer, this may not be an empty string or <see langword="null"/>.
        /// </param>
        /// <returns></returns>
        public ICardManagementSystem CardManagementSystem(InterfaceInfo interfaceInfo, IDataSource dataSource)
        {
            var cmsInterface = iCardManagementSystems.Where(w => Guid.Parse(w.Metadata.IntegrationGUID) == Guid.Parse(interfaceInfo.InterfaceGuid)).SingleOrDefault();

            if (cmsInterface != null && cmsInterface.Value != null)
            {
                cmsInterface.Value.DataSource = dataSource;
                return cmsInterface.Value;
            }

            throw new Exception(String.Format(MISSING_INTEGRATION_DLL, interfaceInfo.InterfaceGuid));
        }

        public ICoreBankingSystem CoreBankingSystem(InterfaceInfo interfaceInfo, IDataSource dataSource)
        {
            var cmsInterface = iCoreBankingSystems.Where(w => Guid.Parse(w.Metadata.IntegrationGUID) == Guid.Parse(interfaceInfo.InterfaceGuid)).SingleOrDefault();
            // log.Debug(iCoreBankingSystems.Count());
            if (cmsInterface != null && cmsInterface.Value != null)
            {
                cmsInterface.Value.DataSource = dataSource;
                return cmsInterface.Value;
            }

            throw new Exception(String.Format(MISSING_INTEGRATION_DLL, interfaceInfo.InterfaceGuid));
        }

        public IPrepaidAccountProcessor PrepaidAccountSystem(InterfaceInfo interfaceInfo, IDataSource dataSource)
        {
            var cmsInterface = iPrepaidAccountProcessors.Where(w => Guid.Parse(w.Metadata.IntegrationGUID) == Guid.Parse(interfaceInfo.InterfaceGuid)).SingleOrDefault();
            // log.Debug(iCoreBankingSystems.Count());
            if (cmsInterface != null && cmsInterface.Value != null)
            {
                cmsInterface.Value.DataSource = dataSource;
                return cmsInterface.Value;
            }

            throw new Exception(String.Format(MISSING_INTEGRATION_DLL, interfaceInfo.InterfaceGuid));
        }
        /// <summary>
        /// Returns an instance of the Hardware Security Module integration object with the specified <see cref="System.Guid"/>
        /// </summary>
        /// <param name="guid">
        /// The <see cref="System.Guid"/> of the integration layer, this may not be an empty string or <see langword="null"/>.
        /// </param>
        /// <returns></returns>
        public IHardwareSecurityModule HardwareSecurityModule(InterfaceInfo interfaceInfo, IDataSource dataSource)
        {
            var hsmInterface = iHardwareSecurityModules.Where(w => Guid.Parse(w.Metadata.IntegrationGUID) == Guid.Parse(interfaceInfo.InterfaceGuid)).SingleOrDefault();

            if (hsmInterface != null && hsmInterface.Value != null)
            {
                hsmInterface.Value.DataSource = dataSource;
                return hsmInterface.Value;
            }

            throw new Exception(String.Format(MISSING_INTEGRATION_DLL, interfaceInfo.InterfaceGuid));
        }
        #endregion

        #region Private Methods

        ///// <summary>
        ///// List all the available interfaces found in the integration folder for Indigo
        ///// </summary>
        ///// <returns></returns>
        private List<IntegrationInterface> ListIntegrationInterfaces()
        {
            List<IntegrationInterface> availableInterfaces = new List<IntegrationInterface>();

            foreach (var item in iCardManagementSystems)
            {
                availableInterfaces.Add(new IntegrationInterface
                {
                    IntegrationName = item.Metadata.IntegrationName,
                    IntegrationGUID = item.Metadata.IntegrationGUID,
                    InterfaceTypeId = (int)InterfaceTypes.CARD_MANAGEMENT_SYSTEM
                });
            }

            foreach (var item in iHardwareSecurityModules)
            {
                availableInterfaces.Add(new IntegrationInterface
                {
                    IntegrationName = item.Metadata.IntegrationName,
                    IntegrationGUID = item.Metadata.IntegrationGUID,
                    InterfaceTypeId = (int)InterfaceTypes.HARDWARE_SECURITY_MODULE
                });
            }

            foreach (var item in iCardProductionSystems)
            {
                availableInterfaces.Add(new IntegrationInterface
                {
                    IntegrationName = item.Metadata.IntegrationName,
                    IntegrationGUID = item.Metadata.IntegrationGUID,
                    InterfaceTypeId = (int)InterfaceTypes.CARD_PRODUCTION
                });
            }
            foreach (var item in iCoreBankingSystems)
            {
                availableInterfaces.Add(new IntegrationInterface
                {
                    IntegrationName = item.Metadata.IntegrationName,
                    IntegrationGUID = item.Metadata.IntegrationGUID,
                    InterfaceTypeId = (int)InterfaceTypes.CORE_BANKING_SYSTEM
                });
            }

            foreach (var item in iCoreBankingSystems)
            {
                availableInterfaces.Add(new IntegrationInterface
                {
                    IntegrationName = item.Metadata.IntegrationName,
                    IntegrationGUID = item.Metadata.IntegrationGUID,
                    InterfaceTypeId = (int)InterfaceTypes.FEE_SCHEME
                });
            }
            foreach (var item in iCardFileProcessors)
            {
                availableInterfaces.Add(new IntegrationInterface
                {
                    IntegrationName = item.Metadata.IntegrationName,
                    IntegrationGUID = item.Metadata.IntegrationGUID,
                    InterfaceTypeId = (int)InterfaceTypes.FILE_LOADER
                });
            }
            foreach (var item in iThreedSecureFileGenerator)
            {
                availableInterfaces.Add(new IntegrationInterface
                {
                    IntegrationName = item.Metadata.IntegrationName,
                    IntegrationGUID = item.Metadata.IntegrationGUID,
                    InterfaceTypeId = (int)InterfaceTypes.THREED_SECURE
                });
            }

            foreach (var item in iExternalAuthentication)
            {
                availableInterfaces.Add(new IntegrationInterface
                {
                    IntegrationName = item.Metadata.IntegrationName,
                    IntegrationGUID = item.Metadata.IntegrationGUID,
                    InterfaceTypeId = (int)InterfaceTypes.EXTERNAL_AUTHENTICATION
                });
            }

            foreach (var item in iRemoteCMS)
            {
                availableInterfaces.Add(new IntegrationInterface
                {
                    IntegrationName = item.Metadata.IntegrationName,
                    IntegrationGUID = item.Metadata.IntegrationGUID,
                    InterfaceTypeId = (int)InterfaceTypes.REMOTE_CMS
                });
            }
            foreach (var item in iNotificationSystems)
            {
                availableInterfaces.Add(new IntegrationInterface
                {
                    IntegrationName = item.Metadata.IntegrationName,
                    IntegrationGUID = item.Metadata.IntegrationGUID,
                    InterfaceTypeId = (int)InterfaceTypes.NOTIFICATIONS
                });
            }
            foreach (var item in iMultiFactorAuthentication)
            {
                availableInterfaces.Add(new IntegrationInterface
                {
                    IntegrationName = item.Metadata.IntegrationName,
                    IntegrationGUID = item.Metadata.IntegrationGUID,
                    InterfaceTypeId = (int)InterfaceTypes.MULTIFACTOR
                });
            }

            foreach (var item in iPrepaidAccountProcessors)
            {
                availableInterfaces.Add(new IntegrationInterface
                {
                    IntegrationName = item.Metadata.IntegrationName,
                    IntegrationGUID = item.Metadata.IntegrationGUID,
                    InterfaceTypeId = (int)InterfaceTypes.PREPAID_FUNDS_LOAD
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
        public ComsResponse<bool> ReloadInterfaces(byte[] integrationfilestream, string checksum)
        {
            //use MEF to find and load all indigo integration dll's.
            AggregateCatalog catalog = GetIntegrationCatalog();

            var container = new CompositionContainer(catalog);

            AppDomain currentDomain = AppDomain.CurrentDomain;
            AppDomain.Unload(currentDomain);

            container.Dispose();

            DirectoryInfo dirInfo = new DirectoryInfo(GetIntegrationDir());

            string path = GetIntegrationDir() + "\\integration.rar";
            if (!dirInfo.Exists)
            {
                File.WriteAllBytes(path, integrationfilestream);

                if (CalculateMD5(path) == checksum)
                {
                    ZipFile.ExtractToDirectory(path, GetIntegrationDir());
                    LoadIntegrationController();
                }
                else
                {
                    File.Delete(path);
                }

            }



            return ComsResponse<bool>.Success("", true);

        }
        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
        public static void Decompress(FileInfo fileToDecompress)
        {
            using (FileStream originalFileStream = fileToDecompress.OpenRead())
            {
                string currentFileName = fileToDecompress.FullName;
                string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

                using (FileStream decompressedFileStream = File.Create(newFileName))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                        Console.WriteLine($"Decompressed: {fileToDecompress.Name}");
                    }
                }
            }
        }
        private IntegrationController()
        {
            LoadIntegrationController();
        }
        private void LoadIntegrationController()
        {
            try
            {
                //use MEF to find and load all indigo integration dll's.
                AggregateCatalog catalog = GetIntegrationCatalog();

                var container = new CompositionContainer(catalog);
                container.ComposeParts(container);

                //Setup lookupdir for integration dll's, may be needed in the integration
                AppDomain currentDomain = AppDomain.CurrentDomain;

                //AppDomainSetup setup = new AppDomainSetup();
                //setup.ApplicationBase = GetIntegrationDir();
                //setup.ShadowCopyFiles = "true";
                //AppDomain currentDomain = AppDomain.CreateDomain("ShadowCopy domain 1" , null, setup);
                currentDomain.AssemblyResolve += new ResolveEventHandler(IntegrationResolveEventHandler);

                //Load up CMS

                var cmsImports = container.GetExports<ICardManagementSystem, IIntegrationCapabilities>();
                iCardManagementSystems = cmsImports.ToArray();

                //Load up CBS
                var cbsImports = container.GetExports<ICoreBankingSystem, IIntegrationCapabilities>();
                iCoreBankingSystems = cbsImports.ToArray();

                //Load up CPS
                var cpsImports = container.GetExports<ICardProductionSystem, IIntegrationCapabilities>();
                iCardProductionSystems = cpsImports.ToArray();

                var fileprocessImports = container.GetExports<ICardFileProcessor, IIntegrationCapabilities>();
                iCardFileProcessors = fileprocessImports.ToArray();

                var externalImports = container.GetExports<IExternalAuthentication, IIntegrationCapabilities>();
                iExternalAuthentication = externalImports.ToArray();

                var threedsecureImports = container.GetExports<I3DSecureRegistration, IIntegrationCapabilities>();
                iThreedSecureFileGenerator = threedsecureImports.ToArray();

                var authenticationImports = container.GetExports<IMutiFactorAuthentication, IIntegrationCapabilities>();
                iMultiFactorAuthentication = authenticationImports.ToArray();

                var remoteCMSExports = container.GetExports<IRemoteCMS, IIntegrationCapabilities>();
                iRemoteCMS = remoteCMSExports.ToArray();


                var notificationsystems = container.GetExports<INotificationSystem, IIntegrationCapabilities>();
                iNotificationSystems = notificationsystems.ToArray();
                //Load up HSM
                var hsmImports = container.GetExports<IHardwareSecurityModule, IIntegrationCapabilities>();
                iHardwareSecurityModules = hsmImports.ToArray();

                var prepaidImports = container.GetExports<IPrepaidAccountProcessor, IIntegrationCapabilities>();
                iPrepaidAccountProcessors = prepaidImports.ToArray();
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

                //log.Error(sb.ToString());
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

            DirectoryInfo dirInfo = new DirectoryInfo(GetIntegrationDir());

            if (!dirInfo.Exists)
            {
                //log.WarnFormat("Directory {0} does not exist, cannot load integrations.", dirInfo.FullName);
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

        private static string GetIntegrationDir()
        {
            return ConfigurationManager.AppSettings["IntegrationDir"].ToString();
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
