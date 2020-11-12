using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using Veneka.Indigo.Integration.FileLoader.DAL;
using Veneka.Indigo.Integration.FileLoader.Objects;
using System.IO;
using Veneka.Licensing.Client;
using Veneka.Licensing.Common;

namespace Veneka.Indigo.Integration.FileLoader.BLL
{
    public class FileLoaderBLL
    {
        private readonly ILog log;
        private readonly FileLoaderDAL fileLoaderDAL;

        public FileLoaderBLL(string connectionString, string logger, int languageId, long auditUserId, string auditWorkStation)
        {
            log = LogManager.GetLogger(logger);
            fileLoaderDAL = new FileLoaderDAL(connectionString);
        }

        internal bool TryIssuerLookup(string issuerCode, out Issuer issuer, long auditUserId, string auditWorkstation)
        {
            issuer = null;

            if(!String.IsNullOrWhiteSpace(issuerCode))
            {
                var result = fileLoaderDAL.FetchIssuerByCode(issuerCode, auditUserId, auditWorkstation);

                if(result != null)
                {
                    issuer = result;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Find the issuer the bin code is linked to
        /// </summary>
        /// <param name="binCode">Will be trimmed to first 6 digits, therefore PAN may be supplied</param>
        /// <param name="issuerId"></param>
        /// <returns></returns>
        internal bool TryBINLookup(string branchCode, out Issuer issuer)
        {
            issuer = null;

            if (!String.IsNullOrWhiteSpace(branchCode))
            {
                var result = fileLoaderDAL.FetchIssuerByProductAndBinCode(null, branchCode);

                if (result != null)
                {
                    issuer = result;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Validates a list of card records.
        /// </summary>
        /// <param name="cardFileRecords"></param>
        /// <param name="issuerId"></param>
        /// <returns></returns>
        //internal Veneka.Indigo.Integration.FileLoader.FileStatus ValidateCardRecords(CardFile cardFile, List<string> licensedBinCodes, ref List<FileCommentsObject> fileComments)
        //{
        //    cardFile.ValidCardFileRecords = new List<CardFileRecord>();

        //    BranchLookup cardCentreBranch = null;
        //    Dictionary<string, CardFileRecord> validCards = new Dictionary<string, CardFileRecord>(); //<CardNumber, CardFileRecord>            
        //    Dictionary<string, BranchLookup> validBranches = new Dictionary<string, BranchLookup>(); //<BranchCode, BranchLookup>
        //    List<IssuerProduct> validProducts = new List<IssuerProduct>();

        //    //get a list of valid bins

        //    #region Loop through all the cards, checking valid branchs, Valid BIN's and duplicates in the file
        //    foreach (CardFileRecord cardFileRecord in cardFile.CardFileRecords)
        //    {
        //        BranchLookup branchLookup;
        //        if (!String.IsNullOrEmpty(cardFileRecord.BranchCode))
        //        {
        //            //See if we've already checked the DB for the branch. If it's not in the dictionary then do a read from the DB.                  
        //            if (!validBranches.TryGetValue(cardFileRecord.BranchCode.Trim().ToUpper(), out branchLookup))
        //            {
        //                //check if branch exists in the DB                                            
        //                branchLookup = fileLoaderDAL.FetchBranchByBranchCode(cardFile.FileIssuer.IssuerId, cardFileRecord.BranchCode);
                        
        //                validBranches.Add(cardFileRecord.BranchCode.Trim().ToUpper(), branchLookup);
        //            }
        //        }
        //        else
        //        {
        //            //link card to card centre branch
        //            if (cardCentreBranch == null)
        //                branchLookup = fileLoaderDAL.FetchCardCentreBranch(cardFile.FileIssuer.IssuerId);
        //            else
        //                branchLookup = cardCentreBranch;

        //            //validBranches.Add(branchLookup.BranchCode.Trim().ToUpper(), branchLookup);
        //        }


        //        //check if branch exist
        //        //if not, add a record to rejected file
        //        if (!branchLookup.IsValid)
        //        {
        //            log.Error("No Valid Branch found in Indigo:\t" + branchLookup.BranchCode);
        //            fileComments.Add(new FileCommentsObject("No Valid Branch found in Indigo:\t" + branchLookup.BranchCode));
        //            return FileStatus.NO_ACTIVE_BRANCH_FOUND;
        //        }
        //        else
        //        {
        //            cardFileRecord.BranchId = branchLookup.BranchId;

        //            //Check if the card has been added to the dictionary already, if it has dont add it again.
        //            if (validCards.ContainsKey(cardFileRecord.PAN))
        //            {
        //                fileComments.Add(new FileCommentsObject("Duplicate card found in card file:\t" + cardFileRecord.PsuedoPAN));
        //                log.Error("Duplicate card found in card file:\t" + cardFileRecord.PsuedoPAN);
        //                return FileStatus.DUPLICATE_CARDS_IN_FILE;
        //            }
        //            else
        //            {
        //                if (CheckIfIsLicenseCardRecord(cardFileRecord.PAN, licensedBinCodes))
        //                {
        //                    //See if we can link this card to a product
        //                    int lookupTries = 0;

        //                    do
        //                    {
        //                        int charCount = 0;
        //                        foreach (var product in validProducts)
        //                        {
        //                            log.TraceFormat("Try Match Product \"{0}\" with PAN \"{1}\"", product.BIN, cardFileRecord.PAN);

        //                            //Check the actual PAN, not PSUDO pan... Product bin can be up to 9 characters.
        //                            if (cardFileRecord.PAN.StartsWith(product.BIN))
        //                            {
        //                                log.Trace("Product Matched");

        //                                //Need to find the bincode with the least amount of characters, as this result may have more than one result.
        //                                //E.G. Card number could be 123456-88771199 but the result may contain a record for products with:
        //                                // 123456 and 123456999 becuse the search is only on the first 6 characters.
        //                                if (product.BIN.Length > charCount)
        //                                {
        //                                    charCount = product.BIN.Length;
        //                                    cardFileRecord.ProductId = product.ProductId;
        //                                    cardFileRecord.SubProductId = product.SubProductId;
        //                                    cardFileRecord.CardIssueMethodId = product.CardIssueMethodId;
        //                                }
        //                            }
        //                        }

        //                        if (cardFileRecord.ProductId == null)//Call the DB to see if we can get valid product for the user
        //                        {
        //                            //var result = fileLoaderDAL.FetchIssuerProduct(cardFileRecord.PAN, cardFile.FileIssuer.IssuerId);
        //                            //log.TraceFormat("Products Found in DB={0}", result.Count);
        //                            //validProducts.AddRange(result);
        //                        }

        //                        lookupTries++;
        //                    }
        //                    while (cardFileRecord.ProductId == null && lookupTries < 3);

        //                    if (cardFileRecord.ProductId != null)
        //                    {
        //                        //cardFileRecord.BranchId = branchLookup.BranchId;
        //                        //cardFileRecord.ProductId = productId;
        //                        validCards.Add(cardFileRecord.PAN.Trim(), cardFileRecord);
        //                    }
        //                    else
        //                    {
        //                        //TODO; could not find product for card.
        //                        //throw new Exception("Could not find product for card.");
        //                        log.Error("Could not find product for card.");
        //                        fileComments.Add(new FileCommentsObject("Could not find product for card."));
        //                        return FileStatus.NO_PRODUCT_FOUND_FOR_CARD;
        //                    }
        //                }
        //                else
        //                {
        //                    log.Error("Unlicensed BIN in card file.");
        //                    fileComments.Add(new FileCommentsObject("Unlicensed BIN in card file."));
        //                    return FileStatus.UNLICENSED_BIN;
        //                }
        //            }
        //        }
        //    }
        //    #endregion

        //    #region check card records for duplication in indigo
        //    //check if card record is in the DB
        //    List<string> results = fileLoaderDAL.ValidateCardsLoaded(new List<string>(validCards.Keys));

        //    foreach (string result in results)
        //    {
        //        CardFileRecord value;
        //        //Because the results coming back are based on the keys, the dictionary should have this key, but checking just incase
        //        if (validCards.TryGetValue(result.Trim(), out value))
        //        {
        //            //card is already in DB
        //            validCards.Remove(result.Trim());
        //            log.Error("Duplicate cards in database.");
        //            fileComments.Add(new FileCommentsObject("Duplicate cards in database."));
        //            return FileStatus.DUPLICATE_CARDS_IN_DATABASE;
        //        }
        //    }
        //    #endregion

        //    cardFile.ValidCardFileRecords = validCards.Values.ToList();

        //    if (cardFile.CardFileRecords.Count == cardFile.ValidCardFileRecords.Count)
        //        return FileStatus.VALID_CARDS;
        //    else
        //        return FileStatus.PARTIAL_LOAD;
        //}

        /// <summary>
        /// Validates that the BIN is valid for the issuer
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        //private bool CheckIfIsLicenseCardRecord(string cardNumber, List<string> licensedBinCodes)
        //{
        //    for (int i = 9; i > 0; i--)
        //    {
        //        string binCode = cardNumber.Substring(0, i);

        //        foreach (var licensedBin in licensedBinCodes)
        //        {
        //            if (binCode.Trim().Equals(licensedBin.Trim()))
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    log.Warn(string.Format("Card Record: {0} is not within licensed bin range. It has been blocked.", cardNumber.Substring(0, 9)));

        //    return true;
        //}

        /// <summary>
        /// Bulk Card Request Validations
        /// </summary>
        /// <param name="cardRequestFile"></param>
        /// <param name="licensedBinCodes"></param>
        /// <param name="fileComments"></param>
        /// <returns></returns>
        internal FileStatuses ValidateCardRequestRecords(BulkRequestsFile cardRequestFile, List<string> licensedBinCodes, ref List<FileCommentsObject> fileComments)
        {
            cardRequestFile.CardRequestFileRecords = new List<BulkRequestRecord>();

            BranchLookup cardCentreBranch = null;
            Dictionary<string, BulkRequestRecord> validCards = new Dictionary<string, BulkRequestRecord>(); //<CardNumber, CardFileRecord>            
            Dictionary<string, BranchLookup> validBranches = new Dictionary<string, BranchLookup>(); //<BranchCode, BranchLookup>

            //get a list of valid bins
            #region Loop through all the cards, checking valid branchs, Valid BIN's and duplicates in the file

            foreach (BulkRequestRecord cardFileRecord in cardRequestFile.CardRequestFileRecords)
            {
                BranchLookup branchLookup;
                if (!String.IsNullOrEmpty(cardFileRecord.BranchCode))
                {
                    //See if we've already checked the DB for the branch. If it's not in the dictionary then do a read from the DB.                  
                    if (!validBranches.TryGetValue(cardFileRecord.BranchCode.Trim().ToUpper(), out branchLookup))
                    {
                        //check if branch exists in the DB                                            
                        branchLookup = fileLoaderDAL.FetchBranchByBranchCode(cardRequestFile.FileIssuer.IssuerId, cardFileRecord.BranchCode);

                        validBranches.Add(cardFileRecord.BranchCode.Trim().ToUpper(), branchLookup);
                    }
                }
                else
                {
                    //link card to card centre branch
                    if (cardCentreBranch == null)
                        branchLookup = fileLoaderDAL.FetchCardCentreBranch(cardRequestFile.FileIssuer.IssuerId);
                    else
                        branchLookup = cardCentreBranch;

                    //validBranches.Add(branchLookup.BranchCode.Trim().ToUpper(), branchLookup);
                }


                //check if branch exist
                //if not, add a record to rejected file
                if (!branchLookup.IsValid)
                {
                    //log.Error("No Valid Branch found in Indigo:\t" + branchLookup.BranchCode);
                    fileComments.Add(new FileCommentsObject("No Valid Branch found in Indigo:\t" + branchLookup.BranchCode, log.Error));
                    return FileStatuses.NO_ACTIVE_BRANCH_FOUND;
                }
                else
                {
                    cardFileRecord.BranchId = branchLookup.BranchId;
                    cardFileRecord.DomicileBranchId = branchLookup.BranchId;

                    //Check if the card has been added to the dictionary already, if it has dont add it again.
                    if (validCards.ContainsKey(cardFileRecord.RequestReferenceNumber))
                    {
                        fileComments.Add(new FileCommentsObject("Duplicate card found in card file:\t" + cardFileRecord.RequestReferenceNumber, log.Error));
                        //log.Error("Duplicate card found in card file:\t" + cardFileRecord.RequestReferenceNumber);
                        return FileStatuses.DUPLICATE_CARDS_IN_FILE;
                    }
                    else
                    {
                        var result = fileLoaderDAL.ValidateProduct(cardRequestFile.FileIssuer.IssuerId, cardFileRecord.ProductCode);

                        if (result == null)
                        {
                            fileComments.Add(new FileCommentsObject("Invalid Product details supplied:\t" + cardFileRecord.ProductCode, log.Error));
                            //log.Error("Invalid Product details supplied:\t" + cardFileRecord.ProductCode);
                            return FileStatuses.NO_PRODUCT_FOUND_FOR_CARD;
                        }
                        else
                        {
                            cardFileRecord.ProductId = result.ProductId;
                            cardFileRecord.SubProductCode = result.SubProductCode;
                            validCards.Add(cardFileRecord.RequestReferenceNumber, cardFileRecord);
                        }
                    }
                }
            }

            #endregion

            cardRequestFile.CardRequestFileRecords = validCards.Values.ToList();

            if (cardRequestFile.CardRequestFileRecords.Count == cardRequestFile.CardRequestFileRecords.Count)
                return FileStatuses.VALID_CARDS;
            else
                return FileStatuses.PARTIAL_LOAD;
        }


        /// <summary>
        /// Generates a load batch reference.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        internal string generateBatchReference(FileInfo fileInfo)
        {
            return fileInfo.Name.Substring(0, fileInfo.Name.Length - 4) + DateTimeOffset.Now.ToString("yyMMddhhmmss");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileComments"></param>
        /// <returns></returns>
        internal string BuildFileComments(List<FileCommentsObject> fileComments)
        {
            StringBuilder commentsBuilder = new StringBuilder();

            foreach (var comment in fileComments)
            {
                commentsBuilder.AppendLine(comment.GetFormatedComment());
            }

            return commentsBuilder.ToString();
        }

        internal List<string> ValidateAffiliateKey(string key, string vmgDirectory)
        {
            var licenseInfo = ValidateClientLicense.VerifyComponentLicenseKey(key, vmgDirectory);

            if (licenseInfo != null)
            {
                IndigoComponentLicense indigoComLic = (IndigoComponentLicense)licenseInfo;

                if (indigoComLic.ExpiryDate > DateTime.Now)
                {
                    return indigoComLic.LicensedBinCodes;
                }
            }

            return null;
        }
    }
}
