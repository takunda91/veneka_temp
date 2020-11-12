using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Utilities;
using IndigoFileLoader.dal;
using IndigoFileLoader.objects;
using IndigoFileLoader.objects.impl.files;
using IndigoFileLoader.objects.root;
using IndigoFileLoader.utility;
//using Veneka.Indigo.PINManagement;
//using Veneka.Indigo.PINManagement.objects;
using Veneka.Indigo.IssuerManagement;
using Veneka.Indigo.Common.Models;


namespace IndigoFileLoader.bll
{
    public class FileLoaderService
    {
        //database communications
        private readonly FileLoaderDAL fileLoaderDAL = new FileLoaderDAL();
        private readonly BranchService branchService = new BranchService();

        //global variables for file loader history
        private List<string[]> CardRecords;
        //private List<PINMailer> PINMailerDuplicates;
        //private List<PINMailer> PINMailerRecords;
        private string applogcomment;
        //private string batchReference;
        private int duplicatefile;
        private int duplicaterecords;
        private int unlicensedrecords;
        private int failedfileload;
        private string fileLoadComments;
        private FileStatus fileStatus;
        private string fullFileName;
        private int invalidformatfile;
        private int partialfileload;
        private int processedrecords;
        private string strDateTime;
        private int successfileload;

        #region PUBLIC METHODS
        /// <summary>
        /// Search for a list of file loader logs.
        /// </summary>
        /// <param name="fileStatus"></param>
        /// <param name="fileName"></param>
        /// <param name="issuerId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowsPerpage"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<GetFileLoderLog_Result> SearchFileLoadLog(int? fileLoadId, FileStatus? fileStatus, string fileName, int? issuerId,
                                                              DateTime? dateFrom, DateTime? dateTo, int languageId, int pageIndex, int rowsPerpage,
                                                              long auditUserId, string auditWorkstation)
        {

            return fileLoaderDAL.SearchFileLoadLog(fileLoadId, fileStatus, fileName, issuerId, dateFrom, dateTo, languageId, pageIndex, rowsPerpage,
                                                   auditUserId, auditWorkstation);
        }

        public void LoadFilesForIssuer(issuer config)
        {
            //check what functions are being used
            if (config.instant_card_issue_YN)
            {
                //load card file for instant issue
                LoadCardFile(config);
            }

            //if (config.pin_mailer_printing_YN)
            //{
            //    //load pin file for instant issue
            //    LoadPinFile(config);
            //}
        }

        #endregion

        #region PRIVATE METHODS

        private void LoadPinFile(issuer config)
        {
            ////build app log comment
            //applogcomment = DateTime.Now.ToLongTimeString() + " " + config.issuer_code + " PIN FILES:";

            //var dirInfo = new DirectoryInfo(config.pin_file_location);

            ////check there are files to load
            //FileInfo[] filesToLoad = GetFilesToLoad(config, dirInfo);

            //if (filesToLoad != null)
            //{
            //    //process each file in array
            //    foreach (FileInfo fileInfo in filesToLoad)
            //    {
            //        //setup variables
            //        SetupVariables(fileInfo);

            //        //check if the file has already been loaded
            //        if (!CheckIfDuplicateFile(config, fileInfo.Name))
            //        {
            //            //TO DO: get this value from the database (hard coded for MSB)
            //            var cmsType = CMS.SMART_VISTA;
            //            string batchReference = "";

            //            switch (cmsType)
            //            {
            //                case CMS.SMART_VISTA:
            //                    //MSB
            //                    CreatePinBatchReference_SmartVista(fileInfo.Name);
            //                    ProcessPinFile_SmartVista(config);
            //                    break;

            //                case CMS.POSTILION:
            //                    //RSwitch
            //                    CreatePinBatchReference_Postilion(config.issuer_id, config.issuer_code, strDateTime);
            //                    ProcessPinFile_Postilion(fullFileName, config, strDateTime);
            //                    break;
            //            }

            //            //write batch and pins to DB
            //            if (fileStatus.Equals(FileStatus.READ) || fileStatus.Equals(FileStatus.PARTIAL_LOAD))
            //            {
            //                string branchCode = batchReference.Substring(0, batchReference.IndexOf('_'));
            //                fileLoaderDAL.InsertPinBatchRecord(batchReference, strDateTime, PINMailerRecords.Count(),
            //                                                   config.issuer_id, branchCode);
            //                fileLoaderDAL.InsertPinRecords(batchReference, PINMailerRecords);
            //            }
            //        }
            //        //write history
            //        CreateFileHistory(fileInfo.Name, fileInfo.Directory.ToString(), fileInfo.Length, FileType.PIN_MAILER,
            //                          fileInfo.CreationTime.ToString(), fileStatus, strDateTime, config.issuer_id,
            //                          processedrecords,
            //                          duplicaterecords, fileLoadComments);
            //        //archive
            //        ArchiveFiles(fileStatus, config, fileInfo, strDateTime);
            //    }
            //    //build app log comment
            //    applogcomment += " Success=" + successfileload + ", Partial=" + partialfileload + " Failed=" +
            //                     failedfileload +
            //                     ", Duplicate=" + duplicatefile + ", Invalid format=" + invalidformatfile;
            //}
            ////write to application log
            //LogFileWriter.WriteFileLoaderComment(applogcomment);
        }

