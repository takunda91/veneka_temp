using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Veneka.Indigo.Integration;
using System.IO;
using Common.Logging;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using Veneka.Indigo.Integration.Remote;
using Veneka.Indigo.RemoteComponentClient.Configuration;
using System.ComponentModel.Composition.Primitives;

namespace Veneka.Indigo.RemoteComponentClient.BLL
{
    /// <summary>
    /// Controller provices method for getting the correct integration layer to be used.
    /// Integration layers are lazy loaded and are referenced by their GUID's.
    /// </summary>
    public class IntegrationController : IDisposable
    {
        private const string MISSING_INTEGRATION_DLL = "Could not find integration library with GUID={0}. Make sure that the integration .dll is present and restart the service.";

        private static readonly ILog log = LogManager.GetLogger(typeof(IntegrationController));

        #region MEF Imports        
        private Lazy<ICardManagementSystem, IIntegrationCapabilities>[] iCMS;
        private Lazy<IRemoteCMS, IIntegrationCapabilities>[] iRemoteCMS;
        #endregion

        #region Constructors
        public IntegrationController(DirectoryInfo integrationPath)
        {            
            var catalog = GetIntegrationCatalog(integrationPath);
            ComposeCatalog(catalog);
        }

        public IntegrationController(ComposablePartCatalog catalog)
        {
            ComposeCatalog(catalog);
        }
        #endregion

        #region Helpers
        private void ComposeCatalog(ComposablePartCatalog catalog)
        {
            try
            {
                var container = new CompositionContainer(catalog);
                container.ComposeParts(container);

                //Setup lookupdir for integration dll's, may be needed in the integration
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.AssemblyResolve += new ResolveEventHandler(IntegrationResolveEventHandler);

                //Load up CMS
                var cmsImports = container.GetExports<ICardManagementSystem, IIntegrationCapabilities>();
                iCMS = cmsImports.ToArray();

                //Load up Remote CMS
                var remoteCMSImports = container.GetExports<IRemoteCMS, IIntegrationCapabilities>();
                iRemoteCMS = remoteCMSImports.ToArray();
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
        private AggregateCatalog GetIntegrationCatalog(DirectoryInfo dirInfo)
        {
            AggregateCatalog catalog = new AggregateCatalog();

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
            string strTempAssmbPath = "";

            strTempAssmbPath = ConfigReader.ApplicationConfigPath.FullName + args.Name.Substring(0, args.Name.IndexOf(",")) + ".dll";
            
            
            //Load the assembly from the specified path.                    
            var MyAssembly = Assembly.LoadFrom(strTempAssmbPath);

            //Return the loaded assembly.
            return MyAssembly;
        }
        #endregion

        #region Public Methods
        #region Remote CMS
        /// <summary>
        /// Returns an instance of the card management system integration object with the specified <see cref="System.Guid"/>
        /// </summary>
        /// <param name="guid">
        /// The <see cref="System.Guid"/> of the integration layer, this may not be an empty string or <see langword="null"/>.
        /// </param>
        /// <returns></returns>
        public IRemoteCMS RemoteCardManagementSystem(Guid interfaceGuid)
        {
            var remoteCMSInterface = iRemoteCMS.Where(w => Guid.Parse(w.Metadata.IntegrationGUID) == interfaceGuid).SingleOrDefault();

            if (remoteCMSInterface != null && remoteCMSInterface.Value != null)
            {
                remoteCMSInterface.Value.ApplicationDirectory = ConfigReader.ApplicationConfigPath;
                return remoteCMSInterface.Value;
            }

            throw new Exception(String.Format(MISSING_INTEGRATION_DLL, interfaceGuid));
        }

        public IRemoteCMS RemoteCardManagementSystem(string interfaceGuid)
        {
            var guid = ValidateGuidArgument(interfaceGuid);
            return RemoteCardManagementSystem(guid);
        }
        #endregion

        #region Card Management System
        public ICardManagementSystem CardManagementSystem(Guid interfaceGuid)
        {
            var cmsInterface = iCMS.Where(w => Guid.Parse(w.Metadata.IntegrationGUID) == interfaceGuid).SingleOrDefault();

            if (cmsInterface != null && cmsInterface.Value != null)
            {
                cmsInterface.Value.IntegrationFolder = ConfigReader.ApplicationConfigPath;
                return cmsInterface.Value;
            }

            throw new Exception(String.Format(MISSING_INTEGRATION_DLL, interfaceGuid));
        }

        public ICardManagementSystem CardManagementSystem(string interfaceGuid)
        {
            var guid = ValidateGuidArgument(interfaceGuid);
            return CardManagementSystem(guid);
        }
        #endregion

        /// <summary>
        /// List all the available interfaces found in the integration folder for Indigo
        /// </summary>
        /// <returns></returns>
        public List<IntegrationInterface> Interfaces
        {
            get
            {
                List<IntegrationInterface> availableInterfaces = new List<IntegrationInterface>();

                foreach (var item in iCMS)
                {
                    availableInterfaces.Add(new IntegrationInterface
                    {
                        IntegrationName = item.Metadata.IntegrationName,
                        IntegrationGUID = item.Metadata.IntegrationGUID,
                        InterfaceTypeId = 1
                    });
                }

                foreach (var item in iRemoteCMS)
                {
                    availableInterfaces.Add(new IntegrationInterface
                    {
                        IntegrationName = item.Metadata.IntegrationName,
                        IntegrationGUID = item.Metadata.IntegrationGUID,
                        InterfaceTypeId = 1
                    });
                }

                return availableInterfaces;
            }
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
        #endregion

        #region Structs
        public struct IntegrationInterface
        {
            public string IntegrationName { get; set; }
            public string IntegrationGUID { get; set; }
            public int InterfaceTypeId { get; set; }
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                foreach (var remoteCMS in iRemoteCMS)
                {
                    try
                    {
                        remoteCMS.Value.Dispose();
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }                

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~IntegrationController() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }        
        #endregion
    }
}