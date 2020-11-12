using Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Integration.EMP.BLL;
using Veneka.Indigo.Integration.FileLoader;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.Integration.EMP
{

    [IntegrationExport("EMPFileProcessor", "A9B5557D-8BE2-44E8-9C96-0EF08EB9F4E5", typeof(ICardFileProcessor))]
    public class EMPFileProcessor : ICardFileProcessor
    {
        private readonly ILog _fileLoaderLog = LogManager.GetLogger(General.FILE_LOADER_LOGGER);

        public string SQLConnectionString
        {
            get;
            set;
        }

        public IDataSource DataSource { get; set; }
        
        public string BaseFileDir
        {
            get;
            set;
        }

        public DirectoryInfo IntegrationFolder { get; set; }


        public bool GenerateFiles(int issuerId, int? productId, Config.IConfig config, int languageId, long auditUserId, string auditWorkStation)
        {
            throw new NotImplementedException();
        }

        public bool ProcessCardsFiles(int? issuerId, int? productId, string interfaceGuid, int languageId, long auditUserId, string auditWorkStation)
        {
            if (String.IsNullOrWhiteSpace(this.SQLConnectionString))
                throw new ArgumentNullException("SQL Connection String is null or empty");

           // ParametersDAL pdal = new ParametersDAL(this.SQLConnectionString);

            var parameters =DataSource.ParametersDAL.GetParametersProductInterface(productId, 4, 0, interfaceGuid, auditUserId, auditWorkStation);

            if (parameters == null && parameters.Count <= 0)
            {
                _fileLoaderLog.Error("No file parameters found for file loader.");
                return false;
            }
            
            
            FileReaderBLL fileReader = new FileReaderBLL();


            FileProcessor p = new FileProcessor(this.SQLConnectionString, this.DataSource,
                                                this.BaseFileDir,
                                                General.FILE_LOADER_LOGGER,
                                                fileReader.ReadFile,
                                                null,
                                                fileReader.GenerateCardReferences,fileReader.WriteFile,
                                                languageId, auditUserId, auditWorkStation);

            //TODO: Fix
            foreach (var parm in parameters)
            {
                if (String.IsNullOrEmpty(parm.Path))
                    _fileLoaderLog.Warn("Empty path found in parameters, skipping...");
                else
                {
                    _fileLoaderLog.InfoFormat("Processing files in directory {0}", parm.Path);
                    p.ProcessFiles(parm.Path, parameters);
                }
            }

            return true;
        }

       

        public bool ProcessBulkRequestFiles(int? issuerId, int? productId, string interfaceGuid, int languageId, long auditUserId, string auditWorkStation)
        {
            return true;
        }
    }
}
