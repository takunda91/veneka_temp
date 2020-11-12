using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.Serialization;

namespace Veneka.Indigo.Integration.FileLoader.Objects
{
    [Serializable]
    [DataContract]
    public class Issuer
    {
        public int IssuerId { get; private set; }
        public string IssuerName { get; private set; }
        public string IssuerCode { get; private set; }
        public bool IsActive { get; private set; }

        //public bool isInstantCardIssue { get; private set; }
        //public bool DeleteCardFile { get; private set; }
        //public bool isClassicCardIssue { get; private set; }
        public string LicenceKey { get; private set; }

        public Issuer(SqlDataReader reader)
        {
            this.IssuerId = int.Parse(reader["issuer_id"].ToString());
            this.IssuerName = reader["issuer_name"].ToString();
            this.IssuerCode = reader["issuer_code"].ToString();
            this.IsActive = reader["issuer_status_id"].ToString() == "0" ? true : false;

            //this.isInstantCardIssue = bool.Parse(reader["instant_card_issue_YN"].ToString());
            //this.DeleteCardFile = bool.Parse(reader["delete_card_file_YN"].ToString());
            //this.isClassicCardIssue = bool.Parse(reader["classic_card_issue_YN"].ToString());
            if (HasColumn(reader, "license_key"))
                this.LicenceKey = reader["license_key"] != null ? reader["license_key"].ToString() : String.Empty;
        }

        private static bool HasColumn(IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
