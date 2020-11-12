using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.COMS.Core;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.COMS.DataSource.Callbacks
{
    public class WcfCardsDAL : ICardsDAL
    {
        private readonly IComsCallback _proxy;
        public WcfCardsDAL(IComsCallback proxy)
        {
            _proxy = proxy;
        }

        public CardObject GetCardByPan(string pan, int mbr, string referenceNumber, long auditUserId, string auditWorkstation)
        {
            return _proxy.GetCardByPan(pan, mbr, referenceNumber, auditUserId, auditWorkstation);
        }

        public CardLimitData GetCardLimitDataByContractNumber(string contractNumber)
        {
            return _proxy.GetCardLimitDataByContractNumber(contractNumber);
        }

        public CardObject GetCardObject(long cardId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _proxy.GetCardObject(cardId, languageId, auditUserId, auditWorkstation);
        }

        public CardObject GetCardObjectFromExport(long exportBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _proxy.GetCardObjectFromExport(exportBatchId, languageId, auditUserId, auditWorkstation);
        }

        public List<CardObject> GetCardsByAccNo(int productId, string accountNumber, long auditUserId, string auditWorkStation)
        {
            return _proxy.GetCardsByAccNo(productId, accountNumber, auditUserId, auditWorkStation);
        }

        public void UpdateCardFeeReferenceNumber(long cardId, string referenceNumber, long auditUserId, string auditWorkStation)
        {
            _proxy.UpdateCardFeeReferenceNumber(cardId, referenceNumber, auditUserId, auditWorkStation);
        }

        public void UpdateCardFeeReversalReferenceNumber(long cardId, string referenceNumber, long auditUserId, string auditWorkStation)
        {
            _proxy.UpdateCardFeeReferenceNumber(cardId, referenceNumber, auditUserId, auditWorkStation);

        }
    }

}
