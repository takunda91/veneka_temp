using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Veneka.Indigo.Integration;
using System.IO;
using Common.Logging;
using System.Reflection;
using System.Text;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.External;
using Veneka.Indigo.NotificationService.DAL;

namespace Veneka.Indigo.NotificationService.Integration
{
    /// <summary>
    /// Controller provices method for getting the correct integration layer to be used.
    /// Integration layers are lazy loaded and are referenced by their GUID's.
    /// </summary>
    public class IntegrationController: IDisposable
    {
        #region Constants
        private const string ISSUER_MISSING_INTERFACE = "{0} interface not configured for issuer, please check issuer setup.";
        private const string MISSING_INTEGRATION_DLL = "Could not find integration library with GUID={0}. Make sure that the integration .dll is present and restart the application.";

        private const int INTERFACE_TYPE_ID = 7;
        private const int INTERFACE_AREA = 0;
        #endregion

        #region Fields
        private static readonly ILog log = LogManager.GetLogger(typeof(IntegrationController));

        private readonly ConfigDAL _configDAL;
        private readonly ExternalSystemDAL _externalSystemDal;

        private readonly string _baseDir;
        private readonly string _connectionString;
        private readonly long _auditUserId;
        private readonly string _auditWorkstation;

        private readonly AggregateCatalog _catalog;
        private readonly CompositionContainer _container;
        #endregion

        #region MEF Imports
        private Lazy<INotificationSystem, IIntegrationCapabilities>[] iNotificationSystems;
        #endregion

        #region Private Constructor
        public IntegrationController(string baseDir, string connectionString, long auditUserId, string auditWorkstation)
        {
            this._baseDir = baseDir;
            this._connectionString = connectionString;

            this._auditUserId = auditUserId;
            this._auditWorkstation = auditWorkstation;

            try
            {
                _configDAL = new ConfigDAL(_connectionString);
                _externalSystemDal = new ExternalSystemDAL(_connectionString);

                //use MEF to find and load all nodes.
                _catalog = new AggregateCatalog();
                
                _catalog.Catalogs.Add(new DirectoryCatalog(Path.Combine(_baseDir, @"integration"), "*.dll"));
                _container = new CompositionContainer(_catalog);                    
                _container.ComposeParts(_container);                              

                //Setup lookupdir for integration dll's, may be needed in the integration
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.AssemblyResolve += new ResolveEventHandler(IntegrationResolveEventHandler);

                var notificationImports = _container.GetExports<INotificationSystem, IIntegrationCapabilities>();
                iNotificationSystems = notificationImports.ToArray();
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
        #endregion

        #region AppDomain
        private Assembly IntegrationResolveEventHandler(object sender, ResolveEventArgs args)
        {
            //This handler is called only when the common language runtime tries to bind to the assembly and fails.

            //Retrieve the list of referenced assemblies in an array of AssemblyName.
            string strTempAssmbPath = "";

            strTempAssmbPath = Path.Combine(_baseDir, @"integration\") + args.Name.Substring(0, args.Name.IndexOf(",")) + ".dll";
            
            //Load the assembly from the specified path.                    
            var MyAssembly = Assembly.LoadFrom(strTempAssmbPath);

            //Return the loaded assembly.
            return MyAssembly;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns an instance of the card management system integration object with the specified <see cref="System.Guid"/>
        /// </summary>
        /// <param name="guid">
        /// The <see cref="System.Guid"/> of the integration layer, this may not be an empty string or <see langword="null"/>.
        /// </param>
        /// <returns></returns>
        public INotificationSystem NotificationSystem(int issuerId,int interface_area, out IConfig config)
        {
            var interfaceConfig = _configDAL.GetIssuerInterfaceConfig(issuerId,
                                                            INTERFACE_TYPE_ID,
                                                            interface_area,
                                                            null,
                                                            _auditUserId,
                                                            _auditWorkstation);

            if (interfaceConfig == null)
                throw new Exception(String.Format(ISSUER_MISSING_INTERFACE, "NOTIFICATION_SYSTEM"));

            var notificationInterface = iNotificationSystems.Where(w => Guid.Parse(w.Metadata.IntegrationGUID) == interfaceConfig.InterfaceGuid).SingleOrDefault();

            if (notificationInterface != null && notificationInterface.Value != null)
            {
                notificationInterface.Value.DataSource =new LocalDataSource(_connectionString);
                config = interfaceConfig;
                return notificationInterface.Value;
            }

            throw new Exception(String.Format(MISSING_INTEGRATION_DLL, interfaceConfig.InterfaceGuid));
        }
        #endregion

        #region Private Methods
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

        public void Dispose()
        {
            iNotificationSystems = null;
            _container.Dispose();
            _catalog.Dispose();
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