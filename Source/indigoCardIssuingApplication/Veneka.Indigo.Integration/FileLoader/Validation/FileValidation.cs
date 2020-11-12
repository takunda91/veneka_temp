using Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.FileLoader.DAL;
using Veneka.Indigo.Integration.FileLoader.Objects;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.Integration.FileLoader.Validation
{
    public sealed class FileValidation
    {
        private const string VALIDATE_FILE_INFO = "Validating file with name {0}.";
        private const string DUPLICATE_FILE_FOUND = "File with name {0} already loaded successfully.";
        private const string NO_RECORDS_IN_FILE = "File with name {0} has no records.";
        private const string NUMBER_RECORDS_IN_FILE = "Validating file with name {0} has {1} record/s, completed.";


        private readonly ILog _logger;

        private readonly FileLoaderDAL _fileLoaderDal;

        public FileValidation(string connectionString, string logger)
        {
            this._fileLoaderDal = new FileLoaderDAL(connectionString);
            _logger = LogManager.GetLogger(logger);
        }

        public FileStatuses Validate(Parameters parameter, FileInfo fileInfo, CardFile cardFile, List<FileCommentsObject> fileComments, long auditUserId, string auditWorkstation)
        {
            fileComments.Add(new FileCommentsObject(String.Format(VALIDATE_FILE_INFO, fileInfo.Name), _logger.Info));

            if (parameter.DuplicateFileCheck)
            {
                if (_fileLoaderDal.CheckIfFileExists(fileInfo.Name))
                {
                    fileComments.Add(new FileCommentsObject(String.Format(DUPLICATE_FILE_FOUND, fileInfo.Name), _logger.Error));
                    return FileStatuses.DUPLICATE_FILE;
                }
            }            

            //Make sure we have card records returned from the file.
            if (cardFile.CardFileRecords.Count <= 0)
            {
                fileComments.Add(new FileCommentsObject(String.Format(NO_RECORDS_IN_FILE, fileInfo.Name), _logger.Error));
                return FileStatuses.CARD_FILE_INFO_READ_ERROR;
            }
            
            fileComments.Add(new FileCommentsObject(String.Format(NUMBER_RECORDS_IN_FILE, fileInfo.Name, cardFile.CardFileRecords.Count), _logger.Info));

            return FileStatuses.READ;
        }

        public FileStatuses Validate(Parameters parameter, FileInfo fileInfo, BulkRequestsFile cardRequestFile, ref List<FileCommentsObject> fileComments, long auditUserId, string auditWorkstation)
        {
            //string fileComment = String.Empty;

            if (parameter.DuplicateFileCheck)
            {
                if (_fileLoaderDal.CheckIfFileExists(fileInfo.Name))
                {
                    //_logger.Error(fileComment = String.Format(DUPLICATE_FILE_FOUND, fileInfo.Name));
                    fileComments.Add(new FileCommentsObject(String.Format(DUPLICATE_FILE_FOUND, fileInfo.Name), _logger.Error));
                    return FileStatuses.DUPLICATE_FILE;
                }
            }

            //Make sure we have card records returned from the file.
            if (cardRequestFile.CardRequestFileRecords.Count <= 0)
            {
                //_logger.Error(fileComment = String.Format(NO_RECORDS_IN_FILE, fileInfo.Name));
                fileComments.Add(new FileCommentsObject(String.Format(NO_RECORDS_IN_FILE, fileInfo.Name), _logger.Error));
                return FileStatuses.CARD_FILE_INFO_READ_ERROR;
            }

            //_logger.Info(fileComment = String.Format(NUMBER_RECORDS_IN_FILE, fileInfo.Name, cardRequestFile.CardRequestFileRecords.Count));
            fileComments.Add(new FileCommentsObject(String.Format(NUMBER_RECORDS_IN_FILE, fileInfo.Name, cardRequestFile.CardRequestFileRecords.Count), _logger.Info));


            // TO DO: File Encryption Checking 
            if (parameter.FileEncryption.Equals(1)) // No File Encryption 
            {

            }
            else
            {
                //Do decryption here.

            }

            return FileStatuses.READ;
        }
    }
}