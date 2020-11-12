using indigoCardIssuingWeb.CardIssuanceService;
using indigoCardIssuingWeb.service;
using indigoCardIssuingWeb.utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace indigoCardIssuingWeb.webpages.renewal
{
    public class RenewalCommon
    {
        private readonly BatchManagementService _batchService = new BatchManagementService();
        private SystemAdminService sysAdminService = new SystemAdminService();
        private readonly RenewalService _cardRenewalService = new RenewalService();
        private readonly CustomerCardIssueService _customerCardIssuerService = new CustomerCardIssueService();
        private readonly IssuerManagementService _issuerMan = new IssuerManagementService();

        public List<string> errorMessages = new List<string>();

        public RenewalBatchStatusType StatusType { get; set; }

        public bool ConfirmAction(long renewalBatchId, bool isConfirm)
        {
            bool successful = false;

            if (isConfirm)
            {
                if (StatusType == RenewalBatchStatusType.Created)
                {
                    var result = _cardRenewalService.ApproveRenewalBatch(renewalBatchId);
                    successful = true;
                }

                if (StatusType == RenewalBatchStatusType.Exported)
                {
                    _cardRenewalService.ChangeRenewalBatchStatus(renewalBatchId, RenewalBatchStatusType.Received);
                    successful = true;
                }
                if (StatusType == RenewalBatchStatusType.Received)
                {
                    List<string> processingErrors = new List<string>();
                    if (CreateCards(renewalBatchId, out processingErrors))
                    {
                        CreateProductionBatches(renewalBatchId);
                        _cardRenewalService.DistributeRenewalBatch(renewalBatchId);
                        successful = true;
                    }
                    else
                    {
                        successful = false;
                        foreach (var item in processingErrors)
                        {
                            errorMessages.Add(item);
                        }
                    }
                }
            }
            else
            {
                var result = _cardRenewalService.RejectRenewalBatch(renewalBatchId);
                if (result != null && result.RenewalBatchStatus == RenewalBatchStatusType.Rejected)
                {
                    successful = true;
                }
            }

            return successful;
        }
        private bool CreateCards(long renewalBatchId, out List<string> processingErrors)
        {
            bool successful = true;
            processingErrors = new List<string>();
            var details = _cardRenewalService.GetRenewalBatchDetails(renewalBatchId, false);
            List<Tuple<RenewalDetailListModel, CustomerDetails>> newCards = new List<Tuple<RenewalDetailListModel, CustomerDetails>>();
            foreach (var item in details)
            {
                if (item.RenewalStatus == RenewalDetailStatusType.Batched)
                {
                    var packedEntry = CreateCustomerDetails(item);
                    if (packedEntry != null)
                    {
                        newCards.Add(Tuple.Create(item, packedEntry));
                    }
                }
            }
            if (newCards.Count == 0)
            {
                processingErrors.Add("No cards found");
                return false;
            }
            else
            {
                foreach (var item in newCards)
                {
                    long cardId = 0;
                    string resultMessage = string.Empty;
                    successful = successful && _customerCardIssuerService.RequestCardForCustomer(item.Item2, item.Item1.RenewalDetailId, out cardId, out resultMessage);
                    if (cardId != 0)
                    {
                        //approve the card
                        _customerCardIssuerService.MakerChecker(cardId, true, string.Empty, out resultMessage);
                    }
                    else
                    {
                        processingErrors.Add(resultMessage);
                    }
                }
            }
            return successful;
        }

        private CustomerDetails CreateCustomerDetails(RenewalDetailListModel item)
        {
            try
            {
                var product = _batchService.GetProduct(item.ProductId);

                var customer = new CustomerDetails();

                customer.CustomerTitleId = 7;
                string[] names = item.CustomerName.Split(' ');

                customer.FirstName = names[0].Trim();
                customer.MiddleName = names[1].Trim();
                if (names.Length > 2)
                {
                    customer.LastName = names[2].Trim();
                }
                else
                {
                    customer.MiddleName = null;
                    customer.LastName = names[1].Trim();
                }

                customer.CmsID = item.ClientId;
                customer.CustomerId = item.InternalAccountNumber;
                customer.ContractNumber = item.ContractNumber;
                customer.AccountNumber = item.ExternalAccountNumber;
                customer.DomicileBranchId = item.BranchId;
                customer.CardReference = item.CardNumber;
                customer.CardNumber = item.CardNumber;

                customer.DeliveryBranchId = item.DeliveryBranchId;
                customer.CardIssueMethodId = 0;

                customer.PriorityId = 1;

                customer.CardIssueReasonId = 2;
                customer.AccountTypeId = product.AccountTypes.FirstOrDefault();
                customer.CustomerTypeId = 0;
                customer.CurrencyId = item.CurrencyId;
                customer.CustomerResidencyId = 0;

                customer.IssuerId = product.Product.issuer_id;
                customer.BranchId = item.BranchId;
                customer.ProductId = item.ProductId;
                customer.CBSAccountType = item.CBSAccountType;
                customer.CMSAccountType = item.CMSAccountType;
                customer.NameOnCard = item.EmbossingName;
                customer.CustomerIDNumber = item.PassportIDNumber;
                customer.ContactNumber = item.ContractNumber;
                customer.FeeOverridenYN = false;
                customer.FeeWaiverYN = product.Product.renewal_charge_card;
                customer.FeeEditbleYN = false;
                customer.FeeCharge = null;
                customer.Vat = null;
                customer.TotalCharged = null;

                var feeDetails = _batchService.GetFeeDetailByProduct(item.ProductId).FirstOrDefault();
                if (feeDetails != null)
                {
                    customer.FeeDetailId = feeDetails.fee_detail_id;

                    var response = _batchService.GetCurrentFees(feeDetails.fee_detail_id,
                                                               customer.CurrencyId.GetValueOrDefault(),
                                                               customer.CardIssueReasonId.GetValueOrDefault(),
                                                               customer.CBSAccountType);
                    if (response != null)
                    {
                        customer.FeeCharge = response.fee_charge;
                        customer.Vat = response.vat;
                        customer.VatCharged = customer.FeeCharge * (customer.Vat.Value / 100);
                        customer.TotalCharged = customer.FeeCharge + customer.VatCharged;
                    }
                }

                customer.ProductFields = new ProductField[0];

                ProductField[] list = new ProductField[0];
                list = _issuerMan.GetPrintFieldsByProductid(item.ProductId);

                customer.ProductFields = list.ToArray();

                foreach (var field in customer.ProductFields)
                {
                    if (field.MappedName.ToUpper() == StaticFields.IND_SYS_NOC) //System default of name on card
                        field.Value = System.Text.Encoding.UTF8.GetBytes(customer.NameOnCard.ToUpper());
                    else if (field.MappedName.ToUpper() == StaticFields.IND_SYS_PAN) //System default of PAN
                        field.Value = System.Text.Encoding.UTF8.GetBytes(System.Text.RegularExpressions.Regex.Replace(customer.CardNumber.Replace("-", ""), ".{4}", "$0 "));
                    else if (field.MappedName.ToUpper() == StaticFields.ING_NOC) //System default of name on card
                        field.Value = System.Text.Encoding.UTF8.GetBytes(string.Empty);
                    else if (field.MappedName.ToUpper() == "IND_SYS_ADDRESS")
                        field.Value = System.Text.Encoding.UTF8.GetBytes(string.Empty);
                    else if (field.MappedName.ToUpper() == "CASHLIMIT")
                        field.Value = System.Text.Encoding.UTF8.GetBytes(item.LimitBalance.GetValueOrDefault().ToString());
                    else if (field.MappedName.ToUpper() == "IND_SYS_DOB")
                        field.Value = System.Text.Encoding.UTF8.GetBytes(item.BirthDate.GetValueOrDefault().ToString("dd-MM-yyyy"));
                    else if (field.MappedName.ToUpper() == "IND_SYS_POSTCODE")
                        field.Value = System.Text.Encoding.UTF8.GetBytes(string.Empty);
                }

                return customer;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void CreateProductionBatches(long renewalBatchId)
        {
            var details = _cardRenewalService.CreateRenewalDistributionBatches(renewalBatchId);
        }

    }
}