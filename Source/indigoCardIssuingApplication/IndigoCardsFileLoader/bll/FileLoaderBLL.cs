using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Common.Logging;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Models;
using IndigoFileLoader;
using IndigoFileLoader.dal;
using IndigoFileLoader.objects;
using IndigoFileLoader.Modules;
using IndigoFileLoader.Modules.Extensibility;
using Veneka.Indigo.IssuerManagement;

namespace IndigoFileLoader.bll
{
    class FileLoaderBLL
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FileLoaderBLL));
        private FileLoaderDAL fileLoaderDAL = new FileLoaderDAL();
        private List<string> licensedBinCodes = new List<string>();
        private string cardsFileLocation;

        public FileLoaderBLL()
        {
            cardsFileLocation = Path.Combine(SystemConfiguration.Instance.GetBaseConfigDir(), @"card_files\");
        }

        /// <summary>
        /// Load all card files within the directory.
        /// </summary>
        public void LoadCardFile()
        {
            int fileLoadId = 0;

            try
            {
                //Create file_load for this file load session
                DateTimeOffset startTime = DateTimeOffset.Now;

                //get all files from issuer directory
                if (Directory.Exists(cardsFileLocation))
                {
                    var dirissuerInfo = new DirectoryInfo(cardsFileLocation);
                    fileLoadId = fileLoaderDAL.CreateFileLoad(startTime, dirissuerInfo.GetFiles().Count());


                    log.Debug(m => m("Reading Encrypted Files from " + cardsFileLocation + "..."));

                    foreach (FileInfo encryptedFileInfo in dirissuerInfo.GetFiles())
                    {
                        List<FileCommentsObject> fileComments = new List<FileCommentsObject>();
                        licensedBinCodes.Clear();

                        log.Debug(m => m("Processing file: " + encryptedFileInfo.Name));
                        fileComments.Add(new FileCommentsObject("Processing file: " + encryptedFileInfo.Name));

                        //Build file_history object.
                        file_history fileHistory = new file_history();
                        fileHistory.file_load_id = fileLoadId;
                        fileHistory.file_types = new file_types();
                        fileHistory.file_statuses = new file_statuses();
                        fileHistory.file_status_id = (int)FileStatus.READ;
                        fileHistory.name_of_file = encryptedFileInfo.Name;
                        fileHistory.file_directory = encryptedFileInfo.Directory.ToString();
                        fileHistory.file_size = (int)encryptedFileInfo.Length;
                        fileHistory.file_created_date = DateTime.SpecifyKind(encryptedFileInfo.CreationTime, DateTimeKind.Local);

                        try //Catch exception per file so proccessing of each file may still happen and not stop on a single file exceptin
                        {
                            FileInfo fileInfo;

                            FileStatus fstatus = DecryptPgpFile(cardsFileLocation, encryptedFileInfo, out fileInfo, ref fileComments);
                            if (fstatus != FileStatus.READ)
                            {
                                fileHistory.file_status_id = (int)fstatus;
                                continue;
                            }

                            //Now process the unencrypted file.
                            log.Debug(m => m(string.Format("Start processing decrypted file:\t{0}", fileInfo.Name)));
                            fileComments.Add(new FileCommentsObject(string.Format("Start processing decrypted file:\t{0}", fileInfo.Name)));

                            try
                            {
                                //Update file history Info with new decrypted file info.
                                fileHistory.name_of_file = fileInfo.Name;
                                fileHistory.file_directory = fileInfo.Directory.ToString();
                                fileHistory.file_size = (int)fileInfo.Length;
                                fileHistory.file_types.file_type_id = (int)FileType.CARD_IMPORT;
                                fileHistory.file_created_date = DateTime.SpecifyKind(fileInfo.CreationTime, DateTimeKind.Local); ;


                                //read all records from the file and decode into CardFileRecord Object.                                                                
                                List<CardFileRecord> cardFileRecords;
                                if (!getCardRecords(fileInfo, out cardFileRecords, ref fileComments))
                                {
                                    fileHistory.file_status_id = (int)FileStatus.CARD_FILE_INFO_READ_ERROR;
                                    continue;
                                }


                                //Find Issuer Based on BIN code Branch
                                log.Debug(m => m("Finding Issuer based on PAN: " + cardFileRecords[0].PsuedoPAN + " BIN: " + cardFileRecords[0].PAN.Substring(0, 9) + " BRANCHCODE: " + cardFileRecords[0].BranchCode));
                                fileComments.Add(new FileCommentsObject("Finding Issuer based on PAN: " + cardFileRecords[0].PsuedoPAN + " BIN: " + cardFileRecords[0].PAN.Substring(0, 9) + " BRANCHCODE: " + cardFileRecords[0].BranchCode));
                                issuer issuer;
                                if (!fileLoaderDAL.FetchIssuerByProductAndBinCode(cardFileRecords[0].PAN, cardFileRecords[0].BranchCode, out issuer))
                                {
                                    log.Error("Issuer and/or Product not correctly setup.");
                                    fileComments.Add(new FileCommentsObject("Issuer and/or Product not correctly setup."));
                                    fileHistory.file_status_id = (int)FileStatus.BRANCH_PRODUCT_NOT_FOUND;
                                    continue;
                                }
                                fileHistory.issuer_id = issuer.issuer_id;

                                if (issuer == null || !issuer.instant_card_issue_YN)
                                {
                                    log.Error("Issuer has not be setup for instant card issuing.");
                                    fileComments.Add(new FileCommentsObject("Issuer has not be setup for instant card issuing."));
                                    fileHistory.file_status_id = (int)FileStatus.LOAD_FAIL;
                                    continue;
                                }

                                //Check if issuer is licenced
                                fileComments.Add(new FileCommentsObject("Checking issuer licence."));
                                log.Debug(m => m("Checking issuer licence."));
                                if (String.IsNullOrWhiteSpace(issuer.license_key))
                                {
                                    log.Error("Unlicenced issuer.");
                                    fileComments.Add(new FileCommentsObject("Unlicenced issuer."));
                                    fileHistory.file_status_id = (int)FileStatus.UNLICENSED_ISSUER;
                                    continue;
                                }


                                //check if the file has already been loaded for an active batch
                                log.Debug(m => m("Checking for duplicate file in database."));
                                fileComments.Add(new FileCommentsObject("Checking for duplicate file in database."));
                                if (CheckIfDuplicateFile(fileInfo.Name))
                                {
                                    fileHistory.file_status_id = (int)FileStatus.DUPLICATE_FILE;
                                    log.Error("Duplicate file with name " + fileInfo.Name + ", skipping file.");
                                    fileComments.Add(new FileCommentsObject("Duplicate file with name " + fileInfo.Name + ", skipping file."));
                                    continue;
                                }

                                //Validate Key, and get valid bins.
                                log.Debug(m => m("Getting licensed bins."));
                                fileComments.Add(new FileCommentsObject("Getting licensed bins."));
                                licensedBinCodes = Veneka.Indigo.Common.License.LicenseManager.ValidateAffiliateKey(issuer.license_key);

                                if (licensedBinCodes == null || licensedBinCodes.Count == 0)
                                {
                                    log.Error("No licensed bins found for this issuer.");
                                    fileComments.Add(new FileCommentsObject("No licensed bins found for this issuer."));
                                    fileHistory.file_status_id = (int)FileStatus.INVALID_ISSUER_LICENSE;
                                    continue;
                                }

                                //Validate all card records
                                log.Debug(m => m("Validating cards."));
                                fileComments.Add(new FileCommentsObject("Validating cards."));
                                Dictionary<string, CardFileRecord> validCards = new Dictionary<string, CardFileRecord>();
                                fileHistory.file_status_id = (int)validateCardRecords(cardFileRecords, issuer.issuer_id, out validCards, ref fileComments);
                                log.Debug(m => m("Valid cards count: " + validCards.Count));
                                fileComments.Add(new FileCommentsObject("Valid cards count: " + validCards.Count));


                                //write load batch and cards to DB
                                if (fileHistory.file_status_id != (int)FileStatus.VALID_CARDS)
                                {
                                    log.Error("Not all card within card file are valid.");
                                    fileComments.Add(new FileCommentsObject("Not all card within card file are valid."));
                                    continue;
                                }

                                //Generate batch reference
                                string loadBatchReference = generateBatchReference(fileInfo);
                                fileHistory.file_status_id = (int)FileStatus.PROCESSED;
                                fileHistory.file_load_comments = BuildFileComments(fileComments);


                                //Persist to the DB
                                log.Debug(m => m("Saving load batch to database, batch ref:\t" + loadBatchReference));
                                fileComments.Add(new FileCommentsObject("Saving load batch to database, batch ref:\t" + loadBatchReference));
                                if (fileLoaderDAL.CreateLoadBatch(validCards, loadBatchReference, issuer.issuer_id, fileHistory))
                                {
                                    log.Info("Successfully processed the file.");
                                    fileComments.Add(new FileCommentsObject("Successfully processed the file."));
                                    fileHistory.file_status_id = (int)FileStatus.PROCESSED;
                                }
                                else
                                {
                                    log.Error("Failed to save load batch to database.");
                                    fileComments.Add(new FileCommentsObject("Failed to save load batch to database."));
                                    fileHistory.file_status_id = (int)FileStatus.LOAD_FAIL;
                                    continue;
                                }

                                //if (issuer.delete_card_file_YN)
                                //{
                                //    File.Delete(fileInfo.FullName);
                                //}
                            }
                            catch (Exception ex)
                            {
                                log.Fatal("Error processing file " + fileInfo.Name + ".", ex);
                                fileComments.Add(new FileCommentsObject("Error processing file:\t" + ex.Message));
                                fileHistory.file_status_id = (int)FileStatus.LOAD_FAIL;
                            }
                            finally
                            {
                                //Delete unencrypted file!!! 
                                fileInfo.Delete();

                                //archive the file
                                ArchiveFiles((FileStatus)fileHistory.file_status_id, encryptedFileInfo);
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                            fileHistory.file_status_id = (int)FileStatus.LOAD_FAIL;
                        }
                        finally
                        {
                            fileHistory.file_load_comments = BuildFileComments(fileComments);

                            //Write FileHistory to DB.
                            if (fileHistory.file_status_id != (int)FileStatus.PROCESSED)
                            {
                                fileLoaderDAL.SaveFileInfo(fileHistory);
                            }

                        }
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
                    fileLoaderDAL.UpdateFileLoad(fileLoadId);
            }
        }

        private string BuildFileComments(List<FileCommentsObject> fileComments)
        {
            StringBuilder commentsBuilder = new StringBuilder();

            foreach (var comment in fileComments)
            {
                commentsBuilder.AppendLine(comment.GetFormatedComment());
            }

            return commentsBuilder.ToString();
        }

        /// <summary>
        /// Decryptes an encrypted pgp file.
        /// </summary>
        /// <param name="cardsFileLocation"></param>
        /// <param name="encryptedFileInfo"></param>
        /// <param name="decryptedFileInfo"></param>
        /// <param name="fileComments"></param>
        /// <returns></returns>
        private FileStatus DecryptPgpFile(string cardsFileLocation, FileInfo encryptedFileInfo, out FileInfo decryptedFileInfo, ref List<FileCommentsObject> fileComments)
        {
            decryptedFileInfo = null;
            string outputdirectory = Path.Combine(cardsFileLocation, "decrypted_files");
            //string outputfilename = Path.GetFileNameWithoutExtension(encryptedFileInfo.FullName).Replace(".gpg", "") + "_decrypted.OUT";

            string outputfilename = encryptedFileInfo.Name.Replace(".gpg", "");

            if (!Directory.Exists(outputdirectory))
            {
                Directory.CreateDirectory(outputdirectory);

                string comment = "Created decrypted file output directory:\t" + outputdirectory;
                fileComments.Add(new FileCommentsObject(comment));
                log.Debug(m => m(comment));
            }

            fileComments.Add(new FileCommentsObject("Start Decryption of file."));
            log.Debug(m => m("Start Decryption of file."));

            //Decrypted PGP file.
            FileStream fs = File.Open(encryptedFileInfo.FullName, FileMode.Open);
            try
            {
                CryptoHelper.DecryptPgpData(fs, outputdirectory + "\\" + outputfilename);

                fileComments.Add(new FileCommentsObject("Successfully Decrypted file:\t" + outputfilename));
                log.Debug(m => m("Successfully Decrypted file:\t" + outputfilename));

                decryptedFileInfo = new FileInfo(Path.Combine(outputdirectory, outputfilename));
                return FileStatus.READ;
            }
            catch (Exception ex)
            {
                fileComments.Add(new FileCommentsObject("Failed to Decryped file:\t" + outputfilename + " Exception: " + ex.Message));
                log.Debug(m => m("Failed to Decryped file:\t" + outputfilename));
                log.Error(ex);
            }
            finally
            {
                fs.Close();
            }

            return FileStatus.FILE_DECRYPTION_FAILED;
        }

        /// <summary>
        /// Validates a list of card records.
        /// </summary>
        /// <param name="cardFileRecords"></param>
        /// <param name="issuerId"></param>
        /// <returns></returns>
        private FileStatus validateCardRecords(List<CardFileRecord> cardFileRecords, int issuerId, out Dictionary<string, CardFileRecord> validCardRecords, ref List<FileCommentsObject> fileComments)
        {
            validCardRecords = new Dictionary<string, CardFileRecord>();
            Dictionary<string, CardFileRecord> validCards = new Dictionary<string, CardFileRecord>(); //<CardNumber, CardFileRecord>            
            Dictionary<string, BranchLookup> validBranches = new Dictionary<string, BranchLookup>(); //<BranchCode, BranchLookup>
            List<issuer_product> validProducts = new List<issuer_product>();
            BranchService branchService = new BranchService();

            //get a list of valid bins

            #region Loop through all the cards, checking valid branchs, Valid BIN's and duplicates in the file
            foreach (CardFileRecord cardFileRecord in cardFileRecords)
            {
                BranchLookup branchLookup;
                //See if we've already checked the DB for the branch. If it's not in the dictionary then do a read from the DB.                  
                if (!validBranches.TryGetValue(cardFileRecord.BranchCode.Trim().ToUpper(), out branchLookup))
                {
                    //check if branch exists in the DB                                            
                    branch Branch = branchService.GetBranchByBranchCode(issuerId, cardFileRecord.BranchCode.Trim().ToUpper(), BranchStatus.ACTIVE);
                    branchLookup = new BranchLookup(cardFileRecord.BranchCode.Trim().ToUpper(), 0, false);

                    if (Branch != null)
                    {
                        branchLookup = new BranchLookup(Branch.branch_code.Trim().ToUpper(), Branch.branch_id, true);
                    }

                    validBranches.Add(cardFileRecord.BranchCode.Trim().ToUpper(), branchLookup);
                }


                //check if branch exist
                //if not, add a record to rejected file
                if (!branchLookup.IsValid)
                {
                    log.Error("No Valid Branch found in Indigo:\t" + branchLookup.BranchCode);
                    fileComments.Add(new FileCommentsObject("No Valid Branch found in Indigo:\t" + branchLookup.BranchCode));
                    return FileStatus.NO_ACTIVE_BRANCH_FOUND;
                }
                else
                {
                    //Check if the card has been added to the dictionary already, if it has dont add it again.
                    if (validCards.ContainsKey(cardFileRecord.PsuedoPAN.Trim()))
                    {
                        fileComments.Add(new FileCommentsObject("Duplicate card found in card file:\t" + cardFileRecord.PsuedoPAN));
                        log.Error("Duplicate card found in card file:\t" + cardFileRecord.PsuedoPAN);
                        return FileStatus.DUPLICATE_CARDS_IN_FILE;
                    }
                    else
                    {
                        if (CheckIfIsLicenseCardRecord(cardFileRecord.PAN.Trim()))
                        {
                            //See if we can link this card to a product
                            int lookupTries = 0;
                            int productId = -1;

                            do
                            {
                                int charCount = 0;
                                foreach (var product in validProducts)
                                {
                                    //Check the actual PAN, not PSUDO pan... Product bin can be up to 9 characters.
                                    if (cardFileRecord.PAN.Trim().StartsWith(product.product_bin_code))
                                    {
                                        //Need to find the bincode with the least amount of characters, as this result may have more than one result.
                                        //E.G. Card number could be 123456-88771199 but the result may contain a record for products with:
                                        // 123456 and 123456999 becuse the search is only on the first 6 characters.
                                        if (product.product_bin_code.Length > charCount)
                                        {
                                            charCount = product.product_bin_code.Length;
                                            productId = product.product_id;
                                        }
                                    }
                                }

                                if (productId == -1)//Call the DB to see if we can get valid product for the user
                                {
                                    var result = fileLoaderDAL.FetchIssuerProduct(cardFileRecord.PsuedoPAN, branchLookup.BranchId);
                                    validProducts.AddRange(result);
                                }

                                lookupTries++;
                            }
                            while (productId == -1 && lookupTries < 3);

                            if (productId > -1)
                            {
                                cardFileRecord.BranchId = branchLookup.BranchId;
                                cardFileRecord.ProductId = productId;
                                validCards.Add(cardFileRecord.PsuedoPAN.Trim(), cardFileRecord);
                            }
                            else
                            {
                                //TODO; could not find product for card.
                                //throw new Exception("Could not find product for card.");
                                log.Error("Could not find product for card.");
                                fileComments.Add(new FileCommentsObject("Could not find product for card."));
                                return FileStatus.NO_PRODUCT_FOUND_FOR_CARD;
                            }
                        }
                        else
                        {
                            log.Error("Unlicensed BIN in card file.");
                            fileComments.Add(new FileCommentsObject("Unlicensed BIN in card file."));
                            return FileStatus.UNLICENSED_BIN;
                        }
                    }
                }
            }
            #endregion

            #region check card records for duplication in indigo
            //check if card record is in the DB
            List<string> results = fileLoaderDAL.ValidateCardsLoaded(new List<string>(validCards.Keys));

            foreach (string result in results)
            {
                CardFileRecord value;
                //Because the results coming back are based on the keys, the dictionary should have this key, but checking just incase
                if (validCards.TryGetValue(result.Trim(), out value))
                {
                    //card is already in DB
                    validCards.Remove(result.Trim());
                    log.Error("Duplicate cards in database.");
                    fileComments.Add(new FileCommentsObject("Duplicate cards in database."));
                    return FileStatus.DUPLICATE_CARDS_IN_DATABASE;
                }
            }
            #endregion

            validCardRecords = validCards;
            return FileStatus.VALID_CARDS;
        }

        /// <summary>
        /// Check the DB if the file has already been loaded.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool CheckIfDuplicateFile(string fileName)
        {
            //TO DO: configure the issuer setup to determine if this check should be done
            bool checkforduplicates = true;
            if (checkforduplicates)
            {
                if (fileLoaderDAL.CheckIfFileExists(fileName))
                {
                    //file with same name has already been processed
                    //fileLoadComments += Environment.NewLine + string.Format("CMS Records File: {0} is a duplicate to a previously loaded batch. loading will is aborted".ToUpper() + Environment.NewLine, fileName);
                    //fileStatus = FileStatus.DUPLICATE;
                    //duplicatefile++;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Generates a load batch reference.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        private string generateBatchReference(FileInfo fileInfo)
        {
            return fileInfo.Name.Substring(0, fileInfo.Name.Length - 4) + DateTime.Now.ToString("yyMMddhhmmss");
        }

        /// <summary>
        /// Read the card file and return a list of CardFileRecords
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        private bool getCardRecords(FileInfo fileInfo, out List<CardFileRecord> cardFileRecords, ref List<FileCommentsObject> fileComments)
        {
            log.Debug(m => m("Reading card info from file."));
            fileComments.Add(new FileCommentsObject("Reading card info from file."));

            cardFileRecords = new List<CardFileRecord>();

            //Open the file to read from. 
            using (StreamReader sr = fileInfo.OpenText())
            {
                string line = "";
                int numberOfRecords = 0;
                int lineNumber = 0;

                while ((line = sr.ReadLine()) != null)
                {
                    lineNumber++;
                    if (line.StartsWith("EOF "))//Grab the file record count
                    {
                        if (int.TryParse(line.Replace("EOF", "").Trim(), out numberOfRecords))
                        {
                            if (numberOfRecords != cardFileRecords.Count)
                            {
                                log.Error("File record count and file records read do not match.");
                                fileComments.Add(new FileCommentsObject("File record count and file records read do not match."));
                                return false;
                            }
                        }
                        else
                        {
                            log.Error("Could not convert number of records at end of file to Integer.");
                            fileComments.Add(new FileCommentsObject("Could not convert number of records at end of file to Integer."));
                            return false;
                        }
                    }
                    else if (lineNumber == 1) //Check that the first line matches the fine name
                    {
                        if (!line.StartsWith(fileInfo.Name))
                        {
                            //throw new Exception("The file name and name of file at line 1 do not match.");
                        }
                    }
                    else
                    {
                        string PAN = line.Substring(1, 19).Trim();
                        string BranchCode = line.Substring(291, 3).Trim();
                        string PsuedoPAN = line.Substring(20, 6) + line.Substring(27, 13).Trim();
                        //string BIN = PAN.Length == 16 ? PAN.Substring(0, 6) : PAN.Substring(0, 9);
                        string SequenceNumber = "0";

                        cardFileRecords.Add(new CardFileRecord(BranchCode, PAN, PsuedoPAN, SequenceNumber));
                    }
                }
            }

            int cardsRead = cardFileRecords.Count;
            log.Debug(m => m("Total cards read from file: " + cardsRead));
            fileComments.Add(new FileCommentsObject("Total cards read from file: " + cardsRead));

            if (cardsRead == 0)
            {
                log.Debug(m => m("No card file info found in file"));
                fileComments.Add(new FileCommentsObject("No card file info found in file"));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates that the BIN is valid for the issuer
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        private bool CheckIfIsLicenseCardRecord(string cardNumber)
        {
            //bool isLicensed = false;
            for (int i = 9; i > 0; i--)
            {
                string binCode = cardNumber.Substring(0, i);

                foreach (var licensedBin in licensedBinCodes)
                {
                    if (binCode.Trim().Equals(licensedBin.Trim()))
                    {
                        return true;
                    }
                }
            }

            //if (binCode.Equals("484680") || binCode.Equals("484681"))
            //{
            //    isLicensed = true;
            //}
            //else
            //{
            log.Warn(string.Format("Card Record: {0} is not within licensed bin range. It has been blocked.", cardNumber.Substring(0, 9)));
            //fileLoadComments += Environment.NewLine + string.Format("Card Record: {0} is not within licensed bin range. will be blocked".ToUpper() + Environment.NewLine, cardNumber);
            //}

            return false;
        }

        /// <summary>
        /// Archives the file according to the file status.
        /// </summary>
        /// <param name="fileStatus"></param>
        /// <param name="config"></param>
        /// <param name="fileInfo"></param>
        /// <param name="strDateTime"></param>
        private void ArchiveFiles(FileStatus fileStatus, FileInfo fileInfo)
        {
            string archivefilename = fileInfo.Name + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            try
            {
                if (fileInfo.Exists)
                {
                    if (fileStatus == FileStatus.PROCESSED)
                    {
                        if (!new DirectoryInfo(fileInfo.DirectoryName + "\\completed").Exists)
                            new DirectoryInfo(fileInfo.DirectoryName + "\\completed").Create();
                        fileInfo.MoveTo(fileInfo.DirectoryName + "\\completed\\" + archivefilename);
                    }
                    else if (fileStatus == FileStatus.PARTIAL_LOAD)
                    {
                        if (!new DirectoryInfo(fileInfo.DirectoryName + "\\partialload").Exists)
                            new DirectoryInfo(fileInfo.DirectoryName + "\\partialload").Create();
                        fileInfo.MoveTo(fileInfo.DirectoryName + "\\partialload\\" + archivefilename);
                    }
                    else
                    {
                        if (!new DirectoryInfo(fileInfo.DirectoryName + "\\failed").Exists)
                            new DirectoryInfo(fileInfo.DirectoryName + "\\failed").Create();
                        fileInfo.MoveTo(fileInfo.DirectoryName + "\\failed\\" + archivefilename);
                    }
                }
            }
            catch (Exception ex)
            {
                //error in writing to DB / moving file 
                log.Error("Problem archiving file", ex);
                //LogFileWriter.WriteFileLoaderError(ToString(), e);
                //fileLoadComments += " | Could not move file to archive directory";
            }
        }
    }
}
