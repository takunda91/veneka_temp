using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Linq;
using System.IO;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Integration.DAL;
using Common.Logging;
using Veneka.Indigo.Integration.FileLoader.Validation;
using Veneka.Indigo.Integration.Config;
using System.Text;
using Veneka.Indigo.COMS.DataSource;

namespace Veneka.Indigo.FileRunner
{
	internal class Processor
	{
        private static readonly ILog log = LogManager.GetLogger(typeof(Processor));
		private readonly string baseDir = ConfigurationManager.AppSettings["BaseConfigDir"].ToString();
		private readonly string SQLConnection;
        private IDataSource localDataSource;

        [ImportMany]
		private Lazy<ICardFileProcessor, IIntegrationCapabilities>[] iCardFileLoaders;

		public Processor()
		{
			using (StreamReader sr = new StreamReader(Path.Combine(baseDir, @"config\Database.config")))
			{
				SQLConnection = sr.ReadToEnd();
			}

			DirectoryCatalog catalog = new DirectoryCatalog(Path.Combine(baseDir, @"integration"), "*.dll");
			var container = new CompositionContainer(catalog);
			container.SatisfyImportsOnce(this);
            localDataSource = new LocalDataSource();
		}

        internal void Test()
        {
            //ProductValidation prodVal = new ProductValidation(SQLConnection);
            //string comment = String.Empty;
            //prodVal.Validate("6061231167890000", comment, 0, "");
            //prodVal.Validate("6061231167890000", comment, 0, "");
        }

		internal void RunFileProcessor()
		{
			foreach (var fileLoader in iCardFileLoaders)
			{
                fileLoader.Value.DataSource = localDataSource; 
                fileLoader.Value.BaseFileDir = baseDir;
                fileLoader.Value.SQLConnectionString = SQLConnection;
				fileLoader.Value.ProcessCardsFiles(null, null, fileLoader.Metadata.IntegrationGUID, 0, -2, "SYSTEM");
			}
		}

        internal void RunFileProcessorCentralized()
        {
            foreach (var fileLoader in iCardFileLoaders)
            {
                fileLoader.Value.DataSource = localDataSource;
                fileLoader.Value.BaseFileDir = baseDir;
                fileLoader.Value.SQLConnectionString = SQLConnection;
                fileLoader.Value.ProcessCardsFiles(null, null, fileLoader.Metadata.IntegrationGUID, 0, -2, "SYSTEM");
            }
        }

        internal void RunExportBatchGenerator()
        {

        }

		internal void RunFileGenerator()
		{
            //TODO: Maybe allows the file runner to have an option to process a single issuer..

            log.Info("START GENERATING EXPORT BATCHES");
            //Create export batches first then have integration layer write them out.
            List<int> exportBatchIds = new List<int>();
            ExportBatchGenerator generator = new ExportBatchGenerator(localDataSource.ExportBatchDAL);
            generator.Generate(null, 0, -2, "FileProcessorService", out exportBatchIds);
            log.InfoFormat("END GENERATING EXPORT BATCHES - BATCHES CREATED={0}", exportBatchIds.Count);

            if(log.IsDebugEnabled)            
                foreach (var exportId in exportBatchIds)
                    log.DebugFormat("Export ID: {0}", exportId);            


            log.Info("STARTING CMS FILE EXPORT");
            //InterfaceDAL interfaceDAL = new InterfaceDAL(SQLConnection);
            ConfigDAL configDal = new ConfigDAL(SQLConnection);

            //Find all products who have file export linked to them.
            //var configs = configDal.GetProductInterfaceConfigs(null, 8, 0, null, -2, "FileProcessorService");

            var products = localDataSource.ProductDAL.GetProductsForExport(null, true, -2, "FileProcessorService");

            if (products == null || products.Count == 0)
                log.Warn("No products setup for export.");

            //Get products for export
            foreach (var product in products)
            {
                log.InfoFormat("Exporting product {0}-{1}", product.ProductCode, product.ProductName);

                var config = configDal.GetProductInterfaceConfig(product.ProductId, 8, 0, null, -2, "FileProcessorService");

                if (config == null)
                {
                    log.WarnFormat("Product {0}-{1} has not been configured correctly to export files. Please check product config.", product.ProductCode, product.ProductName);
                    continue;
                }

                var fileLoader = iCardFileLoaders.Where(w => Guid.Parse(w.Metadata.IntegrationGUID) == config.InterfaceGuid).FirstOrDefault();
                               
                if (fileLoader != null)
                {
                    if (log.IsDebugEnabled)
                        log.DebugFormat("Using interface {0}({1})", fileLoader, config.InterfaceGuid);

                    fileLoader.Value.DataSource = localDataSource;
                    fileLoader.Value.SQLConnectionString = SQLConnection;
                    fileLoader.Value.BaseFileDir = baseDir;
                    fileLoader.Value.GenerateFiles(product.IssuerId, product.ProductId, config, 0, -2, "FileProcessorService");
                }
                else                
                    log.WarnFormat("Could not find .dll with GUID: {0}. Cannot export product.", config.InterfaceGuid);                
            }

            log.Info("END CMS FILE EXPORT");
		}

