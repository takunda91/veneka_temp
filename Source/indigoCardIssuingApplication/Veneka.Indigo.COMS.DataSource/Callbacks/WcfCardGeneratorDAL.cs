using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.COMS.Core;
using Veneka.Indigo.Integration.DAL;

namespace Veneka.Indigo.COMS.DataSource.Callbacks
{
    public class WcfCardGeneratorDAL: ICardGeneratorDAL
    {
        private readonly IComsCallback _proxy;
        public WcfCardGeneratorDAL(IComsCallback proxy)
        {
            _proxy = proxy;
        }

        public int GetLatestSequenceNumber(int productId, long auditUserId, string auditWorkStation)
        {
            return _proxy.GetLatestSequenceNumber(productId, auditUserId, auditWorkStation);
        }

        public void UpdateCardsAndSequenceNumber(Dictionary<long, string> cards, Dictionary<int, int> products, long auditUserId, string auditWorkStation)
        {
            _proxy.UpdateCardsAndSequenceNumber(cards, products, auditUserId, auditWorkStation);
        }
    }
}
