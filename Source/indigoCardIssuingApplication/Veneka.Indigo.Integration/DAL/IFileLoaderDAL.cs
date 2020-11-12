using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Integration.FileLoader.Objects;

namespace Veneka.Indigo.Integration.DAL
{
    public interface IFileLoaderDAL
    {
        int CreateFileLoad(DateTimeOffset startTime, int filesToProcess, long auditUserId, string auditWorkstation);
        void UpdateFileLoad(int fileLoadId, long auditUserId, string auditWorkstation);
         BranchLookup FetchCardCentreBranch(int issuerId);
        BranchLookup FetchBranchByBranchCode(int issuerId, string branchCode);

        List<CardsOrder> FetchOutstandingOrder(int productId, int numberOfCards, long auditUserId, string auditWorkstation);

        List<CardOrderCard> FetchCardsForOrder(long distBatchId, long auditUserId, string auditWorkstation);

        List<string> ValidateCardsLoaded(List<string> cardList);
         List<string> ValidateCardReferencesLoaded(List<string> cardRefList);

         List<Tuple<long, int>> ValidateCardsOrdered(List<Tuple<long, string, int>> cardList);



        bool SaveFileInfo(FileHistory fileHistory);

         bool CreateBulkRequestLoadBatch(BulkRequestsFile cardRequestFile, string loadBatchReference, FileHistory fileHistory, long auditUserId, string auditWorkstation);
    }
}
