using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using System.IO;
using Veneka.Indigo.Integration.FileLoader.BLL;
using Veneka.Indigo.Integration.FileLoader.DAL;
using Veneka.Indigo.Integration.FileLoader.Objects;
using Veneka.Indigo.Integration.FileLoader.Validation;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.Integration.FileLoader.Crypto;

namespace Veneka.Indigo.Integration.FileLoader
{
    public class BulkRequests : Processor
    {
        private ReadFile _readFile;
        private ReadStream _readStream;
        private GenerateCardReferences _genBulkRequestsCardReferences;
        private ClientValidation _clientValidation;

        public delegate BulkRequestsFile ReadFile(FileInfo fileInfo, ref List<FileCommentsObject> fileComments);
        public delegate BulkRequestsFile ReadStream(Stream cardInfo, string fileName, ref List<FileCommentsObject> fileComments);
        public delegate void GenerateCardReferences(BulkRequestsFile cardFile);
        public delegate FileStatuses ClientValidation(BulkRequestsFile cardRequestFile, List<FileCommentsObject> fileComments, long auditUserId, string auditWorkstation);

        public BulkRequests(ReadFile readFile, GenerateCardReferences generateBulkRequestsCardRefs, string baseFileDir,
            string connectionString, IDataSource dataSource, string logger, int languageId, long auditUserId, string auditWorkStation)
            : base(connectionString, dataSource, baseFileDir, logger, languageId, auditUserId, auditWorkStation)
        {
            if (readFile == null)
                throw new ArgumentNullException("readFile", "Cannot be null.");

            _readFile = readFile;

            _genBulkRequestsCardReferences = generateBulkRequestsCardRefs;
        }

        public BulkRequests(ReadStream readStream, GenerateCardReferences generateBulkRequestsCardRefs, string baseFileDir,
            string connectionString, IDataSource dataSource, string logger, int languageId, long auditUserId, string auditWorkStation)
            :base(connectionString, dataSource, baseFileDir, logger, languageId, auditUserId, auditWorkStation)
        {
            if (readStream == null)
                throw new ArgumentNullException("readStream", "Cannot be null.");

            _readStream = readStream;
            _genBulkRequestsCardReferences = generateBulkRequestsCardRefs;
        }

        

        public void ProcessBulkRequestsCardFile(string fileLocation, List<Parameters> parameters)
        {
            int fileLoadId = 0;

            try
            {
                //Create file_load for this file load session
                DateTimeOffset startTime = DateTimeOffset.Now;

                //get all files from issuer directory
                if (Directory.Exists(fileLocation))
                {
                    var dirInfo = new DirectoryInfo(fileLocation);
                    fileLoadId = _fileLoaderDAL.CreateFileLoad(startTime, dirInfo.GetFiles().Count(), AuditUserId, AuditWorkStation);
                    var param = parameters.Where(p => p.Path.Equals(fileLocation)).FirstOrDefault();

                    //log.Debug(m => m("Reading Encrypted Files from " + cardsFileLocation + "..."));

                    foreach (FileInfo fileInfo in dirInfo.GetFiles())
                    {
                        ProcessBulkRequestsCardFile(fileInfo, fileLoadId, param);
                    }
                }
            }
            catch (DirectoryNotFoundException dirEx)
            {
                log.Warn(dirEx);
            }
            catch (Exception ex)
            {
                log.Fatal("Error when running file loader.", ex);
            }
            finally
            {
                if (fileLoadId != 0)
                    _fileLoaderDAL.UpdateFileLoad(fileLoadId, AuditUserId, AuditWorkStation);
            }
        }

