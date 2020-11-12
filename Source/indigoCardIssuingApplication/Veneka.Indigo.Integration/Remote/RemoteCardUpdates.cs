using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Data;

namespace Veneka.Indigo.Integration.Remote
{
    [DataContract]
    public class RemoteCardUpdates
    {
        public RemoteCardUpdates() : this(new Response())
        {   }

        public RemoteCardUpdates(Response response)
        {
            Response = response;
            Cards = new List<CardDetail>();
            ProductSettings = new List<Settings>();
        }

        public void MakeSuccess()
        {
            this.Response.MakeSuccess();
        }

        [DataMember(IsRequired = true)]
        public Response Response { get; set; }

        [DataMember]
        public List<CardDetail> Cards { get; set; }

        [DataMember]
        public List<Settings> ProductSettings { get; set; }
    }

    [DataContract]
    public class Response
    {
        public Response() : this(99, "Failed to process request.")
        {   }

        public Response(int responseCode, string responseMessage)
        {
            ResponseCode = responseCode;
            ResponseMessage = responseMessage;
        }

        public void MakeSuccess()
        {
            this.ResponseCode = 0;
            this.ResponseMessage = "Success";
        }

        [DataMember(IsRequired = true)]
        public int ResponseCode { get; set; }

        [DataMember]
        public string ResponseMessage { get; set; }
    }

    [DataContract]
    public class Settings
    {
        public Settings()
        {
            ExternalFields = new External.ExternalSystemFields
            {
                Field = new Dictionary<string, string>()
            };
        }

        public Settings(System.Data.Common.DbDataReader reader) : this()
        {
            ProductId = (int)reader["product_id"];
            IntegrationTypeId = (int)reader["interface_type_id"];
            IntegrationGuid = reader["interface_guid"].ToString();
            ConfigType = (int)reader["connection_parameter_type_id"];

            DataTable schemaTable = reader.GetSchemaTable();
            DataTable data = new DataTable();
            foreach (DataRow row in schemaTable.Rows)
            {
                string colName = row.Field<string>("ColumnName");
                Type t = row.Field<Type>("DataType");
                data.Columns.Add(colName, t);
            }

            var newRow = data.Rows.Add();
            foreach (DataColumn col in data.Columns)
            {
                newRow[col.ColumnName] = reader[col.ColumnName];
            }

            var config = Config.ConfigFactory.GetConfig(newRow);

            if (config is Config.WebServiceConfig)
                WebServiceSettings = (Config.WebServiceConfig)config;
            else if (config is Config.SocketConfig)
                SocketSettings = (Config.SocketConfig)config;
            else if (config is Config.FileSystemConfig)
                FileSystemSettings = (Config.FileSystemConfig)config;
        }


        [DataMember(IsRequired = true)]
        public int ProductId { get; set; }

        [DataMember(IsRequired = true)]
        public int IntegrationTypeId { get; set; }

        [DataMember(IsRequired = true)]
        public string IntegrationGuid { get; set; }

        [DataMember(IsRequired = true)]
        public int ConfigType { get; set; }

        [DataMember]
        public Config.WebServiceConfig WebServiceSettings { get; set; }

        [DataMember]
        public Config.SocketConfig SocketSettings { get; set; }

        [DataMember]
        public Config.FileSystemConfig FileSystemSettings { get; set; }

        [DataMember]
        public External.ExternalSystemFields ExternalFields { get; set; }
    }

    [DataContract]
    public class CardDetail
    {
        public CardDetail() { }

        public CardDetail(System.Data.Common.DbDataReader reader)
        {
            var entityProperties = TypeDescriptor.GetProperties(this).Cast<PropertyDescriptor>();

            foreach (var entityProperty in entityProperties)
            {
                try
                {
                    var value = reader[entityProperty.Name];

                    if (value != null && value != DBNull.Value)
                    {
                        entityProperty.SetValue(this, value);
                    }
                }
                catch(Exception ex)
                {
                    throw new Exception(entityProperty.Name, ex);
                }
            }
        }


        [DataMember(IsRequired = true)]
        public long card_id { get; set; }

        [DataMemberAttribute()]
        public string card_number { get; set; }

        [DataMember]
        public DateTime? card_expiry_date { get; set; }

        [DataMemberAttribute()]
        public string issuer_name { get; set; }

        [DataMemberAttribute()]
        public string issuer_code { get; set; }

        [DataMemberAttribute()]
        public string branch_name { get; set; }

        [DataMemberAttribute()]
        public string branch_code { get; set; }

        [DataMemberAttribute()]
        public string customer_account_number { get; set; }

        [DataMemberAttribute()]
        public int? card_issue_reason_id { get; set; }

        //[DataMemberAttribute()]
        //public string customer_account_type_name { get; set; }

        [DataMemberAttribute()]
        public string customer_first_name { get; set; }

        [DataMemberAttribute()]
        public string customer_middle_name { get; set; }

        [DataMemberAttribute()]
        public string customer_last_name { get; set; }

        [DataMemberAttribute()]
        public DateTime date_issued { get; set; }

        [DataMemberAttribute()]
        public string name_on_card { get; set; }

        [DataMemberAttribute()]
        public string cms_id { get; set; }

        [DataMemberAttribute()]
        public int? resident_id { get; set; }

        [DataMemberAttribute()]
        public int? customer_type_id { get; set; }

        [DataMemberAttribute()]
        public string contract_number { get; set; }

        [DataMember(IsRequired = true)]
        public int product_id { get; set; }

        [DataMemberAttribute()]
        public string product_name { get; set; }

        [DataMemberAttribute()]
        public string product_code { get; set; }

        [DataMemberAttribute()]
        public int? account_type_id { get; set; }

        [DataMemberAttribute()]
        public int? customer_title_id { get; set; }

        //[DataMemberAttribute()]
        //public string customer_title_name { get; set; }

        //[DataMemberAttribute()]
        //public string customer_residency_name { get; set; }

        //[DataMemberAttribute()]
        //public string customer_type_name { get; set; }

        [DataMemberAttribute()]
        public string card_request_reference { get; set; }

        [DataMemberAttribute()]
        public global::System.Int32 card_priority_id { get; set; }

        [DataMemberAttribute()]
        public string id_number { get; set; }

        [DataMemberAttribute()]
        public string contact_number { get; set; }

        //[DataMemberAttribute()]
        //public string domicile_branch_code { get; set; }

        //[DataMemberAttribute()]
        //public string domicile_branch_name { get; set; }

        [DataMemberAttribute()]
        public string pvv { get; set; }

        [DataMemberAttribute()]
        public string currency_code { get; set; }


        [DataMemberAttribute()]
        public string iso_4217_numeric_code { get; set; }


        [DataMemberAttribute()]
        public string country_code { get; set; }


        [DataMemberAttribute()]
        public string country_name { get; set; }

        [DataMemberAttribute()]
        public string product_bin_code { get; set; }

        [DataMemberAttribute()]
        public string sub_product_code { get; set; }

        [DataMemberAttribute()]
        public string CustomerId { get; set; }        
    }
}