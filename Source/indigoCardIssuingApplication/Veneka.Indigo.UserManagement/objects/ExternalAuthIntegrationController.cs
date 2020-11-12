using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Integration;
using System.IO;
using Common.Logging;

namespace Veneka.Indigo.UserManagement.objects
{ /// <summary>
    /// Controller provices method for getting the correct integration layer to be used.
    /// Integration layers are lazy loaded and are referenced by their GUID's.
    /// </summary>
    public class ExternalAuthIntegrationController
    {
        private const string PRODUCT_MISSING_INTERFACE = "{0} interface not configured for product, please check product setup.";
        private const string ISSUER_MISSING_INTERFACE = "{0} interface not configured for issuer, please check issuer setup.";
        private const string MISSING_INTEGRATION_DLL = "Could not find integration library with GUID={0}. Make sure that the integration .dll is present and restart the application.";

        private static readonly ILog log = LogManager.GetLogger(typeof(ExternalAuthIntegrationController));
       

        private static volatile ExternalAuthIntegrationController instance;
        private static object syncRoot = new Object();

        public static ExternalAuthIntegrationController Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ExternalAuthIntegrationController();
                    }
                }

                return instance;
            }
        }

        #region MEF Imports

        private Lazy<IMutiFactorAuthentication, IIntegrationCapabilities>[] iMultiFactorAuthentication  ;

        //[ImportMany]
        private Lazy<IExternalAuthentication, IIntegrationCapabilities>[] iExternalAuthentication;

        #endregion

        #region Private Constructor

        private ExternalAuthIntegrationController()
        {
            //DirectoryCatalog catalog = new DirectoryCatalog(Path.Combine(SystemConfiguration.Instance.GetBaseConfigDir(), @"integration"), "*.dll");
            //var container = new CompositionContainer(catalog);
            //container.SatisfyImportsOnce(this);

            //use MEF to find and load all nodes.
            AggregateCatalog catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(Path.Combine(SystemConfiguration.Instance.GetBaseConfigDir(), @"integration"), "*.dll"));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(container);

           

            var externalImports = container.GetExports<IExternalAuthentication, IIntegrationCapabilities>();
            iExternalAuthentication = externalImports.ToArray();


            var multifactorImports = container.GetExports<IMutiFactorAuthentication, IIntegrationCapabilities>();
            iMultiFactorAuthentication = multifactorImports.ToArray();
        }

        #endregion

        #region Public Methods
        public IExternalAuthentication ExternalAuthentication(string guid)
        {
            Guid guidOut = ValidateGuidArgument(guid);
            var cbsInterface = iExternalAuthentication.Where(w => Guid.Parse(w.Metadata.IntegrationGUID) == guidOut).SingleOrDefault();

            if (cbsInterface != null && cbsInterface.Value != null)
            {
                //cbsInterface.Value.SQLConnectionString = DatabaseConnectionObject.Instance.SQLConnectionString;
                return cbsInterface.Value;
            }
            throw new Exception(String.Format(MISSING_INTEGRATION_DLL, guid));

        }

        public IMutiFactorAuthentication MultiFactorAuthentication(string guid)
        {
            Guid guidOut = ValidateGuidArgument(guid);
            var multifactorauth = iMultiFactorAuthentication.Where(w => Guid.Parse(w.Metadata.IntegrationGUID) == guidOut).SingleOrDefault();

            if (multifactorauth != null && multifactorauth.Value != null)
            {
               // multifactorauth.Value.SQLConnectionString = DatabaseConnectionObject.Instance.SQLConnectionString;
                return multifactorauth.Value;
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

          
            foreach (var item in iExternalAuthentication)
            {
                availableInterfaces.Add(new IntegrationInterface
                {
                    IntegrationName = item.Metadata.IntegrationName,
                    IntegrationGUID = item.Metadata.IntegrationGUID,
                    InterfaceTypeId = (int)InterfaceTypes.EXTERNAL_AUTHENTICATION
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
