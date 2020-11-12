using System.Collections.Generic;
using System.ServiceModel;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.Integration.DAL
{
    [ServiceContract]
    public interface ICardsDAL
    {
        [OperationContract]
        CardObject GetCardObject(long cardId, int languageId, long auditUserId, string auditWorkstation);
        [OperationContract]
        CardObject GetCardObjectFromExport(long exportBatchId, int languageId, long auditUserId, string auditWorkstation);
        [OperationContract]
        void UpdateCardFeeReferenceNumber(long cardId, string referenceNumber, long auditUserId, string auditWorkStation);
        [OperationContract]
        void UpdateCardFeeReversalReferenceNumber(long cardId, string referenceNumber, long auditUserId, string auditWorkStation);

        [OperationContract]
        List<CardObject> GetCardsByAccNo(int productId, string accountNumber, long auditUserId, string auditWorkStation);

        [OperationContract]
        CardLimitData GetCardLimitDataByContractNumber(string contractNumber);

        [OperationContract]
        CardObject GetCardByPan(string pan, int mbr, string referenceNumber, long auditUserId, string auditWorkstation);
    }
}