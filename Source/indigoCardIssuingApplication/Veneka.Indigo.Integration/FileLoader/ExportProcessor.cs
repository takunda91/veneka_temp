using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Integration.FileLoader.Crypto;
using Veneka.Indigo.Integration.FileLoader.Objects;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.Integration.FileLoader
{
    public class ExportProcessor : Processor
    {
        private readonly IExportBatchDAL _exportBatchDAL;

        public delegate bool WriteFile(List<CardObject> cards, FileInfo fileInfo, ref List<FileCommentsObject> fileComments);
        public delegate bool WriteStream(List<CardObject> cards, Stream cardFile, string file, ref List<FileCommentsObject> fileComments);
        private WriteFile _writeFile;
        private WriteStream _writeStream;

        #region Constructors
        private ExportProcessor(string connectionString, IDataSource dataSource, string baseFileDir, string logger, int languageId, long auditUserId, string auditWorkStation)
            : base(connectionString, dataSource, baseFileDir, logger, languageId, auditUserId, auditWorkStation)
        {
            _exportBatchDAL = dataSource.ExportBatchDAL;
        }

        public ExportProcessor(string connectionString, IDataSource dataSource, string baseFileDir, string logger,
                            WriteFile writeFile, int languageId, long auditUserId, string auditWorkStation)
            : this(connectionString, dataSource, baseFileDir, logger, languageId, auditUserId, auditWorkStation)
        {
            _writeFile = writeFile;            
        }

        public ExportProcessor(string connectionString, IDataSource dataSource, string baseFileDir, string logger,
                            WriteStream writeStream, int languageId, long auditUserId, string auditWorkStation)
            : this(connectionString, dataSource, baseFileDir, logger, languageId, auditUserId, auditWorkStation)
        {
            _writeStream = writeStream;
        }
        #endregion

        public void Process(int issuerId, int productId, Parameters parameters, ref List<FileCommentsObject> fileComment)
        {
            //Check if the directory exists.

            //Get the cards
            List<CardObject> allCards = new List<CardObject>();

            //STEP 1: Get export batches for the issuer.
            var exportBatches = _exportBatchDAL.FindExportBatches(issuerId, productId, 1, 0, this.AuditUserId, this.AuditWorkStation);

            if (exportBatches.Count == 0)
            {
                log.Info("No export batches to process.");
                return;
            }

            //STEP 2: Get card details from the batchs, and consolidate them all.
            foreach (var batch in exportBatches)
            {
                allCards.AddRange(_exportBatchDAL.FetchCardObjectsForExportBatch(batch.Key, 0, this.AuditUserId, this.AuditWorkStation));
            }

            //Write the file

            var result = this.WriteOperation(allCards, new FileInfo(""), parameters, ref fileComment);

            //Update Export batches to "EXPORTED" status.
            if (result)
            {
                //update the batches
                foreach (long exportBatchId in exportBatches.Select(s => s.Key))
                {
                    _exportBatchDAL.StatusChangeExported(exportBatchId, 0, this.AuditUserId, this.AuditWorkStation);
                }
            }

            //do any cleanup
        }

        private bool WriteOperation(List<CardObject> cards, FileInfo fileInfo, Parameters parameters, ref List<FileCommentsObject> fileComments)
        {
            log.Trace(m => m("Write operation started for file: " + fileInfo.FullName));
            log.Debug(m => m("File Encryption Parameter: " + parameters.FileEncryption));

            //Check if we need to decrypt the file.
            if (parameters.FileEncryption > 1)
            {
                var fileEncryptionType = (FileEncryptionType)parameters.FileEncryption;
                log.Trace(m => m("Attempting to encrypt file using " + fileEncryptionType.ToString()));

                //Get provider based on encryption type and decrypt file
                var provider = FileDecryptionFactory.CreateCryptoProvider(fileEncryptionType);
                using (var unencryptedStream = provider.DecryptFile(fileInfo.OpenRead(), parameters.PrivateKey, parameters.Password))
                {
                    if (unencryptedStream == null)
                        throw new Exception("Failed to decrpt file.");

                    log.Trace(m => m("File decrypted successfully."));

                    //Read the file or stream
                    if (_writeFile != null)
                    {
                        log.Debug(m => m("Using delegate " + _writeFile.Target.ToString()));
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
                            return _writeFile(cards, decryptedFile, ref fileComments);
                        }
                        finally
                        {
                            log.Trace(m => m("Attempting to delete decrypted file: " + decrptedFileName));
                            FileCleanup(decryptedFile, true, null);
                        }
                    }
                    else if (_writeStream != null)
                    {
                        log.Debug(m => m("Using delegate " + _writeStream.Target.ToString()));

                        //File was decrypted and we can read it as a stream
                        return _writeStream(cards, unencryptedStream, fileInfo.Name, ref fileComments);
                    }
                }
            }

            if (_writeStream != null)
            {
                log.Debug(m => m("Using delegate " + _writeStream.Target.ToString()));

                //No decryption happened but we need to use a stream
                using (var fileStream = fileInfo.OpenRead())
                    return _writeStream(cards, fileStream, fileInfo.Name, ref fileComments);

            }
            else
            {
                log.Debug(m => m("Using delegate " + _writeFile.Target.ToString()));

                //Default way to read file.
                return _writeFile(cards, fileInfo, ref fileComments);
            }
        }
    }
}
