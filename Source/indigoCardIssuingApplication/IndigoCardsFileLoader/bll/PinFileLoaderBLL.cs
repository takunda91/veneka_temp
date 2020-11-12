using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Veneka.Indigo.Common.Models;
using IndigoFileLoader;
using IndigoFileLoader.dal;
using IndigoFileLoader.objects;
using IndigoFileLoader.Modules;
using IndigoFileLoader.Modules.Extensibility;
using Veneka.Indigo.IssuerManagement;

namespace IndigoFileLoader.bll
{
    class PinFileLoaderBLL
    {
        private FileLoaderDAL fileLoaderDAL = new FileLoaderDAL();

        public void LoadPinCardFile(issuer issuer)
        {
            ////build app log comment
            //string applogcomment = DateTime.Now.ToLongTimeString() + " " + issuer.issuer_code + " CARD FILES: ";

            //var dirInfo = new DirectoryInfo(issuer.cards_file_location);
            //FileInfo[] filesToLoad = dirInfo.GetFiles();

            //if (filesToLoad != null)
            //{
            //    //process each file in array
            //    foreach (FileInfo fileInfo in filesToLoad)
            //    {
            //        file_history fileHistory = new file_history();
            //        fileHistory.file_types = new file_types();
            //        fileHistory.file_statuses = new file_statuses();
            //        fileHistory.name_of_file = fileInfo.Name;
            //        fileHistory.file_directory = fileInfo.Directory.ToString();
            //        fileHistory.file_size = (int)fileInfo.Length;
            //        fileHistory.file_types.file_type_id = (int)FileType.CARD_IMPORT;
            //        fileHistory.file_created_date = fileInfo.CreationTime;

            //        //check if the file has already been loaded
            //        if (!CheckIfDuplicateFile(fileInfo.Name))
            //        {                        
            //            IndigoFileReaderModule fileReader = new IndigoFileReaderModule();
            //            string loadBatchReference = fileReader.cardFileReader.generateBatchReference(fileInfo);

            //            List<CardFileRecord> cardFileRecords = fileReader.cardFileReader.getCardRecords(fileInfo);

            //            Dictionary<string, CardFileRecord> validCards = new Dictionary<string, CardFileRecord>();
            //            validCards = validateCardRecords(cardFileRecords, issuer.issuer_id);
                       
            //            //write batch and cards to DB
            //            if (fileHistory.file_statuses.file_status_id == (int)FileStatus.PROCESSED ||
            //                    fileHistory.file_statuses.file_status_id == (int)FileStatus.PARTIAL_LOAD)
            //            {
            //                if (validCards != null && validCards.Count > 0)
            //                {
            //                    if (fileLoaderDAL.CreateLoadBatch(validCards, loadBatchReference, issuer.issuer_id, fileHistory))
            //                    {
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
        }

        /// <summary>
        /// Validates a list of card records.
        /// </summary>
        /// <param name="cardFileRecords"></param>
        /// <param name="issuerId"></param>
        /// <returns></returns>
        private Dictionary<string, CardFileRecord> validateCardRecords(List<CardFileRecord> cardFileRecords, int issuerId)
        {            
            Dictionary<string, CardFileRecord> validCards = new Dictionary<string, CardFileRecord>(); //<CardNumber, CardFileRecord>            
            Dictionary<string, BranchLookup> validBranches = new Dictionary<string, BranchLookup>(); //<BranchCode, BranchLookup>
            BranchService branchService = new BranchService();

            #region Loop through all the cards, checking valid branchs, Valid BIN's and duplicates in the file
            foreach (CardFileRecord cardFileRecord in cardFileRecords)
            {
                BranchLookup branchLookup;
                //See if we've already checked the DB for the branch. If it's not in the dictionary then do a read from the DB.                  
                if (!validBranches.TryGetValue(cardFileRecord.BranchCode, out branchLookup))
                {
                    //check if branch exists in the DB                                            
                    branch Branch = branchService.GetBranchByBranchCode(issuerId, cardFileRecord.BranchCode, BranchStatus.ACTIVE);
                    branchLookup = new BranchLookup(cardFileRecord.BranchCode, 0, false);

                    if (Branch != null)
                    {
                        branchLookup = new BranchLookup(Branch.branch_code, Branch.branch_id, true);
                    }

                    validBranches.Add(cardFileRecord.BranchCode, branchLookup);
                }


                //check if branch exist
                //if not, add a record to rejected file
                if (!branchLookup.IsValid)
                {
                    //RegisterNonExistanceBranch(line, validCard.BranchCode, config);
                }
                else
                {
                    //Check if the card has been added to the dictionary already, if it has dont add it again.
                    if (validCards.ContainsKey(cardFileRecord.PsuedoPAN))
                    {
                        //duplicaterecords++;
                        //duplicatesInFileMessage += validCard.PsuedoPAN + ", ";
                    }
                    else
                    {                        
                        if (CheckIfIsLicenseCardRecord(cardFileRecord.PsuedoPAN))
                        {
                            cardFileRecord.BranchId = branchLookup.BranchId;
                            validCards.Add(cardFileRecord.PsuedoPAN, cardFileRecord);
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
                    //duplicaterecords++;
                    //duplicatesInDB += string.Format("Card Record is a duplicate in Indigo {0},", result);
                    //fileLoadComments += Environment.NewLine + string.Format("Card Record {0} is a duplicate to a previously loaded record. Record will be excluded".ToUpper() + Environment.NewLine, result);
                }
            }
            #endregion

            return validCards;
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
        /// Validates that the BIN is valid for the issuer
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        private bool CheckIfIsLicenseCardRecord(string cardNumber)
        {
            bool isLicensed = false;
            //TODO: Remove hard coding
            string binCode = cardNumber.Substring(0, 6);

            if (binCode.Equals("484680") || binCode.Equals("484681"))
            {
                isLicensed = true;
            }
            else
            {
                //fileLoadComments += Environment.NewLine + string.Format("Card Record: {0} is not within licensed bin range. will be blocked".ToUpper() + Environment.NewLine, cardNumber);
            }

            return isLicensed;
        }
    }
}
