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
    public class FileProcessor : Processor
    {
        public delegate CardFile ReadFile(FileInfo fileInfo, ref List<FileCommentsObject> fileComments);
        public delegate CardFile ReadStream(Stream cardFile, string filename, ref List<FileCommentsObject> fileComments);
        public delegate FileStatuses ClientValidation(CardFile cardFile, List<FileCommentsObject> fileComments, long auditUserId, string auditWorkstation);
        public delegate void GenerateCardReferences(CardFile cardFile);
        public delegate void WriteFile(FileInfo fileInfo);
        public delegate void WriteFile2(FileInfo originalFile, FileInfo processedDir);
        private ReadFile _readCardFile;
        private ReadStream _readCardStream;
        private ClientValidation _clientValidation;
        private GenerateCardReferences _genCardReferences;
        private WriteFile _writeFile;
        private WriteFile2 _writeFile2;


        /// <summary>
        /// This contrsuctor accepts a delegate of ReadFile
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="baseFileDir"></param>
        /// <param name="logger"></param>
        /// <param name="readFile"></param>
        /// <param name="clientValidation"></param>
        /// <param name="generateCardRefs"></param>
        /// <param name="writeFile"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        public FileProcessor(string connectionString, IDataSource dataSource, string baseFileDir, string logger,
                            ReadFile readFile, ClientValidation clientValidation,
                             GenerateCardReferences generateCardRefs, WriteFile writeFile,
                            int languageId, long auditUserId, string auditWorkStation)
            : base(connectionString, dataSource, baseFileDir, logger, languageId, auditUserId, auditWorkStation)
        {
            _readCardFile = readFile;
            _clientValidation = clientValidation;
            _genCardReferences = generateCardRefs;
            _writeFile = writeFile;
        }

        public FileProcessor(string connectionString, IDataSource dataSource, string baseFileDir, string logger,
                            ReadFile readFile, ClientValidation clientValidation,
                             GenerateCardReferences generateCardRefs, WriteFile2 writeFile2,
                            int languageId, long auditUserId, string auditWorkStation)
            : base(connectionString, dataSource, baseFileDir, logger, languageId, auditUserId, auditWorkStation)
        {
            _readCardFile = readFile;
            _clientValidation = clientValidation;
            _genCardReferences = generateCardRefs;
            _writeFile2 = writeFile2;
        }

        /// <summary>
        /// This contrsuctor accepts a delegate of ReadStream
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="baseFileDir"></param>
        /// <param name="logger"></param>
        /// <param name="readStream"></param>
        /// <param name="clientValidation"></param>
        /// <param name="generateCardRefs"></param>
        /// <param name="writeFile"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkStation"></param>
        public FileProcessor(string connectionString, IDataSource dataSource, string baseFileDir, string logger,
                            ReadStream readStream, ClientValidation clientValidation,
                             GenerateCardReferences generateCardRefs, WriteFile writeFile,
                            int languageId, long auditUserId, string auditWorkStation)
            : base(connectionString, dataSource, baseFileDir, logger, languageId, auditUserId, auditWorkStation)
        {
            _readCardStream = readStream;
            _clientValidation = clientValidation;
            _genCardReferences = generateCardRefs;
            _writeFile = writeFile;
        }

        public void ProcessFiles(string cardsFileLocation, List<Parameters> parameters)
        {
            int fileLoadId = 0;

            try
            {
                //Create file_load for this file load session
                DateTimeOffset startTime = DateTimeOffset.Now;

                //get all files from issuer directory
                if (Directory.Exists(cardsFileLocation))
                {
                    var dirInfo = new DirectoryInfo(cardsFileLocation);
                    fileLoadId = _fileLoaderDAL.CreateFileLoad(startTime, dirInfo.GetFiles().Count(), AuditUserId, AuditWorkStation);
                    var param = parameters.Where(p => p.Path.Equals(cardsFileLocation)).FirstOrDefault();

                    //log.Debug(m => m("Reading Encrypted Files from " + cardsFileLocation + "..."));

                    foreach (FileInfo fileInfo in dirInfo.GetFiles())
                    {
                        ProcessFile(fileInfo, fileLoadId, param);
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

        private void ProcessFile(FileInfo fileInfo, int fileLoadId, Parameters parameters)
        {
            List<FileCommentsObject> fileComments = new List<FileCommentsObject>();
            licensedBinCodes.Clear();
            bool multiProductFile = false;
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

            try //Catch exception per file so proccessing of each file may still happen and not stop on a single file exceptin
            {
                try
                {
                    //Read all records from the file and decode into CardFileRecord Object.                                                                
                    CardFile cardFile;

                    try
                    {
                        cardFile = ReadOperation(fileInfo, parameters, ref fileComments);

                        if (cardFile == null)
                            throw new Exception("cardFile object is null.");
                    }
                    catch (Exception ex)
                    {
                        fileComments.Add(new FileCommentsObject("Failed to read file.", ex, log.Error));
                        fileHistory.FileStatus = FileStatuses.CARD_FILE_INFO_READ_ERROR;
                        return;
                    }

                    log.DebugFormat("Issuer code for file: {0}", cardFile.IssuerCode);

                    // If the issuer code was supplied load the issuer for all records.
                    if (!String.IsNullOrWhiteSpace(cardFile.IssuerCode) && cardFile.FileIssuer == null)
                    {
                        log.Debug("Trying to lookup issuer");
                        Issuer issuer;
                        if (_fileLoaderBLL.TryIssuerLookup(cardFile.IssuerCode.Trim(), out issuer, AuditUserId, AuditWorkStation))
                        {
                            log.DebugFormat("Issuer found: {0} {1}", issuer.IssuerName, issuer.IssuerId);
                            cardFile.FileIssuer = issuer;

                            foreach (var record in cardFile.CardFileRecords)
                            {
                                record.IssuerId = issuer.IssuerId;

                                log.DebugFormat("Card: {0} {1}", record.PAN, record.IssuerId);
                            }
                        }
                        else
                        {
                            fileComments.Add(new FileCommentsObject(String.Format("Issuer with code {0} not found.", cardFile.IssuerCode.Trim()), log.Error));
                            fileHistory.FileStatus = FileStatuses.ISSUER_NOT_FOUND;
                            return;
                        }
                    }

                    //Do file validations                    
                    if ((fileHistory.FileStatus = fileValidation.Validate(parameters, fileInfo, cardFile, fileComments, AuditUserId, AuditWorkStation)) != FileStatuses.READ)
                        return;

                    //Do product validations
                    productValidation.Clear();
                    if ((fileHistory.FileStatus = productValidation.ValidateCardFile(cardFile, fileComments, AuditUserId, AuditWorkStation)) != FileStatuses.READ)
                        return;

                    //Do issuer validations
                    issuerValidation.Clear();
                    if ((fileHistory.FileStatus = issuerValidation.ValidateCardFile(cardFile, fileComments, AuditUserId, AuditWorkStation)) != FileStatuses.READ)
                        return;

                    //Do branch valaidations
                    // Remember to clear out branch code or it might pick up a different issues branch code from cache
                    branchValidation.Clear();
                    if ((fileHistory.FileStatus = branchValidation.ValidateCardFile(cardFile, fileComments, AuditUserId, AuditWorkStation)) != FileStatuses.READ)
                        return;

                    //Do card validations
                    if ((fileHistory.FileStatus = cardValidation.ValidateSingleProductInFile(cardFile, fileComments)) != FileStatuses.READ)
                        return;
                    if (cardFile.OrderBatchId.HasValue && cardFile.OrderBatchId.Value == -99)
                    {
                        cardFile.OrderBatchId = null;
                        multiProductFile = true;
                    }


                    if ((fileHistory.FileStatus = cardValidation.ValidateDuplicatesInFile(cardFile, fileComments)) != FileStatuses.READ)
                        return;

                    List<CardFile> splitFile = new List<CardFile>();

                    List<int> productIds = new List<int>();
                    if (multiProductFile == true)
                    {
                        var results = from p in cardFile.CardFileRecords
                                      group p.CardId by p.ProductId into g
                                      select new { ProductId = g.Key };
                        productIds.AddRange(results.Select(p => p.ProductId.GetValueOrDefault()).ToList());
                    }

                    if (multiProductFile == true && productIds.Count > 1)
                    {
                        //Generate batch reference
                        log.Trace(d => d("Multiple Product Split : START"));
                        log.Trace(d => d("Generate batch references for multiple products: START"));
                        fileHistory.LoadBatchReference = _fileLoaderBLL.generateBatchReference(fileInfo);
                        foreach (var productId in productIds)
                        {
                            List<CardFileRecord> forProduct = cardFile.CardFileRecords.Where(p => p.ProductId == productId).ToList();
                            CardFile subFile = new CardFile(cardFile.IssuerCode, cardFile.FileIdentifier, forProduct)
                            {
                                FileIdentifier = cardFile.FileIdentifier,
                                FileIssuer = cardFile.FileIssuer,
                                IssuerCode = cardFile.IssuerCode,
                                LoadBatchReference = $"{fileHistory.LoadBatchReference}_{productId}",
                                OrderBatchId = cardFile.OrderBatchId,
                                OrderBatchreference = cardFile.OrderBatchreference
                            };

                            log.Debug(d => d("Load batch reference - for sub-file: " + subFile.LoadBatchReference));
                            splitFile.Add(subFile);
                        }
                        log.Trace(d => d("Generate batch reference - for multiple products: DONE"));
                    }
                    else
                    {
                        //Generate batch reference
                        log.Trace(d => d("Generate batch reference: START"));
                        cardFile.LoadBatchReference =
                        fileHistory.LoadBatchReference = _fileLoaderBLL.generateBatchReference(fileInfo);
                        log.Debug(d => d("Load batch reference: " + fileHistory.LoadBatchReference));
                        log.Trace(d => d("Generate batch reference: DONE"));
                        splitFile.Add(cardFile);
                    }

                    foreach (var splitCardFile in splitFile)
                    {

                        log.Debug("ProductLoadTypeId=" + splitCardFile.CardFileRecords[0].ProductLoadTypeId + ", CardId=" + splitCardFile.CardFileRecords[0].CardId);
                        //If load to exsiting and card ID is null, try find ordered cards based on product and number of cards
                        //Card id will be null if its not read from the file
                        if (splitCardFile.CardFileRecords[0].ProductLoadTypeId == 4 && splitCardFile.CardFileRecords[0].CardId == null)
                        {
                            log.Debug("Cards missing card ID, looking for ordered batch.");
                            LoadToExisting lte = new LoadToExisting(this._connectionString);
                            if (!lte.FindMatchingOrder(cardFile, AuditUserId, AuditWorkStation))
                            {
                                fileComments.Add(new FileCommentsObject("No order with matching product and card size found.", log.Error));
                                fileHistory.FileStatus = FileStatuses.CARDS_NOT_ORDERED;
                                return;
                            }
                            log.Debug("Successfully completed looking up ordered batch.");
                        }

                        if ((fileHistory.FileStatus = cardValidation.ValidateDuplicatesInDB(splitCardFile, fileComments)) != FileStatuses.READ)
                            return;
                        if ((fileHistory.FileStatus = cardValidation.ValidateCardsOrdered(splitCardFile, fileComments)) != FileStatuses.READ)
                            return;

                        //Do client specific validations
                        if (_clientValidation != null)
                        {
                            log.Debug(m => m("Client validation delegate " + _clientValidation.Target.ToString()));
                            if ((fileHistory.FileStatus = _clientValidation(splitCardFile, fileComments, AuditUserId, AuditWorkStation)) != FileStatuses.READ)
                                return;
                        }

                        //Check if we need to generate references
                        if (_genCardReferences != null)
                        {
                            //Generate Reference numbers for all the cards
                            log.Trace(d => d("Process Card Reference Numbers for cards: START"));
                            log.Debug(m => m("Card Reference Numbers delegate " + _genCardReferences.Target.ToString()));
                            _genCardReferences(cardFile);
                            log.Trace(d => d("Process Card Reference Numbers for cards: DONE"));
                        }

                        //All validation complete, change status to valid cards
                        fileHistory.FileStatus = FileStatuses.VALID_CARDS;
                    }

                    foreach (var splitCardFile in splitFile)
                    {
                        //Persist to the DB
                        long? loadBatchId;
                        fileComments.Add(new FileCommentsObject("Saving load batch to database, batch ref:\t" + fileHistory.LoadBatchReference, log.Info));
                        fileHistory.FileLoadComments = _fileLoaderBLL.BuildFileComments(fileComments);
                        if (_fileLoaderDAL.CreateLoadBatch(splitCardFile, splitCardFile.LoadBatchReference, fileHistory, AuditUserId, AuditWorkStation, out loadBatchId))
                        {
                            fileComments.Add(new FileCommentsObject(String.Format("Successfully processed file {0}.", fileInfo.Name), log.Info));
                            fileHistory.FileStatus = FileStatuses.PROCESSED;
                            PostProcessing(fileInfo, fileComments, fileHistory);
                        }
                        else
                        {
                            //log.Error("Failed to save load batch to database.");
                            fileComments.Add(new FileCommentsObject("Failed to save load batch to database.", log.Error));
                            fileHistory.FileStatus = FileStatuses.LOAD_FAIL;
                            return;
                        }
                    }

                }
                catch (Exception ex)
                {
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
                base.FileCleanup(fileInfo, parameters.DeleteFile, fileHistory);
            }
        }



        /// <summary>
        /// Reads the card file
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="unencryptedStream"></param>
        /// <param name="fileComments"></param>
        /// <returns></returns>
        private CardFile ReadOperation(FileInfo fileInfo, Parameters parameters, ref List<FileCommentsObject> fileComments)
        {
            log.Trace(m => m("Read operation started for file: " + fileInfo.FullName));
            log.Debug(m => m("File Encryption Parameter: " + parameters.FileEncryption));

            //Check if we need to decrypt the file.
            if (parameters.FileEncryption > 0)
            {
                var fileEncryptionType = (FileEncryptionType)parameters.FileEncryption;
                fileComments.Add(new FileCommentsObject(String.Format("Attempting to decrypt file using {0}.", fileEncryptionType.ToString()), log.Info));

                //Get provider based on encryption type and decrypt file
                var provider = FileDecryptionFactory.CreateCryptoProvider(fileEncryptionType);
                using (var fileOpenStream = fileInfo.OpenRead())
                {
                    using (var unencryptedStream = provider.DecryptFile(fileOpenStream, parameters.PrivateKey, parameters.Password))
                    {
                        if (unencryptedStream == null)
                            throw new Exception("Failed to decrpt file.");

                        fileComments.Add(new FileCommentsObject("File decrypted successfully.", log.Info));

                        //Read the file or stream
                        if (_readCardFile != null)
                        {
                            log.Debug(m => m("Using delegate " + _readCardFile.Target.ToString()));
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
                                return _readCardFile(decryptedFile, ref fileComments);
                            }
                            finally
                            {
                                log.Trace(m => m("Attempting to delete decrypted file: " + decrptedFileName));
                                FileCleanup(decryptedFile, true, null);
                            }
                        }
                        else if (_readCardStream != null)
                        {
                            log.Debug(m => m("Using delegate " + _readCardStream.Target.ToString()));

                            //File was decrypted and we can read it as a stream
                            return _readCardStream(unencryptedStream, fileInfo.Name, ref fileComments);
                        }
                    }
                }
            }

            if (_readCardStream != null)
            {
                log.Debug(m => m("Using delegate " + _readCardStream.Target.ToString()));

                //No decryption happened but we need to use a stream
                using (var fileStream = fileInfo.OpenRead())
                    return _readCardStream(fileStream, fileInfo.Name, ref fileComments);

            }
            else
            {
                log.Debug(m => m("Using delegate " + _readCardFile.Target.ToString()));

                //Default way to read file.
                return _readCardFile(fileInfo, ref fileComments);
            }
        }

        /// <summary>
        /// Perform any post processing after the load batch has been loaded successfully.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="fileComments"></param>
        /// <param name="fileHistory"></param>
        private void PostProcessing(FileInfo fileInfo, List<FileCommentsObject> fileComments, FileHistory fileHistory)
        {
            if (_writeFile != null)
            {
                log.Debug(m => m("Post Processing delegate " + _writeFile.Target.ToString()));

                FileInfo processedFileInfo = new FileInfo(fileInfo.DirectoryName + @"\PROCESSED\" + fileInfo.Name);

                if (!Directory.Exists(processedFileInfo.DirectoryName))
                    Directory.CreateDirectory(processedFileInfo.DirectoryName);

                fileComments.Add(new FileCommentsObject("Writing file to:\t" + processedFileInfo.FullName, log.Info));
                _writeFile(processedFileInfo);
                fileComments.Add(new FileCommentsObject("Post Processing complete, file written to:\t" + processedFileInfo.FullName, log.Info));
            }
            else if (_writeFile2 != null)
            {
                log.Debug(m => m("Post Processing delegate " + _writeFile2.Target.ToString()));

                FileInfo processedFileInfo = new FileInfo(fileInfo.DirectoryName + @"\PROCESSED\" + fileInfo.Name);

                if (!Directory.Exists(processedFileInfo.DirectoryName))
                    Directory.CreateDirectory(processedFileInfo.DirectoryName);

                fileComments.Add(new FileCommentsObject("Writing file to:\t" + processedFileInfo.FullName, log.Info));
                _writeFile2(fileInfo, processedFileInfo);
                fileComments.Add(new FileCommentsObject("Post Processing complete, file written to:\t" + processedFileInfo.FullName, log.Info));
            }
            else
            {
                log.Debug(m => m("No Post Processing delegate "));
            }
        }
    }
}