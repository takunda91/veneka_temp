using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Veneka.Indigo.Integration.ProductPrinting;

namespace Veneka.Indigo.Integration.Objects
{
    public enum CardType
    {
        VISA,
        MASTERCARD,
        CHINAUNIONPAY,
        DINNERSCLUB,
        AMERICANEXPRESS
    }
    [KnownType(typeof(ProductField))]
    [KnownType(typeof(AccountDetails))]

    [Serializable]
    [DataContract]
    public class CardObject
    {

        [DataMember]
        public long CardId { get; set; }
        [DataMember]
        public long DistBatchId { get; set; }
        [DataMember]
        public string DistBatchReference { get; set; }
        [DataMember]
        public int DistCardStatusId { get; set; }
        [DataMember]
        public List<PrintStringField> PrintStringField
        {
            get; set;
        }
        public List<PrintImageField> PrintImageFields
        {
            get
            {
                List<PrintImageField> _printfieldlist = new List<PrintImageField>();
                foreach (var item in PrintFields)
                {
                    if (item is PrintImageField)
                    {
                        _printfieldlist.Add((PrintImageField)item);
                    }
                }
                return _printfieldlist;
            }
            set
            {
                foreach (var item in value)
                {
                    if (item is PrintImageField)
                    {
                        PrintFields = new List<IProductPrintField>();
                        PrintFields.Add(item);
                    }
                }
            }
        }
        [DataMember]
        public List<PrintStringField> PrintStringFields
        {
            get
            {
                List<PrintStringField> _printfieldlist = new List<PrintStringField>();
                foreach (var item in PrintFields)
                {
                    if (item is PrintStringField)
                    {
                        _printfieldlist.Add((PrintStringField)item);
                    }
                }
                return _printfieldlist;
            }
            set
            {
                PrintFields = new List<IProductPrintField>();
                foreach (var item in value)
                {
                    if (item is PrintStringField)
                    {

                        PrintFields.Add(item);
                    }
                }
            }
        }
        public List<IProductPrintField> PrintFields
        {
            get; set;
        }



        private string cardNumber;
        public string CardNumber
        {
            get
            {
                return cardNumber;
            }
            set
            {
                ///if (String.IsNullOrWhiteSpace(cardNumber))
                this.cardNumber = value;
                // else
                //    throw new ArgumentException("Cannot set CardNumber property when property has already been set.");
            }
        }
        [DataMember]
        public string CardReferenceNumber { get; private set; }
        [DataMember]
        public DateTime? CardIssuedDate { get; set; }
        [DataMember]
        public DateTime? CardActivatedDate { get; set; }
        private DateTime? expiryDate;
        [DataMember]
        public DateTime? ExpiryDate
        {
            get
            {
                return expiryDate;
            }
            set
            {
                if (expiryDate == null)
                    this.expiryDate = value;
                else
                    throw new ArgumentException("Cannot set ExpiryDate property when property has already been set.");
            }
        }

        //public CardType CardAuthority { get; private set; }
        [DataMember]
        public int InterchangeRules { get; private set; }
        [DataMember]
        public int AuthorisationProcessing { get; private set; }
        public int RangeOfServices { get; private set; }
        [DataMember]
        public int CardIssueMethodId { get; set; }
        [DataMember]
        public int IssuerId { get; private set; }
        public string IssuerCode { get; private set; }
        public string IssuerName { get; private set; }

        public string DeliveryBranchCode { get; set; }
        public string DeliveryBranchName { get; set; }
        public int BranchId { get; private set; }
        public string BranchCode { get; private set; }
        public string BranchName { get; private set; }
        public int sequenceNumber { get; private set; }
        public string EMPDeliveryBranchCode { get; set; }

        #region Product Specific Properties
        public int ProductId { get; private set; }
        public string ProductCode { get; private set; }
        public string ProductName { get; set; }
        public string BIN { get; private set; }
        public string SubProductCode { get; set; }
        public int PANLength { get; private set; }
        public int ExpiryMonths { get; set; }
        public string ServiceCodes
        {
            get
            {
                return "" + InterchangeRules + AuthorisationProcessing + RangeOfServices;
            }
        }
        #endregion

        #region HSM Specific Properties             

        public int SecurePINLengthOrPVKI { get; private set; }
        public string PVK { get; private set; }
        private string pvv;
        public string PVV
        {
            get
            {
                return pvv;
            }
            set
            {
                // if (String.IsNullOrWhiteSpace(pvv))
                this.pvv = value;
                // else
                //     throw new ArgumentException("Cannot set PVV property when property has already been set.");
            }
        }

        public string VisaCVKA { get; private set; }
        public string VisaCVKB { get; private set; }

        #endregion
        [DataMember]
        public AccountDetails CustomerAccount { get; set; }

        public CardObject() { }

        public CardObject(long cardId, string cardRefNo, int issuerId, string issuerCode, string issuerName, int branchId, string branchCode,
                            string branchName, int productId, string productCode, string bin, string subProductCode, int panLength,
                            int interchangeRules, int authorisationProcessing, int rangeOfServices,
                            int securePINLengthOrPVKI, string pvk, string visaCVKA, string visaCVKB,
                            int sequenceNumber)
        {
            this.CardId = cardId;
            this.CardReferenceNumber = cardRefNo;
            this.IssuerId = issuerId;
            this.IssuerCode = issuerCode;
            this.IssuerName = issuerName;
            this.BranchId = branchId;
            this.BranchCode = branchCode;
            this.BranchName = branchName;
            this.ProductId = productId;
            this.ProductCode = productCode;
            this.BIN = bin;
            this.SubProductCode = subProductCode;
            this.PANLength = panLength;
            this.InterchangeRules = interchangeRules;
            this.AuthorisationProcessing = authorisationProcessing;
            this.RangeOfServices = rangeOfServices;

            this.SecurePINLengthOrPVKI = securePINLengthOrPVKI;
            this.VisaCVKA = visaCVKA;
            this.VisaCVKB = visaCVKB;
            this.PVK = pvk;

            this.sequenceNumber = sequenceNumber;
        }