        public void ProcessBulkRequestsCardFile(FileInfo fileInfo, int fileLoadId, Parameters parameter)
        {
            List<FileCommentsObject> fileComments = new List<FileCommentsObject>();
            licensedBinCodes.Clear();

            //log.Debug(m => m("Processing file: " + fileInfo.Name));
            fileComments.Add(new FileCommentsObject("Processing file: " + fileInfo.Name, log.Info));

            //Build file_history object.
            FileHistory fileHistory = new FileHistory
            {
                FileLoadId = fileLoadId,
                FileStatus = FileStatuses.READ,
                NameOfFile = fileInfo.Name,
                FileDirectory = fileInfo.Directory.ToString(),
                FileSize = (int)fileInfo.Length,
                FileCreatedDate = fileInfo.CreationTime,
                LoadDate = DateTimeOffset.Now
            };

            try
            {
                try
                {
                    BulkRequestsFile bulkRequestsFile;

                    try
                    {
                        //Read all records from the file and decode into CardRequestFileRecord Object. 
                        bulkRequestsFile = ReadOperation(fileInfo, parameter, ref fileComments);

                        if (bulkRequestsFile == null)
                            throw new Exception("bulkRequestsFile is null.");
                    }
                    catch (Exception ex)
                    {
                        fileComments.Add(new FileCommentsObject("Failed to read file.", ex, log.Error));
                        fileHistory.FileStatus = FileStatuses.CARD_FILE_INFO_READ_ERROR;
                        //log.Error(ex);
                        return;
                    }

                    //Do file validations
                    if ((fileValidation.Validate(parameter, fileInfo, bulkRequestsFile, ref fileComments, AuditUserId, AuditWorkStation)) != FileStatuses.READ)
                        return;

                    //Do product validations
                    productValidation.Clear();
                    if ((fileHistory.FileStatus = productValidation.ValidateBulkRequestsFile(bulkRequestsFile, fileComments, AuditUserId, AuditWorkStation)) != FileStatuses.READ)
                        return;

                    //Do issuer validations
                    issuerValidation.Clear();
                    if ((fileHistory.FileStatus = issuerValidation.ValidateBulkRequestsFile(bulkRequestsFile, fileComments, AuditUserId, AuditWorkStation)) != FileStatuses.READ)
                        return;

                    //Do branch valaidations
                    // Remember to clear out branch code or it might pick up a different issues branch code from cache
                    branchValidation.Clear();
                    if ((fileHistory.FileStatus = branchValidation.ValidateBulkRequestsFile(bulkRequestsFile, fileComments, AuditUserId, AuditWorkStation)) != FileStatuses.READ)
                        return;

                    //Do card validations
                    if((fileHistory.FileStatus = cardValidation.ValidateDuplicatesInFile(bulkRequestsFile, fileComments)) != FileStatuses.READ)
                        return;
                    if ((fileHistory.FileStatus = cardValidation.ValidateReferenceDuplicatesInDB(bulkRequestsFile, fileComments)) != FileStatuses.READ)
                        return;
                    if ((fileHistory.FileStatus = cardValidation.ValidateDuplicatesInDB(bulkRequestsFile, fileComments)) != FileStatuses.READ)
                        return;
                    
                    //Do client specific validations
                    if (_clientValidation != null)
                    {
                        if ((fileHistory.FileStatus = _clientValidation(bulkRequestsFile, fileComments, AuditUserId, AuditWorkStation)) != FileStatuses.READ)
                            return;
                    }

                    //Check if we need to generate references
                    if (_genBulkRequestsCardReferences != null)
                    {
                        //Generate Reference numbers for all the cards
                        log.Debug(d => d("Process Reference Numbers for cards"));
                        _genBulkRequestsCardReferences(bulkRequestsFile);
                    }

                    fileHistory.FileStatus = FileStatuses.VALID_CARDS;

                   //write load batch and cards to DB
                    if (fileHistory.FileStatusId != (int)FileStatuses.VALID_CARDS)
                    {
                        //log.Error("Not all card within card file are valid.");
                        fileComments.Add(new FileCommentsObject("Not all card within card file are valid.", log.Error));
                        return;
                    }

                    //Generate batch reference
                    fileHistory.LoadBatchReference = _fileLoaderBLL.generateBatchReference(fileInfo);
                    fileHistory.FileLoadComments = _fileLoaderBLL.BuildFileComments(fileComments);

                    //Persist to the DB
                    //log.Debug(m => m("Saving load batch to database, batch ref:\t" + fileHistory.LoadBatchReference));
                    fileComments.Add(new FileCommentsObject("Saving load batch to database, batch ref:\t" + fileHistory.LoadBatchReference, log.Info));
                    if (_fileLoaderDAL.CreateBulkRequestLoadBatch(bulkRequestsFile, fileHistory.LoadBatchReference, fileHistory, AuditUserId, AuditWorkStation))
                    {
                        //log.Info("Successfully processed the file.");
                        fileComments.Add(new FileCommentsObject("Successfully processed the file.", log.Info));
                        fileHistory.FileStatus = FileStatuses.PROCESSED;
                    }
                    else
                    {
                        //log.Error("Failed to save load batch to database.");
                        fileComments.Add(new FileCommentsObject("Failed to save load batch to database.", log.Error));
                        fileHistory.FileStatus = FileStatuses.LOAD_FAIL;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    //log.Fatal("Error processing file " + fileInfo.Name + ".", ex);
                    fileComments.Add(new FileCommentsObject("Error processing file " + fileInfo.Name + ".", ex, log.Fatal));
                    fileHistory.FileStatus = FileStatuses.LOAD_FAIL;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                fileHistory.FileStatus = FileStatuses.LOAD_FAIL;
            }
            finally
            {
                base.CommitFileHistory(fileComments, fileHistory);
                base.FileCleanup(fileInfo, parameter.DeleteFile, fileHistory);
            }
        }

        /// <summary>
        /// Reads the card file
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="unencryptedStream"></param>
        /// <param name="fileComments"></param>
        /// <returns></returns>
        private BulkRequestsFile ReadOperation(FileInfo fileInfo, Parameters parameters, ref List<FileCommentsObject> fileComments)
        {
            log.Trace(m => m("Read operation started for file: " + fileInfo.FullName));
            log.Debug(m => m("File Encryption Parameter: " + parameters.FileEncryption));

            //Check if we need to decrypt the file.
            if (parameters.FileEncryption > 0)
            {
                var fileEncryptionType = (FileEncryptionType)parameters.FileEncryption;
                log.Trace(m => m("Attempting to decrypt file using " + fileEncryptionType.ToString()));

                //Get provider based on encryption type and decrypt file
                var provider = FileDecryptionFactory.CreateCryptoProvider(fileEncryptionType);
                using (var unencryptedStream = provider.DecryptFile(fileInfo.OpenRead(), parameters.PrivateKey, parameters.Password))
                {
                    if (unencryptedStream == null)
                        throw new Exception("Failed to decrpt file.");

                    log.Trace(m => m("File decrypted successfully."));

                    //Read the file or stream
                    if (_readFile != null)
                    {
                        log.Debug(m => m("Using delegate " + _readFile.Target.ToString()));
                        log.Trace(m => m("Attempting to write decrypted file."));

                        string decrptedFileName = fileInfo.FullName + DECRYPTED_EXTENTION;
                        //File was decrypted but we need to save it then read it.
                        //Remeber to delete the decrytped file.
                        using (var fileStream = File.Create(decrptedFileName))
                        {
                            log.Debug(m => m("Writing decrypted file to: " + decrptedFileName));

                            try //this is because of BouncyCastle not supporting this.
                            {
                                unencryptedStream.Position = 0;
                            }
                            catch { }

                            unencryptedStream.CopyTo(fileStream);
                            fileStream.Flush();
                        }

                        //Read the file
                        FileInfo decryptedFile = new FileInfo(decrptedFileName);

                        try
                        {
                            return _readFile(decryptedFile, ref fileComments);
                        }
                        finally
                        {
                            log.Trace(m => m("Attempting to delete decrypted file: " + decrptedFileName));
                            FileCleanup(decryptedFile, true, null);
                        }
                    }
                    else if (_readStream != null)
                    {
                        log.Debug(m => m("Using delegate " + _readStream.Target.ToString()));

                        //File was decrypted and we can read it as a stream
                        return _readStream(unencryptedStream, fileInfo.Name, ref fileComments);
                    }
                }
            }

            if (_readStream != null)
            {
                log.Debug(m => m("Using delegate " + _readStream.Target.ToString()));

                //No decryption happened but we need to use a stream
                using (var fileStream = fileInfo.OpenRead())
                    return _readStream(fileStream, fileInfo.Name, ref fileComments);

            }
            else
            {
                log.Debug(m => m("Using delegate " + _readFile.Target.ToString()));

                //Default way to read file.
                return _readFile(fileInfo, ref fileComments);
            }
        }
    }
}
