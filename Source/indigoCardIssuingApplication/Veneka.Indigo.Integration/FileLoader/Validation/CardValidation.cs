using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.FileLoader.DAL;
using Veneka.Indigo.Integration.FileLoader.Objects;

namespace Veneka.Indigo.Integration.FileLoader.Validation
{
    public class CardValidation
    {
        private const string DUPLICATE_CARDS_IN_FILE = "Duplicate cards found in file: {0}.";
        private const string MULTIPLE_PRODUCTS_IN_FILE = "Multiple card products found in file, file may only have cars for a single product.";
        private const string DUPLICATE_CARDS_IN_DB = "Duplicate cards found in database: {0}.";        
        private const string CARDS_NOT_ORDERED = "Cards in file have not been ordered through Indigo.";
        private const string ORDER_CARD_MISSING_REF = "Ordered Cards missing Indigo reference.";
        private const string ORDER_CARD_PRODUCT_MISS_MATCH = "Ordered Cards product miss match.";

        private readonly ILog _logger;
        private readonly FileLoaderDAL _fileLoaderDal;

        public CardValidation(string connectionString, string logger)
        {
            _fileLoaderDal = new FileLoaderDAL(connectionString);
            _logger = LogManager.GetLogger(logger);
        }

        public FileStatuses ValidateDuplicatesInFile(CardFile cardFile, List<FileCommentsObject> fileComments)
        {
            fileComments.Add(new FileCommentsObject("Checking for duplicate cards in file.", _logger.Info));
            var groups = cardFile.CardFileRecords.GroupBy(g => g.PAN);
            StringBuilder lineNumbers = new StringBuilder();
            bool hasDuplicates = false;

            foreach (var group in groups)
            {
                if (group.Count() > 1)
                {                    
                    foreach (var item in group)
                    {
                        if (hasDuplicates)
                            lineNumbers.Append(", ");

                        hasDuplicates = true;
                        lineNumbers.Append(item.LineNumber);
                    }
                }
            }

            if(hasDuplicates) // we got duplicates
            {
                //_logger.Error(fileComment = DUPLICATE_CARDS_IN_FILE);
                fileComments.Add(new FileCommentsObject(String.Format(DUPLICATE_CARDS_IN_FILE, lineNumbers.ToString()), _logger.Error));
                return FileStatuses.DUPLICATE_CARDS_IN_FILE;
            }

            return FileStatuses.READ;
        }

        public FileStatuses ValidateSingleProductInFile(CardFile cardFile, List<FileCommentsObject> fileComments)
        {
            if (cardFile.OrderBatchId!=null && cardFile.OrderBatchId.GetValueOrDefault() == -99)
            {
                fileComments.Add(new FileCommentsObject("Product integration indicates multiple products in file are permitted.", _logger.Info));
                return FileStatuses.READ;
            }
            
            fileComments.Add(new FileCommentsObject("Checking only one card product in file.", _logger.Info));

            if (cardFile.CardFileRecords.GroupBy(g => g.ProductId.Value).Count() > 1)
            {
                fileComments.Add(new FileCommentsObject(MULTIPLE_PRODUCTS_IN_FILE, _logger.Error));
                return FileStatuses.MULTIPLE_PRODUCTS_IN_FILE;
            }

            return FileStatuses.READ;
        }

        public FileStatuses ValidateDuplicatesInFile(BulkRequestsFile cardRequestFile, List<FileCommentsObject> fileComments)
        {
            //string fileComment = String.Empty;
            var groups = cardRequestFile.CardRequestFileRecords.GroupBy(g => g.RequestReferenceNumber);
            StringBuilder lineNumbers = new StringBuilder();
            bool hasDuplicates = false;

            foreach (var group in groups)
            {
                if (group.Count() > 1)
                {
                    foreach (var item in group)
                    {
                        if (hasDuplicates)
                            lineNumbers.Append(", ");

                        hasDuplicates = true;
                        lineNumbers.Append(item.LineNumber);
                    }                    
                }
            }

            if (hasDuplicates) // we got duplicates
            {
                //_logger.Error(fileComment = DUPLICATE_CARDS_IN_FILE);
                fileComments.Add(new FileCommentsObject(String.Format(DUPLICATE_CARDS_IN_FILE, lineNumbers.ToString()), _logger.Error));
                return FileStatuses.DUPLICATE_CARDS_IN_FILE;
            }

            return FileStatuses.READ;
        }

