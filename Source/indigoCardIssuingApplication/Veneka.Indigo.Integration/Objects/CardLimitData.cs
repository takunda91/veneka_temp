using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Integration.Objects
{
    [Serializable]
    public class CardLimitData
    {
        public long CardId { get; set; }

        public string ContractNumber { get; set; }

        public string CardReferenceNumber { get; set; }

        public string AccountNumber { get; set; }

        public CardLimitData(SqlDataReader reader)
        {
            this.CardId = Convert.ToInt64(reader["card_id"]);
            this.CardReferenceNumber = reader["card_request_reference"].ToString();
            this.ContractNumber = reader["contract_number"].ToString();
            this.AccountNumber = reader["customer_account_number"].ToString();
        }
    }
}