        internal void Run3DSecureGenerator()
        {
            log.Info("START GENERATING 3D SECURE BATCHES");

            //Create export batches first then have integration layer write them out.
            List<int> exportBatchIds = new List<int>();
            ExportBatchGenerator generator = new ExportBatchGenerator(localDataSource.ExportBatchDAL);

            generator.Generate(null, 0, -2, "FileProcessorService", out exportBatchIds);
            log.InfoFormat("END GENERATING EXPORT BATCHES - BATCHES CREATED={0}", exportBatchIds.Count);

            if (log.IsDebugEnabled)
                foreach (var exportId in exportBatchIds)
                    log.DebugFormat("Export ID: {0}", exportId);


            log.Info("STARTING CMS FILE EXPORT");
            //InterfaceDAL interfaceDAL = new InterfaceDAL(SQLConnection);
            ConfigDAL configDal = new ConfigDAL(SQLConnection);

            //Find all products who have file export linked to them.
            //var configs = configDal.GetProductInterfaceConfigs(null, 8, 0, null, -2, "FileProcessorService");

            var products = localDataSource.ProductDAL.GetProductsForExport(null, true, -2, "FileProcessorService");

            if (products == null || products.Count == 0)
                log.Warn("No products setup for export.");

            //Get products for export
            foreach (var product in products)
            {
                log.InfoFormat("Exporting product {0}-{1}", product.ProductCode, product.ProductName);

                var config = configDal.GetProductInterfaceConfig(product.ProductId, 8, 0, null, -2, "FileProcessorService");

                if (config == null)
                {
                    log.WarnFormat("Product {0}-{1} has not been configured correctly to export files. Please check product config.", product.ProductCode, product.ProductName);
                    continue;
                }

                var fileLoader = iCardFileLoaders.Where(w => Guid.Parse(w.Metadata.IntegrationGUID) == config.InterfaceGuid).FirstOrDefault();

                if (fileLoader != null)
                {
                    if (log.IsDebugEnabled)
                        log.DebugFormat("Using interface {0}({1})", fileLoader, config.InterfaceGuid);

                    fileLoader.Value.DataSource = localDataSource;
                    fileLoader.Value.SQLConnectionString = SQLConnection;
                    fileLoader.Value.BaseFileDir = baseDir;
                    fileLoader.Value.GenerateFiles(product.IssuerId, product.ProductId, config, 0, -2, "FileProcessorService");
                }
                else
                    log.WarnFormat("Could not find .dll with GUID: {0}. Cannot export product.", config.InterfaceGuid);
            }

            log.Info("END CMS FILE EXPORT");
        }

        internal void RunBulkFileProcesser()
        {
            foreach (var fileLoader in iCardFileLoaders)
            {
                fileLoader.Value.DataSource = localDataSource;
                fileLoader.Value.SQLConnectionString = SQLConnection;
                fileLoader.Value.BaseFileDir = baseDir;
                fileLoader.Value.ProcessBulkRequestFiles(null, null, fileLoader.Metadata.IntegrationGUID, 0, -2, "SYSTEM");
            }
        }
	}
}