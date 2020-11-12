using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.FileLoader.Objects
{
    public class CardOrderCard
    {
        public long CardId { get; private set; }

        public CardOrderCard(long cardId)
        {
            this.CardId = cardId;
        }

        public CardOrderCard(System.Data.SqlClient.SqlDataReader reader)
        {
            this.CardId = reader["card_id"] as long? ?? 0;
        }
    }
}
