using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.ThreeDSecure.Data.Objects
{
   public class ThreeDSecureBatch
    {
        public long ThreedsBatchId { get; set; }
        public string BatchReference { get; set; }
        public DateTime  DateCreated { get; set; }
        public int NumberOfCards { get; set; }

        public static List<ThreeDSecureBatch> FillObject(DataTable table)
        {
            List<ThreeDSecureBatch> results = new List<ThreeDSecureBatch>();

            foreach (DataRow row in table.Rows)
            {
                ThreeDSecureBatch record = new ThreeDSecureBatch();

                record.ThreedsBatchId = row.Field<long>("threed_batch_id");
                record.BatchReference = row.Field<string>("batch_reference");
                record.DateCreated = Convert.ToDateTime(row["date_created"]);
                record.NumberOfCards = row.Field<int>("no_cards");                

                results.Add(record);
            }

            return results;
        }
    }    
}
