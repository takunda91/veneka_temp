using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.FileLoader.DAL;
using Veneka.Indigo.Integration.FileLoader.Objects;

namespace Veneka.Indigo.Integration.FileLoader.BLL
{
    internal class LoadToExisting
    {
        private readonly ILog log = LogManager.GetLogger(typeof(LoadToExisting));   
        private readonly FileLoaderDAL _fileLoaderDal;
        public LoadToExisting(string connectionString)
        {
            _fileLoaderDal = new FileLoaderDAL(connectionString);
        }

        public bool FindMatchingOrder(CardFile cardFile, long auditUserId, string auditWorkstation)
        {
            var orders = _fileLoaderDal.FetchOutstandingOrder(cardFile.CardFileRecords[0].ProductId.GetValueOrDefault(), cardFile.CardFileRecords.Count, auditUserId, auditWorkstation);

            if(orders.Count > 0)
            {
                log.Debug("Orders found: count="+orders.Count);

                var minOrderDate = orders.Min(m => m.CreatedOn);
                var oldestOrder = orders.Where(w => w.CreatedOn == minOrderDate).First();

                log.Debug("Getting Cards for order: " + oldestOrder.DistBatchId);
                //Get the cards linked to the outstanding order
                var orderedCards = _fileLoaderDal.FetchCardsForOrder(oldestOrder.DistBatchId, auditUserId, auditWorkstation);

                cardFile.OrderBatchreference = oldestOrder.DistBatchRef;
                cardFile.OrderBatchId = oldestOrder.DistBatchId;

                int index = 0;
                foreach (var card in orderedCards)
                { 
                    cardFile.CardFileRecords[index].CardId = card.CardId;

                    log.Debug("Card - " + card.CardId + " = " + cardFile.CardFileRecords[index].CardId);
                    index++;
                }

                return true;
            }

            return false;
        }
    }
}
