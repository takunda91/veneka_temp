using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Veneka.Indigo.Integration.FileLoader.Objects
{
    internal class IssuerProduct
    {        
        public int ProductId { get; private set; }
        public string SubProductCode { get; private set; }
        public int CardIssueMethodId { get; private set; }
        //public int? SubProductIdLength { get; private set; }

        public string BIN { get; private set; }

        public IssuerProduct(SqlDataReader reader)
        {
            this.ProductId = int.Parse(reader["product_id"].ToString());
            this.BIN = (reader["product_bin_code"] as string).Trim();
            this.CardIssueMethodId = int.Parse(reader["card_issue_method_id"].ToString());
            this.SubProductCode = reader["sub_product_code"] as string;
            //this.SubProductIdLength = reader["sub_product_id_length"] as int?;

            //if (this.SubProductCode != null)
            //    this.BIN += this.SubProductCode.ToString().PadLeft(this.SubProductIdLength.Value, '0');
            
        }
    }
}
