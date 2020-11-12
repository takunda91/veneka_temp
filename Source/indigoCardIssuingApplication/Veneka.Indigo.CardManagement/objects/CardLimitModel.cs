using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.CardManagement.objects
{
    public class CardLimitModel
    {
        public long CardLimitId { get; set; }

        public long CardId { get; set; }

        public decimal Limit { get; set; }

        public decimal? LimitApproved { get; set; }

        public string ContractNumber { get; set; }

        public long? CreditAnalystId { get; set; }

        public long? CreditManagerId { get; set; }
    }
}