        public CardObject(SqlDataReader reader)
        {
            this.CardId = Convert.ToInt64(reader["card_id"]);
            this.CardReferenceNumber = reader["card_request_reference"].ToString();
            this.sequenceNumber = Convert.ToInt32(reader["card_sequence"]);
            this.IssuerId = Convert.ToInt32(reader["issuer_id"]);
            this.IssuerCode = reader["issuer_code"].ToString();
            this.IssuerName = reader["issuer_name"].ToString();
            this.BranchId = Convert.ToInt32(reader["branch_id"]);

            this.BranchCode = reader["branch_code"].ToString();
            this.BranchName = reader["branch_name"].ToString();
            this.ProductId = Convert.ToInt32(reader["product_id"]);
            this.ProductCode = reader["product_code"].ToString();
            this.BIN = reader["product_bin_code"].ToString();
            this.SubProductCode = reader["sub_product_code"].ToString();
            this.PANLength = int.Parse(reader["pan_length"].ToString());
            this.InterchangeRules = Convert.ToInt32(reader["src1_id"]);
            this.AuthorisationProcessing = Convert.ToInt32(reader["src2_id"]);
            this.RangeOfServices = Convert.ToInt32(reader["src3_id"]);
            this.SecurePINLengthOrPVKI = Convert.ToInt32(reader["PVKI"]);
            this.PVK = reader["PVK"].ToString();
            this.VisaCVKA = reader["CVKA"].ToString();
            this.VisaCVKB = reader["CVKB"].ToString();

            this.CardNumber = reader["card_number"].ToString();

            object obj = reader["card_activation_date"];

            //if (reader["card_activation_date"] != null)
            this.CardActivatedDate = reader["card_activation_date"] as DateTime?;

            // if (reader["card_production_date"] != null)
            this.CardIssuedDate = reader["card_production_date"] as DateTime?;

            //if (reader["card_expiry_date"] != null)
            this.ExpiryDate = reader["card_expiry_date"] as DateTime?;

            this.ExpiryMonths = Convert.ToInt32(reader["expiry_months"]);

            this.DistBatchId = Convert.ToInt64(reader["dist_batch_id"]);
            this.DistCardStatusId = Convert.ToInt32(reader["dist_card_status_id"]);


            this.PVV = reader["pvv"].ToString();
            this.CardIssueMethodId = Convert.ToInt32(reader["card_issue_method_id"]);

            //if (Convert.ToInt32(reader["card_issue_method_id"]) == 0)
            //{
            var account = new AccountDetails();

            account.AccountNumber = reader["customer_account_number"].ToString();
            account.AccountTypeId = Convert.ToInt32(reader["account_type_id"]);
            account.CardIssueMethodId = Convert.ToInt32(reader["card_issue_method_id"]);
            account.CardPriorityId = Convert.ToInt32(reader["card_priority_id"]);
            account.CmsID = reader["cms_id"].ToString();
            account.ContactNumber = reader["contact_number"].ToString();
            account.ContractNumber = reader["contract_number"].ToString();
            account.CurrencyId = Convert.ToInt32(reader["currency_id"]);
            account.CustomerIDNumber = reader["Id_number"].ToString();
            account.CustomerResidencyId = Convert.ToInt32(reader["resident_id"]);
            account.CustomerTitleId = Convert.ToInt32(reader["customer_title_id"]);
            account.CustomerTypeId = Convert.ToInt32(reader["customer_type_id"]);
            account.FirstName = reader["customer_first_name"].ToString();
            account.LastName = reader["customer_last_name"].ToString();
            account.MiddleName = reader["customer_middle_name"].ToString();
            account.NameOnCard = reader["name_on_card"].ToString();
            account.PriorityId = Convert.ToInt32(reader["card_priority_id"]);

            try
            {
                var columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
                account.CreditContractNumber = GetValue(reader, "credit_contract_number", columns);
                account.CreditLimit = Convert.ToDecimal(GetValue(reader, "credit_limit", columns, "0"));
                account.CreditLimit = Convert.ToDecimal(GetValue(reader, "credit_limit_approved", columns, "0"));
                account.EmailAddress = GetValue(reader, "customer_email", columns);
            }
            catch (Exception)
            {

            }
            
            this.CustomerAccount = account;
        }

        private string GetValue(SqlDataReader reader, string fieldName, List<string> columns, string defaultValue = "")
        {
            if (columns.Where(p => p == fieldName).Count() > 0)
            {
                try
                {
                    return reader[fieldName].ToString();
                }
                catch (Exception)
                {
                    return defaultValue;
                }
            }
            else
            {
                return defaultValue;
            }
        }

        public bool isPANGenerated()
        {
            if (String.IsNullOrWhiteSpace(BIN))
                throw new ArgumentNullException("BIN property is empty, set BIN property.");

            return CardNumber.StartsWith(BIN);
        }

    }
}
