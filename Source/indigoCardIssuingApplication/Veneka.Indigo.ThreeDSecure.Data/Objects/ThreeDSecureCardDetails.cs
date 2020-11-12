using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.ThreeDSecure.Data.Objects
{
   public class ThreeDSecureCardDetails
    {
        public long ThreedsBatchId { get; set; }
        public string CardNumber { get; set; }
        public string NameOnCard { get; set; }
        public DateTime?  CardExpiryDate { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustoemrMiddleName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerTitle { get; set; }
        public string ContactNumber { get; set; }
        public string ContactEmail { get; set; }
        public string CustomerAccountNumber { get; set; }
        public string product_prefix { get; set; }
        public string issuer_code { get; set; }

        public static List<ThreeDSecureCardDetails> FillObject(DataTable table)
        {
            List<ThreeDSecureCardDetails> results = new List<ThreeDSecureCardDetails>();

            foreach (DataRow row in table.Rows)
            {
                ThreeDSecureCardDetails record = new ThreeDSecureCardDetails();

                record.ThreedsBatchId = row.Field<long>("threeds_batch_id");
                record.CardNumber = row.Field<string>("card_number");
                record.NameOnCard = row.Field<string>("name_on_card");
                record.CardExpiryDate = row.Field<DateTime?>("card_expiry_date");                
                record.CustomerFirstName = row.Field<string>("customer_first_name");
                record.CustoemrMiddleName = row.Field<string>("customer_middle_name");
                record.CustomerLastName = row.Field<string>("customer_last_name");
                record.CustomerTitle = row.Field<string>("customer_title_name");
                record.ContactNumber = row.Field<string>("contact_number");
                record.ContactEmail = row.Field<string>("customer_email");
                

                results.Add(record);
            }

            return results;
        }

        public static List<ThreeDSecureCardDetails> FillCustomersObject(DataTable table)
        {
            List<ThreeDSecureCardDetails> results = new List<ThreeDSecureCardDetails>();

            foreach (DataRow row in table.Rows)
            {
                ThreeDSecureCardDetails record = new ThreeDSecureCardDetails();

               
                record.CustomerFirstName = row.Field<string>("customer_first_name");
                record.CustoemrMiddleName = row.Field<string>("customer_middle_name");
                record.CustomerLastName = row.Field<string>("customer_last_name");
                record.ContactNumber = row.Field<string>("contact_number");
                record.ContactEmail = row.Field<string>("customer_email");
                record.CustomerAccountNumber = row.Field<string>("customer_account_number");


                results.Add(record);
            }

            return results;
        }

        public static List<ThreeDSecureCardDetails> FillFileHeaderDetails(string issuer_code, string product_prefix)
        {
            List<ThreeDSecureCardDetails> results = new List<ThreeDSecureCardDetails>();

                ThreeDSecureCardDetails record = new ThreeDSecureCardDetails();
            record.issuer_code = issuer_code;
            record.product_prefix = product_prefix;
                results.Add(record);
            
            return results;
        }

    }    
}