        /// <summary>
        /// THIS IS NOW REDUNDENT CODE - DONT USE
        /// </summary>
        /// <param name="config"></param>
        private void LoadCardFile(issuer config)
        {
            //build app log comment
            applogcomment = DateTime.Now.ToLongTimeString() + " " + config.issuer_code + " CARD FILES: ";

            var dirInfo = new DirectoryInfo("" /*config.cards_file_location*/);

            //check there are files to load
            FileInfo[] filesToLoad = GetFilesToLoad(config, dirInfo);

            if (filesToLoad != null)
            {
                //process each file in array
                foreach (FileInfo fileInfo in filesToLoad)
                {
                    file_history fileHistory = new file_history();
                    fileHistory.file_types = new file_types();
                    fileHistory.file_statuses = new file_statuses();
                    fileHistory.name_of_file = fileInfo.Name;
                    fileHistory.file_directory = fileInfo.Directory.ToString();
                    fileHistory.file_size = (int)fileInfo.Length;
                    fileHistory.file_types.file_type_id  = (int)FileType.CARD_IMPORT;
                    fileHistory.file_created_date = fileInfo.CreationTime;                    

                    //setup variables
                    SetupVariables(fileInfo);

                    //check if the file has already been loaded
                    if (!CheckIfDuplicateFile(config, fileInfo.Name))
                    {
                        //TO DO: get this value from the database (hard coded for Ecobank)
                        var cmsType = CMS.TIETO;
                        string loadBatchReference = "";
                        //<PAN, ValidCard>
                        Dictionary<string, ValidCard> validCards = new Dictionary<string, ValidCard>(); 

                        //card issuing
                        switch (cmsType)
                        {
                            case CMS.TIETO:
                                //Ecobank
                                loadBatchReference = CreateCardBatchReference_Tieto(fileInfo.Name);
                                if (fileStatus.Equals(FileStatus.READ))
                                    validCards = ProcessCardFile_Tieto(fullFileName, config, fileHistory);
                                break;
                        }

                        //write batch and cards to DB
                        if (fileStatus.Equals(FileStatus.PROCESSED) || fileStatus.Equals(FileStatus.PARTIAL_LOAD))
                        {
                            if (validCards.Count > 0)
                            {
                                if (true /*fileLoaderDAL.CreateLoadBatch(validCards, loadBatchReference, config.issuer_id, fileHistory)*/)
                                {

                                    Veneka.Indigo.CardManagement.LoadBatchMangementService loadBatchMgm = new Veneka.Indigo.CardManagement.LoadBatchMangementService();

                                    //SELF APPROVES LOAD BATCH
                                    Veneka.Indigo.CardManagement.objects.DatabaseResponse response = null;//loadBatchMgm.UpdateLoadBatchStatus(loadBatchReference, "SYSTEM",
                                    //     indigoCardManagement.LoadBatchStatus.APPROVED.ToString());


                                    if (response.responseSuccess)
                                    {
                                        //CREATES A DISTRIBUTION BATCH
                                        List<string> cardsList = new List<string>();
                                        string branchCode = String.Empty;
                                        foreach (var item in this.CardRecords)
                                        {
                                            cardsList.Add(item[0]);
                                            branchCode = item[1];
                                        }

                                        var batch = new Veneka.Indigo.CardManagement.objects.DistributionBatch
                                        (
                                        0, String.Empty, string.Empty, config.issuer_id, null,
                                        DateTime.Now, Veneka.Indigo.CardManagement.DistributionBatchStatus.CREATED,
                                        cardsList, branchCode, string.Empty,
                                        new List<string>());

                                        Veneka.Indigo.CardManagement.DistBatchManagementService distBatchMgm = new Veneka.Indigo.CardManagement.DistBatchManagementService();


                                        if (cardsList.Count > 0)
                                        {
                                            response = null;//distBatchMgm.CreateDistributionBatch(batch, "SYSTEM", cardsList);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //write history
                    CreateFileHistory(fileInfo.Name, fileInfo.Directory.ToString(), fileInfo.Length,
                                      FileType.CARD_IMPORT,
                                      fileInfo.CreationTime.ToString(), fileStatus, strDateTime, config.issuer_id,
                                      processedrecords,
                                      duplicaterecords, fileLoadComments);
                    //archive

                    ArchiveFiles(fileStatus, config, fileInfo, strDateTime);

                }

                //build app log comment
                applogcomment += " Success=" + successfileload + ", Partial=" + partialfileload + " Failed=" +
                                 failedfileload +
                                 ", Duplicate=" + duplicatefile + ", Invalid format=" + invalidformatfile;
            }

            //write to application log
            LogFileWriter.WriteFileLoaderComment(applogcomment);
        }

        private FileStatus ProcessPinFile_SmartVista(issuer config)
        {
            //branchCode = batchReference.Substring(0, batchReference.IndexOf('_'));

            //read file
            //IndigoFileLoader.objects.root.ACardsFile cardsFile = null;
            var cardsFile = new CSVFile(fullFileName, false, false);
            List<string[]> records = cardsFile.ReadFile();

            //note --- check if ref is null

            //parse records, format contents
            for (int i = 0; i < records.Count; i++)
            {
                var newRecord = new string[3];
                string record = records[i][0];
                newRecord[0] = record.Substring(0, 4) + record.Substring(5, 4) + record.Substring(10, 4) +
                               record.Substring(15, 4);
                newRecord[1] =
                   IndigoFileLoader.utility.UtilityClass.HandleUnexpectedCharacters(
                        record.Substring(record.IndexOf('^') + 1, record.LastIndexOf('^') - record.IndexOf('^') - 1)
                              .Replace('/', ' '));
                newRecord[2] = record.Substring(record.LastIndexOf('#') + 1, 16);
                records[i] = newRecord;
            }

            return FileStatus.READ;
        }

        private void ProcessPinFile_Postilion(string fullFileName, issuer config, string processedDT)
        {
            //try
            //{
            //    //read file
            //    ACardsFile cardsFile = null;
            //    cardsFile = new CSVFile(fullFileName, false, false);
            //    List<string[]> records = cardsFile.ReadFile();


            //    var batchReferences = new List<string>();

            //    //PINMailerRecords = new List<PINMailer>();

            //    //parse records, format contents
            //    //start at pos 1 and end at count -1 (ignore header & footer)
            //    for (int i = 1; i < records.Count - 1; i++)
            //    {
            //        string cardNumber = records[i][0];
            //        string batchReference = records[i][1]; //validate what i am getting here!!
            //        string customerName = records[i][7] + " " + records[i][8] + " " + records[i][10];
            //        string encryptedPIN = records[i][19];
            //        string customerAddress = records[i][24] + ";" + records[i][25] + ";" + records[i][26];

            //        //RL fix this
            //        //int index = batchNames.IndexOf(batchReference);
            //        int index = 1;

            //        if (index == null)
            //        {
            //            //new branch, need to add a new List for that branch to the split batches
            //            //splitBatches.Add(new List<string[]>());

            //            //add the PIN mailer to the the List for the correct branch

            //            //we now need to create a batch and insert into DB
            //            // batchReference = CreatePinBatchReference_Postilion(config.IssuerID, config.IssuerCode, processedDT);

            //            // batchReferences.Add(batchReference);

            //            PINMailerRecords.Add(new PINMailer(config.issuer_id,
            //                                               batchReference,
            //                                               "", //pin mailer ref
            //                                               PINMailerStatus.NOT_PRINTED,
            //                                               cardNumber,
            //                                               "", //pvv offset 
            //                                               encryptedPIN,
            //                                               customerName,
            //                                               customerAddress,
            //                                               DateTime.MinValue,
            //                                               DateTime.MinValue));
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    //invalid file structure
            //    LogFileWriter.WriteFileLoaderError(
            //        config.issuer_code + ": " + fullFileName + " contains invalid data, or is not formatted correctly" +
            //        ToString(), ex);
            //    fileLoadComments += " | file contains invalid data, or is not formatted correctly";
            //    fileStatus = FileStatus.INVALID_FORMAT;
            //}
        }

        /// <summary>
        /// Reads all card info from file, checks for duplicates in the file, in the DB and verifies the branch is valid, returns a list of valid card number with their branch code.
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <param name="config"></param>
        /// <param name="fileHistory"></param>
        /// <returns></returns>
        private Dictionary<string, ValidCard> ProcessCardFile_Tieto(string fullFileName, issuer config, file_history fileHistory)
        {
            //<PAN, validCard>
            Dictionary<string, ValidCard> validCards = new Dictionary<string, ValidCard>(); 
            var cardsFile = new TextFile(fullFileName, false, false);
            try
            {
                //<BranchCode, BranchLookup>
                Dictionary<string, BranchLookup> validBranches = new Dictionary<string, BranchLookup>();

                CardRecords = cardsFile.ReadFile();

                string duplicatesInFileMessage = "";
                string duplicatesInDB = "";
                string unlicensesCardsErrorMessage = "";//temporary until license management is fully implemented

                #region basic records processing and fields settings
                for (int i = 0; i < CardRecords.Count; i++)
                {
                    string line = CardRecords[i][0];
                    //text file, so each record only contains 1 string which then needs to be sub-stringed.

                    string[] fields = line.Split(new string[]{ 
                        " ".PadLeft(1)," ".PadLeft(2)," ".PadLeft(3),
                        " ".PadLeft(4)," ".PadLeft(5)," ".PadLeft(6),
                        " ".PadLeft(7)," ".PadLeft(8)," ".PadLeft(9),
                        " ".PadLeft(10)," ".PadLeft(11)," ".PadLeft(12),
                        " ".PadLeft(13)," ".PadLeft(14)," ".PadLeft(15)
                    },StringSplitOptions.RemoveEmptyEntries);

                    ValidCard validCard = new ValidCard();
                    validCard.PAN = fields[0].Substring(1, 16);
                    validCard.BranchCode = validCard.PAN.Substring(6, 2);
                    validCard.PsuedoPAN = fields[1] + fields[2];
                    validCard.SequenceNumber = fields[PostilionDetailedRecordColumns.SEQ_NR];

                    BranchLookup branchLookup;                    
                    //See if we've already checked the DB for the branch. If it's not in the dictionary then do a read from the DB.                  
                    if (!validBranches.TryGetValue(validCard.BranchCode, out branchLookup))
                    {
                        //check if branch exist                                              
                        branch Branch = CheckDBForBranch(config.issuer_id, validCard.BranchCode);
                        branchLookup = new BranchLookup(validCard.BranchCode, 0, false);

                        if (Branch != null)
                        {                            
                            branchLookup = new BranchLookup(Branch.branch_code, Branch.branch_id, true);
                        }

                        validBranches.Add(validCard.BranchCode, branchLookup);
                    }
                    

                    //check if branch exist
                    //if not, add a record to rejected file
                    if (!branchLookup.IsValid)
                    {
                        RegisterNonExistanceBranch(line, validCard.BranchCode, config);
                    }
                    else
                    {
                        //CardRecords[i] = new[] { validCard.PsuedoPAN, validCard.BranchCode }; //, custNames, pinBlock };

                        //Check if the card has been added to the dictionary already, if it has dont add it again.
                        if (validCards.ContainsKey(validCard.PsuedoPAN))
                        {
                            duplicaterecords++;
                            duplicatesInFileMessage += validCard.PsuedoPAN + ", ";
                        }
                        else
                        {
                            //check if card is within licensed ecobank bins -- temp solution
                            //TODO: RAB Remove hard coding here.... yuck yuck yuck
                            if (CheckIfIsLicenseCardRecord(validCard.PsuedoPAN))
                            {
                                validCard.BranchId = branchLookup.BranchId;
                                validCards.Add(validCard.PsuedoPAN, validCard);
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
                    //Because the results coming back are based on the keys, the dictionary should have this key, but checking just incase
                    if (validCards.ContainsKey(result))
                    {
                        //recored is already in DB
                        validCards.Remove(result);                        
                        duplicaterecords++;
                        duplicatesInDB += string.Format("Card Record is a duplicate in Indigo {0},", result);
                        fileLoadComments += Environment.NewLine + string.Format("Card Record {0} is a duplicate to a previously loaded record. Record will be excluded".ToUpper() + Environment.NewLine, result);
                    }
                }
                #endregion

                processedrecords = validCards.Count;

                //update file status
                UpdateFileStatusAfterRead();

                //add comments for duplicates
                if (duplicatesInFileMessage.Trim().Length > 0)
                {
                    fileLoadComments += " Duplicate records in file: ".ToUpper() + duplicatesInFileMessage + ". ";
                }

                if (duplicatesInDB.Trim().Length > 0)
                {
                    fileLoadComments += " Records already exist in DB: ".ToUpper() + duplicatesInDB + ". ";
                }

                //add comments for unlicense records
                if (unlicensesCardsErrorMessage.Trim().Length > 0)
                {
                    fileLoadComments += " - Unlicensed cards processing is disabled. Unlicensed records: ".ToUpper() +
                        unlicensedrecords + ". ";
                }
            }
            catch (Exception ex)
            {
                //invalid file structure
                LogFileWriter.WriteFileLoaderError(
                    config.issuer_code + ": " + fullFileName + " contains invalid data, or is not formatted correctly" +
                    ToString(), ex);
                fileLoadComments += " | file contains invalid data, or is not formatted correctly";
                fileStatus = FileStatus.INVALID_FORMAT;
                invalidformatfile++;
            }

            fileHistory.file_statuses.file_status_id = (int)fileStatus;
            fileHistory.number_successful_records = processedrecords;
            fileHistory.number_failed_records = duplicaterecords;
            fileHistory.file_load_comments = fileLoadComments;

            return validCards;
        }

        private branch CheckDBForBranch(int issuerId, string branchCode)
        {
            return branchService.GetBranchByBranchCode(issuerId, branchCode, BranchStatus.ACTIVE);
        }

        private void RegisterNonExistanceBranch(string record,string brachCode,issuer config)
        {
            try
            {
                //if (!Directory.Exists(config.cards_file_location + "\\missingBranches"))
                //{
                //    Directory.CreateDirectory(config.cards_file_location + "\\missingBranches");
                //}

                StreamWriter sr = new StreamWriter(//config.cards_file_location +
                    "\\missingBranches\\" + config.issuer_code +"_"+brachCode+ ".OUT",true);

                sr.WriteLine(record);
                sr.Flush();
                sr.Close();

                LogFileWriter.WriteFileLoaderError(
                  config.issuer_code + ": " + fullFileName + " contains records of a non existant branch", new Exception("INVALID ISSUER BRANCH"));

                fileLoadComments += " | file contains data for a non existant branch in this issuer, please ensure branches are created prior loading";
                fileStatus = FileStatus.PARTIAL_LOAD;
                
            }
            catch (Exception ex)
            {
                LogFileWriter.WriteFileLoaderError(ToString(), ex);              
            }
        }

        private bool CheckIfDuplicatePinRecord(string CardNumber, string EncryptedPIN)
        {
            //if (fileLoaderDAL.CheckPinMailerLoaded(CardNumber, EncryptedPIN))
            //{
            //    //write to file loading comments
            //    if (PINMailerDuplicates.Count() == 0)
            //        fileLoadComments += " | Duplicate cards: ";

            //    fileLoadComments += CardNumber.Substring(0, 4) + "**"
            //                        + CardNumber.Substring(CardNumber.Length - 4, 4) + " ";

            //    return true;
            //}
            return false;
        }

        private bool CheckIfDuplicateCardRecords(List<string> cardList)
        {
            List<string> results = fileLoaderDAL.ValidateCardsLoaded(cardList);

            foreach(string card in cardList)
            {
                bool cardFound = false;
                foreach (string result in results)
                {
                    if(card.Equals(result))
                    {
                        cardFound = true;
                        break;
                    }
                }

                if (!cardFound)
                {
                    fileLoadComments += Environment.NewLine + string.Format("Card Record {0} is a duplicate to a previously loaded record. Record will be excluded".ToUpper() + Environment.NewLine, card);
                }
            }
            return false;
        }

        //private bool CheckIfDuplicateCardRecord(string cardNumber)
        //{
        //    if (fileLoaderDAL.ValidateCardLoaded(cardNumber))
        //    {
        //        fileLoadComments += Environment.NewLine + string.Format("Card Record {0} is a duplicate to a previously loaded record. Record will be excluded".ToUpper() + Environment.NewLine, cardNumber);
        //        return true;
        //    }
        //    return false;
        //}

        private bool CheckIfIsLicenseCardRecord(string cardNumber)
        {
            bool isLicensed = false;

            string binCode = cardNumber.Substring(0, 6);

            if (binCode.Equals("484680") || binCode.Equals("484681"))
            {
                isLicensed = true;
            }
            else
            {
                fileLoadComments += Environment.NewLine + string.Format("Card Record: {0} is not within licensed bin range. will be blocked".ToUpper() + Environment.NewLine, cardNumber);
            }

            return isLicensed;
        }

        private bool CheckIfDuplicateFile(issuer config, string fileName)
        {
            //TO DO: configure the issuer setup to determine if this check should be done
            bool checkforduplicates = true;
            if (checkforduplicates)
            {
                if (fileLoaderDAL.CheckIfFileExists(fileName))
                {
                    //file with same name has already been processed
                    fileLoadComments += Environment.NewLine + string.Format("CMS Records File: {0} is a duplicate to a previously loaded batch. loading will is aborted".ToUpper() + Environment.NewLine, fileName);
                    fileStatus = FileStatus.DUPLICATE_FILE;
                    duplicatefile++;
                    return true;
                }
            }
            return false;
        }

        private void ArchiveFiles(FileStatus fileStatus, issuer config, FileInfo fileInfo, string strDateTime)
        {
            string archivefilename = fileInfo.Name + "_" + strDateTime;
            try
            {

                if (true/*config.delete_card_file_YN*/)
                {
                    File.Delete(fileInfo.FullName);
                }
                else
                {

                    if (fileStatus.Equals(FileStatus.PROCESSED))
                    {

                        if (!new DirectoryInfo(fileInfo.DirectoryName + "\\completed").Exists)
                            new DirectoryInfo(fileInfo.DirectoryName + "\\completed").Create();
                        fileInfo.MoveTo(fileInfo.DirectoryName + "\\completed\\" + archivefilename);
                    }
                    else if (fileStatus.Equals(FileStatus.PARTIAL_LOAD))
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
            catch (Exception e)
            {
                //error in writing to DB / moving file 
                LogFileWriter.WriteFileLoaderError(ToString(), e);
                fileLoadComments += " | Could not move file to archive directory";
            }
        }

        private void CreatePinBatchReference_SmartVista(string fileName)
        {
            try
            {
                fileName = fileName.Substring(3, (fileName.Length - 3));
                fileName = fileName.Substring(0, fileName.LastIndexOf('_'));
            }
            catch (Exception ex)
            {
                //file name incorrectly formatted
                LogFileWriter.WriteFileLoaderError(fileName + " file name formatted incorrectly: " + ToString(), ex);
                fileLoadComments += "| file name formatted incorrectly";
                fileStatus = FileStatus.INVALID_NAME;
            }
        }

        private void CreatePinBatchReference_Postilion(int issuerID, string issuerCode, string loadedDT)
        {
            var sbBatchReference = new StringBuilder();
            sbBatchReference.Append(issuerID);
            sbBatchReference.Append("-");
            sbBatchReference.Append(issuerCode);
            sbBatchReference.Append("-BRANCH_NAME-");
            sbBatchReference.Append(loadedDT);
            //TODO: FIX!!!
            //batchReference = sbBatchReference.ToString();
        }

        private string CreateCardBatchReference_Tieto(string fileName)
        {            
            try
            {
                //Mpho will need to put more logic in here
                return fileName.Substring(0, fileName.Length);
            }
            catch (Exception ex)
            {
                //file name incorrectly formatted
                LogFileWriter.WriteFileLoaderError(fileName + " file name formatted incorrectly: " + ToString(), ex);
                fileLoadComments += " | file name formatted incorrectly";
                fileStatus = FileStatus.INVALID_NAME;
                invalidformatfile++;
            }

            return null;
        }

        private FileInfo[] FilesToImport(DirectoryInfo dirInfo)
        {
            try
            {
                return dirInfo.GetFiles();
            }
            catch (FileNotFoundException e)
            {
                //invalid Dir structure
                LogFileWriter.WriteFileLoaderError(ToString(), e);
                return null;
            }
        }

        private void CreateFileHistory(string fileName, string directory, long length, FileType fileType,
                                       string fileCreatedDate, FileStatus fileStatus, string fileLoadedDate,
                                       int issuerID, int successRecords, int failedRecords, string comments)
        {
            //write file to file history
            var fileHistory = new FileDetails(0, fileName, fileType, fileCreatedDate, directory, length,
                                              fileStatus.ToString(), fileLoadedDate, issuerID,successRecords,
                                              failedRecords, comments);


            //long fileID = fileLoaderDAL.SaveFileInfo(fileHistory);
        }

        private FileInfo[] GetFilesToLoad(issuer config, DirectoryInfo dirInfo)
        {
            try
            {
                FileInfo[] filesToLoad = FilesToImport(dirInfo);

                if (filesToLoad.Count() == 0)
                {
                    //build app log comment
                    applogcomment += "No files to load";

                    return null;
                }

                //build app log comment
                applogcomment += " Total=" + filesToLoad.Count();

                return filesToLoad;
            }
            catch (Exception ex)
            {
                //build app log comment
                applogcomment += "Invalid directory: " + dirInfo.FullName;
                //write to error log
                LogFileWriter.WriteFileLoaderError(config.issuer_code + ": Cannot find import directory: " +
                                                   dirInfo.FullName + ToString(), ex);

                return null;
            }
        }

        private void SetupVariables(FileInfo fileInfo)
        {
            fullFileName = fileInfo.DirectoryName + "\\" + fileInfo.Name;
            strDateTime = DateTimeOffset.Now.ToString();
            fileStatus = FileStatus.READ;
            processedrecords = 0;
            duplicaterecords = 0;
            unlicensedrecords = 0;
            fileLoadComments = "";
        }

        private void UpdateFileStatusAfterRead()
        {
            if (processedrecords <= 0)
            {
                //all duplicate records
                fileStatus = FileStatus.LOAD_FAIL;
                failedfileload++;
            }
            else
                if (duplicaterecords >= 1 || unlicensedrecords >= 1)
                {
                    fileStatus = FileStatus.PARTIAL_LOAD;
                    partialfileload++;
                }
                else
                {
                    fileStatus = FileStatus.PROCESSED;
                    successfileload++;

                }

        #endregion
        }
    }
}
