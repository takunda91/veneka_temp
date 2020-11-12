using System.Collections.Generic;
using System.ServiceModel;

namespace Veneka.Indigo.Integration.DAL
{

    [ServiceContract]
    public interface ICardGeneratorDAL
    {
        [OperationContract]
        int GetLatestSequenceNumber(int productId, long auditUserId, string auditWorkStation);
        [OperationContract]
        void UpdateCardsAndSequenceNumber(Dictionary<long, string> cards, Dictionary<int, int> products, long auditUserId, string auditWorkStation);
    }
}