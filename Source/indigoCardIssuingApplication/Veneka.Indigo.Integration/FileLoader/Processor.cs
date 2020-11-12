using Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.FileLoader.BLL;
using Veneka.Indigo.Integration.FileLoader.Crypto;
using Veneka.Indigo.Integration.FileLoader.DAL;
using Veneka.Indigo.Integration.FileLoader.Objects;
using Veneka.Indigo.Integration.FileLoader.Validation;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.Integration.FileLoader
{
    public enum FileStatuses
    {
        READ,
        VALID_CARDS,
        PROCESSED,
        DUPLICATE_FILE,
        LOAD_FAIL,
        FILE_CORRUPT,
        PARTIAL_LOAD,
        INVALID_FORMAT,
        INVALID_NAME,
        ISSUER_NOT_FOUND,
        INVALID_ISSUER_LICENSE,
        NO_ACTIVE_BRANCH_FOUND,
        DUPLICATE_CARDS_IN_FILE,
        DUPLICATE_CARDS_IN_DATABASE,
        FILE_DECRYPTION_FAILED,
        UNLICENSED_BIN,
        NO_PRODUCT_FOUND_FOR_CARD,
        UNLICENSED_ISSUER,
        BRANCH_PRODUCT_NOT_FOUND,
        CARD_FILE_INFO_READ_ERROR,
          CARDS_NOT_ORDERED,
        ORDERED_CARD_REF_MISSING,
        ORDERED_CARD_PRODUCT_MISS_MATCH,
        MULTIPLE_PRODUCTS_IN_FILE,
        PRODUCT_NOT_ACTIVE,
        NO_LOAD_FOR_PRODUCT,
        ISSUER_NOT_ACTIVE,
        ISSUER_LICENCE_EXPIRED
      
    }

    public abstract class Processor
    {
        protected const string DECRYPTED_EXTENTION = "_wrk";

        protected readonly ILog log;

        protected string _connectionString;
        protected string _logger;
        protected int _languageId;
        protected string _baseFileDir;

        protected long AuditUserId { get; set; }
        protected string AuditWorkStation { get; set; }

        protected List<string> licensedBinCodes = new List<string>();

        protected FileLoaderBLL _fileLoaderBLL;
        protected FileLoaderDAL _fileLoaderDAL;

        protected CardValidation cardValidation;
        protected BranchValidation branchValidation;
        protected IssuerValidation issuerValidation;
        protected ProductValidation productValidation;
        protected FileValidation fileValidation;

        protected Processor(string connectionString, IDataSource dataSource, string baseFileDir, string logger, int languageId, long auditUserId, string auditWorkStation)
        {
            log = LogManager.GetLogger(logger);            

            this._connectionString = connectionString;
            this._logger = logger;
            this._languageId = languageId;
            this.AuditUserId = auditUserId;
            this.AuditWorkStation = auditWorkStation;

            this._fileLoaderBLL = new FileLoaderBLL(connectionString, logger, languageId, auditUserId, auditWorkStation);
            this._fileLoaderDAL = new FileLoaderDAL(connectionString);

            fileValidation = new FileValidation(connectionString, logger);
            productValidation = new ProductValidation(dataSource.ProductDAL, logger);
            issuerValidation = new IssuerValidation(dataSource.IssuerDAL, Path.Combine(baseFileDir, @"license\machine.vmg"), logger);
            branchValidation = new BranchValidation(dataSource.BranchDAL, logger);
            cardValidation = new CardValidation(connectionString, logger);

            
            _baseFileDir = baseFileDir;
        }        

        /// <summary>
        /// Delete the file or archive it
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="parameters"></param>
        /// <param name="fileHistory"></param>
        protected void FileCleanup(FileInfo fileInfo, bool deleteFile, FileHistory fileHistory)
        {
            //File cleanup
            try
            {
                log.Trace(m => m("File Cleanup started."));

                if (deleteFile)
                {
                    log.Debug(m => m("Deleting file: " + fileInfo.FullName));
                    fileInfo.Delete();
                    log.Trace(m => m("Deleted file: " + fileInfo.FullName));
                }
                else
                {
                    log.Debug(m => m("Archiving file: " + fileInfo.FullName));
                    DirectoryInfo archiveDir;

                    if (fileHistory.FileStatus == FileStatuses.PROCESSED)
                        archiveDir = new DirectoryInfo(Path.Combine(fileInfo.DirectoryName, "SUCCESSFUL", fileHistory.LoadBatchReference));
                    else
                        archiveDir = new DirectoryInfo(Path.Combine(fileInfo.DirectoryName, "FAILED", fileHistory.LoadBatchReference));

                    if (!archiveDir.Exists)
                        archiveDir.Create();

                    log.Debug(m => m("Archive location: " + archiveDir.FullName));

                    fileInfo.MoveTo(Path.Combine(archiveDir.FullName, fileInfo.Name));

                    log.Trace(m => m("Archived file " + fileInfo.FullName + " to " + archiveDir.FullName));
                }

                log.Trace(m => m("File Cleanup completed."));
            }
            catch (Exception ex)
            {
                log.Error(ex);
                log.Trace(m => m("File Cleanup failed."));
            }
        }

        /// <summary>
        /// Writes the file history to the DB
        /// </summary>
        /// <param name="fileComments"></param>
        /// <param name="fileHistory"></param>
        protected void CommitFileHistory(List<FileCommentsObject> fileComments, FileHistory fileHistory)
        {
            log.Trace(m => m("Commiting file history to DB."));
            fileHistory.FileLoadComments = _fileLoaderBLL.BuildFileComments(fileComments);

            //Write FileHistory to DB.
            try
            {
                log.Debug(m => m("fileHistory status: " + fileHistory.FileStatus.ToString()));
                //if (fileHistory.FileStatus != FileStatuses.PROCESSED)
                //{
                    _fileLoaderDAL.SaveFileInfo(fileHistory);
                //}

                log.Trace(m => m("Commiting file history to DB complete."));
            }
            catch (Exception ex)
            {
                log.Error(ex);
                log.Trace(m => m("Commiting file history to DB failed."));
            }
        }
    }
}