        /// <summary>
        /// Checks for duplicate card numbers/PANS
        /// </summary>
        /// <param name="cardFile"></param>
        /// <param name="fileComments"></param>
        /// <returns></returns>
        public FileStatuses ValidateDuplicatesInDB(BulkRequestsFile cardFile, List<FileCommentsObject> fileComments)
        {
            //string fileComment = String.Empty;
            //check if card record is in the DB
            List<string> results = _fileLoaderDal.ValidateCardsLoaded(cardFile.CardRequestFileRecords.Select(s => s.CardNumber).ToList());

            StringBuilder lineNumbers = new StringBuilder();
            bool hasDuplicates = false;

            if (results.Count > 0)
            {
                foreach (var result in results)
                {
                    if (hasDuplicates)
                        lineNumbers.Append(", ");

                    hasDuplicates = true;
                    lineNumbers.Append(result);
                }

                //_logger.Error(fileComment = DUPLICATE_CARDS_IN_DB);
                fileComments.Add(new FileCommentsObject(String.Format(DUPLICATE_CARDS_IN_DB, lineNumbers.ToString()), _logger.Error));
                return FileStatuses.DUPLICATE_CARDS_IN_DATABASE;
            }

            return FileStatuses.READ;
        }

        /// <summary>
        /// Checks for duplicate card numbers/PANS
        /// </summary>
        /// <param name="cardFile"></param>
        /// <param name="fileComments"></param>
        /// <returns></returns>
        public FileStatuses ValidateDuplicatesInDB(CardFile cardFile, List<FileCommentsObject> fileComments)
        {
            fileComments.Add(new FileCommentsObject("Checking for duplicate cards in DB.", _logger.Info));
            //string fileComment = String.Empty;
            //check if card record is in the DB
            List<string> results = _fileLoaderDal.ValidateCardsLoaded(cardFile.CardFileRecords.Select(s => s.PAN).ToList());

            StringBuilder lineNumbers = new StringBuilder();
            bool hasDuplicates = false;

            if (results.Count > 0)
            {
                foreach (var result in results)
                {
                    if (hasDuplicates)
                        lineNumbers.Append(", ");

                    hasDuplicates = true;
                    lineNumbers.Append(result);
                }

                //_logger.Error(fileComment = DUPLICATE_CARDS_IN_DB);
                fileComments.Add(new FileCommentsObject(String.Format(DUPLICATE_CARDS_IN_DB, lineNumbers.ToString()), _logger.Error));
                return FileStatuses.DUPLICATE_CARDS_IN_DATABASE;
            }

            return FileStatuses.READ;
        }

        /// <summary>
        /// Checks for duplicate reference numbers
        /// </summary>
        /// <param name="CardRequestFile"></param>
        /// <param name="fileComments"></param>
        /// <returns></returns>
        public FileStatuses ValidateReferenceDuplicatesInDB(BulkRequestsFile CardRequestFile, List<FileCommentsObject> fileComments)
        {
            //string fileComment = String.Empty;
            //check if card reference record is in the DB
            List<string> results = _fileLoaderDal.ValidateCardReferencesLoaded(CardRequestFile.CardRequestFileRecords.Select(s => s.RequestReferenceNumber).ToList());

            StringBuilder lineNumbers = new StringBuilder();
            bool hasDuplicates = false;

            if (results.Count > 0)
            {
                foreach (var result in results)
                {
                    if (hasDuplicates)
                        lineNumbers.Append(", ");

                    hasDuplicates = true;
                    lineNumbers.Append(result);
                }

                //_logger.Error(fileComment = DUPLICATE_CARDS_IN_DB);
                fileComments.Add(new FileCommentsObject(DUPLICATE_CARDS_IN_DB, _logger.Error));
                return FileStatuses.DUPLICATE_CARDS_IN_DATABASE;
            }

            return FileStatuses.READ;
        }

        public FileStatuses ValidateCardsOrdered(CardFile cardFile, List<FileCommentsObject> fileComments)
        {
            //string fileComment = String.Empty;

            //Check that the link between ordered cards exists
            if (cardFile.CardFileRecords
                    .Where(w => w.ProductLoadTypeId.GetValueOrDefault(0) == 4)
                    .Any(a => a.CardId == null))
            {
                //_logger.Error(fileComment = ORDER_CARD_MISSING_REF);
                fileComments.Add(new FileCommentsObject(ORDER_CARD_MISSING_REF, _logger.Error));
                return FileStatuses.ORDERED_CARD_REF_MISSING;
            }

            List<Tuple<long, string, int>> orderedCards = cardFile.CardFileRecords
                                                        .Where(w => w.ProductLoadTypeId.GetValueOrDefault(0) == 4)
                                                        .Select(s => new Tuple<long, string, int>(s.CardId.Value, s.PAN, s.ProductId.Value)).ToList();

            //check if the cards have been ordered in Indigo
            if (orderedCards.Count > 0)
            {
                List<Tuple<long, int>> results = _fileLoaderDal.ValidateCardsOrdered(orderedCards);

                //check that all cards in the file were ordered on indigo
                foreach (var card in orderedCards)
                {

                    var item = results.Where(w => w.Item1 == card.Item1).SingleOrDefault();

                    if (item != null)
                    {
                        if (item.Item2 != card.Item3)
                        {
                            //_logger.Error(fileComment = ORDER_CARD_PRODUCT_MISS_MATCH);
                            fileComments.Add(new FileCommentsObject(ORDER_CARD_PRODUCT_MISS_MATCH, _logger.Error));
                            return FileStatuses.ORDERED_CARD_PRODUCT_MISS_MATCH;
                        }
                    }
                    else
                    {
                        //_logger.Error(fileComment = CARDS_NOT_ORDERED);
                        fileComments.Add(new FileCommentsObject(CARDS_NOT_ORDERED, _logger.Error));
                        return FileStatuses.CARDS_NOT_ORDERED;
                    }
                }
            }

            return FileStatuses.READ;
        }

        public FileStatuses ValidateCardsOrdered(BulkRequestsFile cardRequestFile, List<FileCommentsObject> fileComments)
        {
            //string fileComment = String.Empty;

            //Check that the link between ordered cards exists
            if (cardRequestFile.CardRequestFileRecords
                    .Where(w => w.ProductLoadTypeId.GetValueOrDefault(0) == 4)
                    .Any(a => a.CardId == null))
            {
                //_logger.Error(fileComment = ORDER_CARD_MISSING_REF);
                fileComments.Add(new FileCommentsObject(ORDER_CARD_MISSING_REF, _logger.Error));
                return FileStatuses.ORDERED_CARD_REF_MISSING;
            }

            List<Tuple<long, string, int>> orderedCards = cardRequestFile.CardRequestFileRecords
                                                        .Where(w => w.ProductLoadTypeId.GetValueOrDefault(0) == 4)
                                                        .Select(s => new Tuple<long, string, int>(s.CardId, s.RequestReferenceNumber, s.ProductId.Value)).ToList();

            //check if the cards have been ordered in Indigo
            if (orderedCards.Count > 0)
            {
                List<Tuple<long, int>> results = _fileLoaderDal.ValidateCardsOrdered(orderedCards);

                //check that all cards in the file were ordered on indigo
                foreach (var card in orderedCards)
                {
                    var item = results.Where(w => w.Item1 == card.Item1).SingleOrDefault();

                    if (item != null)
                    {
                        if (item.Item2 != card.Item3)
                        {
                            //_logger.Error(fileComment = ORDER_CARD_PRODUCT_MISS_MATCH);
                            fileComments.Add(new FileCommentsObject(ORDER_CARD_PRODUCT_MISS_MATCH, _logger.Error));
                            return FileStatuses.ORDERED_CARD_PRODUCT_MISS_MATCH;
                        }
                    }
                    else
                    {
                        //_logger.Error(fileComment = CARDS_NOT_ORDERED);
                        fileComments.Add(new FileCommentsObject(CARDS_NOT_ORDERED, _logger.Error));
                        return FileStatuses.CARDS_NOT_ORDERED;
                    }
                }
            }

            return FileStatuses.READ;
        }
    }
}
