using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Globalization;
using Veneka.Indigo.Common;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.Common.Database;
using Veneka.Indigo.Common.Objects;
using Veneka.Indigo.CardManagement.objects;
using System.IO;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.Common.Utilities;
using Veneka.Indigo.Integration.ProductPrinting;
using Veneka.Indigo.Common.DataAccess;
using Veneka.Indigo.Common.Models.IssuerManagement;

namespace Veneka.Indigo.CardManagement.dal
{
    public class CardManagementDAL : ICardManagementDAL
    {
        private readonly DatabaseConnectionObject _dbObject = DatabaseConnectionObject.Instance;

        #region BRANCH CARD

        /// <summary>
        /// Fetch a list of Branch card codes.
        /// </summary>
        /// <param name="BranchCardCodeType"></param>
        /// <param name="isException"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<branch_card_codes> ListBranchCardCodes(int BranchCardCodeType, bool isException, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_list_branch_card_codes(BranchCardCodeType, isException, auditUserId, auditWorkstation);
                return result.ToList();
            }
        }

        /// <summary>
        /// Used to approve or reject maker checker.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="approved"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public SystemResponseCode MakerChecker(long cardId, Boolean approved, string notes, long auditUserId, string auditWorkstation, out int cardIssueMethodId)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));
            ObjectParameter cardIssueMethod = new ObjectParameter("card_issue_method_id", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_card_MakerChecker(cardId, approved, notes, auditUserId, auditWorkstation, cardIssueMethod, ResultCode);
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());
            cardIssueMethodId = int.Parse(cardIssueMethod.Value.ToString());

            return (SystemResponseCode)resultCode;
        }


        /// <summary>
        /// Used to approve or reject maker checker.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="approved"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public SystemResponseCode RequestMakerChecker(long RequestId, Boolean approved, string notes, long auditUserId, string auditWorkstation, out int cardIssueMethodId)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));
            ObjectParameter cardIssueMethod = new ObjectParameter("card_issue_method_id", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_request_MakerChecker(RequestId, approved, notes, auditUserId, auditWorkstation, cardIssueMethod, ResultCode);
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());
            cardIssueMethodId = int.Parse(cardIssueMethod.Value.ToString());

            return (SystemResponseCode)resultCode;
        }

        /// <summary>
        /// Spoil a branch card.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public SystemResponseCode SpoilBranchCard(long cardId, long auditUserId, string auditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_card_branch_spoil(cardId, auditUserId, auditWorkstation, ResultCode);
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;
        }

        /// <summary>
        /// Card View Page: Gets Branch Card Status for Card
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<CardHistoryStatus> GetCardStatusHistory(long cardId, int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_get_card_history_status((int)cardId, languageId, auditUserId, auditWorkstation);
                return result.ToList();
            }
        }

        ///
        public List<RequestReferenceHistoryResult> GetRequestReferenceHistory(long RequestId, int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_get_request_history_reference((int)RequestId, languageId, auditUserId, auditWorkstation);
                return result.ToList();
            }
        }

        public List<RequestStatusHistoryResult> GetRequestStatusHistory(long RequestId, int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_get_request_history_status((int)RequestId, languageId, auditUserId, auditWorkstation);
                return result.ToList();
            }
        }


        /// <summary>
        /// Card View Page: Gets Batch and Production Reference Numbers for Card
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<CardHistoryReference> GetCardReferenceHistory(long cardId, int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_get_card_history_reference((int)cardId, languageId, auditUserId, auditWorkstation);
                return result.ToList();
            }
        }

        #endregion

        #region EXPOSED METHODS - ISSUE CARD

        public bool ActivateCard(long cardId, long auditUserId, string auditWorkstation)
        {
            //usp_activate_card
            try
            {
                using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
                {
                    var result = context.usp_activate_card(cardId, auditUserId, auditWorkstation);

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region CLASSIC CARD METHODS

        ///// <summary>
        ///// Create a card request for classic card issuing.
        ///// </summary>
        ///// <param name="customerDetails"></param>
        ///// <param name="auditUserId"></param>
        ///// <param name="auditWorkstation"></param>
        ///// <param name="cardId"></param>
        ///// <returns></returns>
        //public SystemResponseCode RequestCardForCustomer(CustomerDetails customerDetails, long auditUserId, string auditWorkstation, out long cardId)
        //{
        //    ObjectParameter CardId = new ObjectParameter("card_id", typeof(long));
        //    ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));
        //    ObjectParameter customer_account_id = new ObjectParameter("new_customer_account_id", typeof(int));
        //    using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
        //    {
        //        context.usp_request_card_for_customer(customerDetails.BranchId, customerDetails.BranchId, customerDetails.ProductId, customerDetails.PriorityId,
        //                                             customerDetails.AccountNumber, customerDetails.DomicileBranchId,
        //                                             customerDetails.AccountTypeId, customerDetails.CardIssueReasonId,
        //                                             customerDetails.FirstName, customerDetails.MiddleName, customerDetails.LastName,
        //                                             customerDetails.NameOnCard,
        //                                             customerDetails.CustomerTitleId,
        //                                             customerDetails.CurrencyId,
        //                                             customerDetails.CustomerResidencyId,
        //                                             customerDetails.CustomerTypeId,
        //                                             customerDetails.CmsID, customerDetails.ContractNumber, customerDetails.CustomerIDNumber, customerDetails.ContactNumber,
        //                                             customerDetails.CustomerId,
        //                                             customerDetails.FeeWaiverYN, customerDetails.FeeEditbleYN,
        //                                             customerDetails.FeeCharge,
        //                                             customerDetails.FeeOverridenYN,
        //                                             auditUserId, auditWorkstation, CardId, ResultCode, customer_account_id);

        //    }

        //    cardId = long.Parse(CardId.Value.ToString());
        //    int resultCode = int.Parse(ResultCode.Value.ToString());
        //    int new_customer_account_id = int.Parse(customer_account_id.Value.ToString());
        //    UpdatePrintFieldValue(customerDetails.ProductFields.ToArray(), new_customer_account_id, auditUserId, auditWorkstation);
        //    return (SystemResponseCode)resultCode;
        //}

        /// <summary>
        /// Create a card request for classic card issuing.
        /// </summary>
        /// <param name="customerDetails"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public SystemResponseCode RequestCardForCustomer(
            int delivery_branch_id, int branch_id, int product_id, int? card_priority_id, string customer_account_number,
            int domicile_branch_id, int? account_type_id, int? card_issue_reason_id, string customer_first_name,
            string customer_middle_name, string customer_last_name, string name_on_card, int customer_title_id,
            int? currency_id, int? resident_id, int? customer_type_id, string cms_id, string contract_number, string idnumber, string contact_number, string email_address,
            string customer_id, bool? fee_waiver_YN, bool? fee_editable_YN, decimal? fee_charged, decimal? vat, decimal? vat_charged, decimal? total_charged,
            bool? fee_overridden_YN, List<IProductPrintField> productFields, int card_issue_method_id, long? renewal_detail_id, string cbs_account_type,
            long auditUserId, string auditWorkstation, out long cardId)
        {
            ObjectParameter CardId = new ObjectParameter("card_id", typeof(long));
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));
            ObjectParameter customer_account_id = new ObjectParameter("new_customer_account_id", typeof(int));
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_request_card_for_customer(delivery_branch_id, branch_id, product_id, card_priority_id, customer_account_number,
                                                        domicile_branch_id, account_type_id, card_issue_reason_id, customer_first_name,
                                                        customer_middle_name, customer_last_name, name_on_card, customer_title_id,
                                                        currency_id, resident_id, customer_type_id, cms_id, contract_number, idnumber, contact_number,
                                                        customer_id, fee_waiver_YN, fee_editable_YN, fee_charged, vat, vat_charged, total_charged, fee_overridden_YN, card_issue_method_id,
                                                        auditUserId, auditWorkstation, email_address, renewal_detail_id, cbs_account_type, CardId, ResultCode, customer_account_id);

            }

            cardId = long.Parse(CardId.Value.ToString());
            int resultCode = int.Parse(ResultCode.Value.ToString());
            int new_customer_account_id = int.Parse(customer_account_id.Value.ToString());
            UpdatePrintFieldValue(productFields.ToArray(), new_customer_account_id, auditUserId, auditWorkstation);
            return (SystemResponseCode)resultCode;
        }



        public SystemResponseCode RequestInstantCardForCustomer(
           int delivery_branch_id, int branch_id, int product_id, int? card_priority_id, string customer_account_number,
           int domicile_branch_id, int? account_type_id, int? card_issue_reason_id, string customer_first_name,
           string customer_middle_name, string customer_last_name, string name_on_card, int customer_title_id,
           int? currency_id, int? resident_id, int? customer_type_id, string cms_id, string contract_number, string idnumber, string contact_number, string email_address,
           string customer_id, bool? fee_waiver_YN, bool? fee_editable_YN, decimal? fee_charged, bool? fee_overridden_YN, List<IProductPrintField> productFields, int card_issue_method_id,
           long auditUserId, string auditWorkstation, out long cardId, out long PrintJobId, out long new_customer_account_id)
        {
            ObjectParameter CardId = new ObjectParameter("card_id", typeof(long));
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));
            ObjectParameter customer_account_id = new ObjectParameter("new_customer_account_id", typeof(int));
            ObjectParameter print_job_id = new ObjectParameter("print_job_id", typeof(long));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_request_instantcard_for_customer(delivery_branch_id, branch_id, product_id, card_priority_id, customer_account_number,
                                                        domicile_branch_id, account_type_id, card_issue_reason_id, customer_first_name,
                                                        customer_middle_name, customer_last_name, name_on_card, customer_title_id,
                                                        currency_id, resident_id, customer_type_id, cms_id, contract_number, idnumber, contact_number, email_address,
                                                        customer_id, fee_waiver_YN, fee_editable_YN, fee_charged, fee_overridden_YN, card_issue_method_id,
                                                        auditUserId, auditWorkstation, CardId, ResultCode, print_job_id, customer_account_id);

            }

            cardId = long.Parse(CardId.Value.ToString());
            int resultCode = int.Parse(ResultCode.Value.ToString());
            new_customer_account_id = long.Parse(customer_account_id.Value.ToString());
            PrintJobId = long.Parse(print_job_id.Value.ToString());

            UpdatePrintFieldValue(productFields.ToArray(), new_customer_account_id, auditUserId, auditWorkstation);
            return (SystemResponseCode)resultCode;
        }
        public SystemResponseCode UpdateCustomerDetails(long cardId, long customerAccountId, CustomerDetails customerDetails, long auditUserId, string auditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_update_customer_details(cardId, customerAccountId, customerDetails.BranchId, customerDetails.DeliveryBranchId, customerDetails.ProductId, customerDetails.PriorityId,
                                                     customerDetails.AccountNumber, customerDetails.DomicileBranchId,
                                                     customerDetails.AccountTypeId, customerDetails.CardIssueReasonId,
                                                     customerDetails.FirstName, customerDetails.MiddleName, customerDetails.LastName,
                                                     customerDetails.NameOnCard,
                                                     customerDetails.CustomerTitleId,
                                                     customerDetails.CurrencyId,
                                                     customerDetails.CustomerResidencyId,
                                                     customerDetails.CustomerTypeId,
                                                     customerDetails.CmsID, customerDetails.ContractNumber, customerDetails.CustomerIDNumber, customerDetails.ContactNumber, customerDetails.EmailAddress,
                                                     auditUserId, auditWorkstation, ResultCode);

            }

            int resultCode = int.Parse(ResultCode.Value.ToString());
            UpdatePrintFieldValue(customerDetails.ProductFields.ToArray(), customerAccountId, auditUserId, auditWorkstation);

            return (SystemResponseCode)resultCode;
        }
        #endregion

        /// <summary>
        /// Get customer details based on card i
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public CustomerDetailsResult GetCustomerDetails(long cardId, int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_get_customer_details(cardId, auditUserId, auditWorkstation);

                return result.First();
            }
        }

        /// <summary>
        /// Returns a list of card priorities.
        /// </summary>
        /// <param name="languageId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<card_priority> GetCardPriorityList(int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var priorityList = context.usp_get_card_priority_list(languageId, auditUserId, auditWorkstation);

                return priorityList.ToList();
            }
        }

        /// <summary>
        /// Search for a list of cards based on the parameteres provided. Null parameters wont be searched on.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userRoleId"></param>
        /// <param name="issuerId"></param>
        /// <param name="branchId"></param>
        /// <param name="cardNumber"></param>
        /// <param name="cardLastFourDigits"></param>
        /// <param name="batchReference"></param>
        /// <param name="loadCardStatus"></param>
        /// <param name="distCardStatus"></param>
        /// <param name="branchCardStatus"></param>
        /// <param name="accountNumber"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="cardIssueMethodId"></param>
        /// <param name="productId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<CardSearchResult> SearchForCards(long userId, int? userRoleId, int? issuerId, int? branchId, string cardNumber,
                                                        string cardLastFourDigits, string cardrefnumber, string batchReference,
                                                        int? loadCardStatusId, int? distCardStatusId, int? branchCardStatusId, long? distBatchId, long? pinBatchId, long? threedBatchId,
                                                        string accountNumber, string firstName, string lastName, string cmsId,
                                                        DateTime? dateFrom, DateTime? dateTo, int? cardIssueMethodId,
                                                        int? productId, int? priorityId, int pageIndex, int rowsPerPage,
                                                        long auditUserId, string auditWorkstation)
        {
            List<CardSearchResult> rtnValue = new List<CardSearchResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {

                ObjectResult<CardSearchResult> results = context.usp_search_cards(userId, userRoleId, issuerId, branchId, cardNumber, cardLastFourDigits, cardrefnumber, batchReference, loadCardStatusId, distCardStatusId,
                                                          branchCardStatusId, distBatchId, pinBatchId, accountNumber, firstName, lastName, cmsId, dateFrom, dateTo, threedBatchId, cardIssueMethodId, productId, priorityId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);

                foreach (CardSearchResult result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }

        /// <summary>
        /// Fetch all cards that are work in progress for the user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>


        public List<CardSearchResult> GetOperatorCardsInProgress(long userId, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            List<CardSearchResult> rtnValue = new List<CardSearchResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<CardSearchResult> results = context.usp_get_operator_cards_inprogress(userId, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);

                return results.ToList();
            }
        }
        public SystemResponseCode RequestHybridCardForCustomer(int delivery_branch_id, int branch_id, int product_id, int? card_priority_id, string customer_account_number, int domicile_branch_id, int? account_type_id, int? card_issue_reason_id, string customer_first_name, string customer_middle_name, string customer_last_name, string name_on_card, int customer_title_id, int? currency_id, int? resident_id, int? customer_type_id, string cms_id, string contract_number, string idnumber, string contact_number, string customer_id, bool? fee_waiver_YN, bool? fee_editable_YN, decimal? fee_charged, bool? fee_overridden_YN, List<IProductPrintField> productFields, int card_issue_method_id, string cbsaccounttype, long auditUserId, string auditWorkstation, out long requestId)
        {
            ObjectParameter Request_id = new ObjectParameter("request_id", typeof(long));
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));
            ObjectParameter customer_account_id = new ObjectParameter("new_customer_account_id", typeof(int));
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_request_hybrid_card_for_customer(delivery_branch_id, branch_id, product_id, card_priority_id, customer_account_number,
                                                        domicile_branch_id, account_type_id, card_issue_reason_id, customer_first_name,
                                                        customer_middle_name, customer_last_name, name_on_card, customer_title_id,
                                                        currency_id, resident_id, customer_type_id, cms_id, contract_number, idnumber, contact_number,
                                                        customer_id, fee_waiver_YN, fee_editable_YN, fee_charged, fee_overridden_YN, card_issue_method_id, cbsaccounttype,
                                                        auditUserId, auditWorkstation, Request_id, ResultCode, customer_account_id);

            }

            requestId = long.Parse(Request_id.Value.ToString());
            int resultCode = int.Parse(ResultCode.Value.ToString());
            int new_customer_account_id = int.Parse(customer_account_id.Value.ToString());
            UpdatePrintFieldValue(productFields.ToArray(), new_customer_account_id, auditUserId, auditWorkstation);
            return (SystemResponseCode)resultCode;
        }


        /// <returns></returns>
        public List<HybridRequestResult> GetOperatorHybridRequestsInProgress(int? statusId, long userId, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            List<HybridRequestResult> rtnValue = new List<HybridRequestResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<HybridRequestResult> results = context.usp_get_operator_hybrid_request_inprogress(statusId, userId, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);

                return results.ToList();
            }
        }
        /// <summary>
        /// Fetch all cards that have an exception status.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<CardSearchResult> GetCardsInError(long userId, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            List<CardSearchResult> rtnValue = new List<CardSearchResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {

                ObjectResult<CardSearchResult> results = context.usp_get_cards_in_error(userId, languageId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);

                foreach (CardSearchResult result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }

        /// <summary>
        /// Fetch all cards waiting to be linked to CMS.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<CardSearchResult> SearchForReissueCards(long userId, int pageIndex, int rowsPerPage, long audit_user_id, string auditWorkstation)
        {
            List<CardSearchResult> rtnValue = new List<CardSearchResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {

                ObjectResult<CardSearchResult> results = context.usp_find_reissue_cards(userId, pageIndex, rowsPerPage, audit_user_id, auditWorkstation);

                foreach (CardSearchResult result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }

        /// <summary>
        /// Checks the Customers' Account Balance after fee charges
        /// </summary>
        /// <param name="CustomerAccount"></param>
        /// <param name="AccountDetails"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public SystemResponseCode CheckCustomerAccountBalance(CustomerDetails customer, long auditUserId, string auditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(bool));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_issue_card_check_account_balance(customer.FeeCharge, customer.AccountBalance, ResultCode);
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;
        }

        /// <summary>
        /// Issues a card to a customer.
        /// </summary>
        /// <param name="customerAccount"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public SystemResponseCode IssueCardToCustomer(CustomerDetails customerAccount, long auditUserId, string auditWorkstation)
        {
            //ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_issue_card_to_customer]";

                    command.Parameters.Add("@card_id", SqlDbType.BigInt).Value = customerAccount.CardId;
                    command.Parameters.Add("@customer_account_number", SqlDbType.VarChar).Value = customerAccount.AccountNumber;
                    command.Parameters.Add("@customer_account_pin", SqlDbType.VarChar).Value = customerAccount.AccountPin;
                    command.Parameters.Add("@domicile_branch_id", SqlDbType.Int).Value = customerAccount.DomicileBranchId;
                    command.Parameters.Add("@account_type_id", SqlDbType.Int).Value = customerAccount.AccountTypeId;


                    command.Parameters.Add("@card_issue_reason_id", SqlDbType.Int).Value = customerAccount.CardIssueReasonId;
                    command.Parameters.Add("@customer_first_name", SqlDbType.VarChar).Value = customerAccount.FirstName;
                    command.Parameters.Add("@customer_middle_name", SqlDbType.VarChar).Value = customerAccount.MiddleName;
                    command.Parameters.Add("@customer_last_name", SqlDbType.VarChar).Value = customerAccount.LastName;
                    command.Parameters.Add("@name_on_card", SqlDbType.VarChar).Value = customerAccount.NameOnCard;
                    command.Parameters.Add("@customer_title_id", SqlDbType.Int).Value = customerAccount.CustomerTitleId;
                    command.Parameters.Add("@cms_id", SqlDbType.VarChar).Value = customerAccount.CmsID;
                    command.Parameters.Add("@customer_id", SqlDbType.VarChar).Value = customerAccount.CustomerId;
                    command.Parameters.Add("@contract_number", SqlDbType.VarChar).Value = customerAccount.ContractNumber;
                    command.Parameters.Add("@contact_number", SqlDbType.VarChar).Value = customerAccount.ContactNumber;
                    command.Parameters.Add("@id_number", SqlDbType.VarChar).Value = customerAccount.CustomerIDNumber;
                    command.Parameters.Add("@customer_email", SqlDbType.VarChar).Value = customerAccount.EmailAddress;
                    command.Parameters.Add("@currency_id", SqlDbType.Int).Value = customerAccount.CurrencyId;
                    command.Parameters.Add("@resident_id", SqlDbType.Int).Value = customerAccount.CustomerResidencyId;
                    command.Parameters.Add("@customer_type_id", SqlDbType.Int).Value = customerAccount.CustomerTypeId;
                    //command.Parameters.Add("@cms_account_type", SqlDbType.VarChar).Value = customerAccount.CMSAccountType;
                    command.Parameters.Add("@cbs_account_type", SqlDbType.VarChar).Value = customerAccount.CBSAccountType;
                    command.Parameters.Add("@fee_detail_id", SqlDbType.Int).Value = customerAccount.FeeDetailId;
                    command.Parameters.Add("@fee_waiver_YN", SqlDbType.Bit).Value = customerAccount.FeeWaiverYN;
                    command.Parameters.Add("@fee_editable_YN", SqlDbType.Bit).Value = customerAccount.FeeEditbleYN;
                    command.Parameters.Add("@fee_charged", SqlDbType.Decimal).Value = customerAccount.FeeCharge;
                    command.Parameters.Add("@fee_overridden_YN", SqlDbType.Bit).Value = customerAccount.FeeOverridenYN;

                    command.Parameters.AddWithValue("@product_fields", UtilityClass.CreateKeyValueTable2<byte[]>(customerAccount.ProductFields.ToDictionary(k => k.ProductPrintFieldId, v => v.Value)));
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    return (SystemResponseCode)int.Parse(command.Parameters["@ResultCode"].Value.ToString());

                }
            }

            //int resultCode = int.Parse(ResultCode.Value.ToString());

            //return (SystemResponseCode)resultCode;
        }

        public SystemResponseCode createPinRequest(PinObject pinDetails, long auditUserId, string auditWorkstation)
        {
            //ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_create_card_pin_request]";

                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = pinDetails.IssuerId;
                    command.Parameters.Add("@branch_id", SqlDbType.Int).Value = pinDetails.BranchId;
                    command.Parameters.Add("@dom_branch_id", SqlDbType.Int).Value = pinDetails.DomBranchId;
                    command.Parameters.Add("@request_reference", SqlDbType.VarChar).Value = pinDetails.PinRequestReference;
                    command.Parameters.Add("@pin_request_status", SqlDbType.NChar).Value = pinDetails.PinRequestStatus;

                    command.Parameters.Add("@product_id", SqlDbType.Int).Value = pinDetails.ProductId;
                    command.Parameters.Add("@product_bin", SqlDbType.VarChar).Value = pinDetails.ProductBin;
                    command.Parameters.Add("@last_four_digits_of_pan", SqlDbType.NChar).Value = pinDetails.LastFourDigitsOfPan;
                    command.Parameters.Add("@expiry_period", SqlDbType.Int).Value = pinDetails.ExpiryPeriod;
                    command.Parameters.Add("@channel", SqlDbType.VarChar).Value = pinDetails.Channel;
                    command.Parameters.Add("@customer_account_number", SqlDbType.VarChar).Value = pinDetails.CustomerAccountNumber;
                    command.Parameters.Add("@customer_email", SqlDbType.VarChar).Value = pinDetails.CustomerEmail;
                    command.Parameters.Add("@customer_contact", SqlDbType.VarChar).Value = pinDetails.CustomerContact;
                    command.Parameters.Add("@pin_request_type", SqlDbType.NChar).Value = pinDetails.PinRequestType;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    return (SystemResponseCode)int.Parse(command.Parameters["@ResultCode"].Value.ToString());

                }
            }

            //int resultCode = int.Parse(ResultCode.Value.ToString());

            //return (SystemResponseCode)resultCode;
        }

        public SystemResponseCode updatePinRequestStatus(PinObject pinDetails, long auditUserId, string auditWorkstation)
        {
            //ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_update_pin_request_status]";

                    command.Parameters.Add("@request_id", SqlDbType.Int).Value = pinDetails.PinRequestId;
                    command.Parameters.Add("@request_status", SqlDbType.VarChar).Value = pinDetails.PinRequestStatus; 
                    command.Parameters.Add("@request_comment", SqlDbType.VarChar).Value = pinDetails.request_comment;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    command.ExecuteNonQuery();

                    return (SystemResponseCode)int.Parse(command.Parameters["@ResultCode"].Value.ToString());

                }
            }
            
        }

        //UpdatePinRequestStatusForReissue
        public SystemResponseCode UpdatePinRequestStatusForReissue(PinObject pinDetails, long auditUserId, string auditWorkstation)
        {
            //ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_update_pin_request_status_for_user_role]";

                    command.Parameters.Add("@request_id", SqlDbType.Int).Value = pinDetails.PinRequestId;
                    command.Parameters.Add("@user_role", SqlDbType.VarChar).Value = pinDetails.reissue_approval_stage;
                    command.Parameters.Add("@request_status", SqlDbType.VarChar).Value = pinDetails.PinRequestStatus;
                    command.Parameters.Add("@request_comment", SqlDbType.VarChar).Value = pinDetails.request_comment;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    command.ExecuteNonQuery();

                    return (SystemResponseCode)int.Parse(command.Parameters["@ResultCode"].Value.ToString());

                }
            }

        }

        public SystemResponseCode updatePinFileStatus(Integration.Common.TerminalCardData PinBlock, long auditUserId, string auditWorkstation)
        {
            //ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_update_pin_file_status]";

                    command.Parameters.Add("@card_id", SqlDbType.VarChar).Value = PinBlock.CardId;
                    command.Parameters.Add("@pin_block", SqlDbType.VarChar).Value = PinBlock.PINBlock;
                    command.Parameters.Add("@approval_status", SqlDbType.VarChar).Value = PinBlock.approval_status;
                    command.Parameters.Add("@approval_comment", SqlDbType.VarChar).Value = PinBlock.approval_comment;
                
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    command.ExecuteNonQuery();

                    return (SystemResponseCode)int.Parse(command.Parameters["@ResultCode"].Value.ToString());

                }
            }

        }

        //updateBatchFileStatus
        public SystemResponseCode updateBatchFileStatus(Integration.Common.TerminalCardData PinBlock, long auditUserId, string auditWorkstation)
        {
            //ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_update_pin_batch_status]";

                    command.Parameters.Add("@pin_file_batch_id", SqlDbType.Int).Value = PinBlock.header_pin_file_batch_id;
                    command.Parameters.Add("@approval_status", SqlDbType.VarChar).Value = PinBlock.approval_status;
                    command.Parameters.Add("@approval_comment", SqlDbType.VarChar).Value = PinBlock.approval_comment;

                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    command.ExecuteNonQuery();

                    return (SystemResponseCode)int.Parse(command.Parameters["@ResultCode"].Value.ToString());

                }
            }

        }

        // updatePinFileStatus(Integration.Common.TerminalCardData PinBlock, long auditUserId, string auditWorkstation)
        public SystemResponseCode deletePinBlock(string product_pan_bin_code, string pan_last_four, string expiry_date, long auditUserId, string auditWorkstation)
        {
            //ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_delete_pin_block]";

                    command.Parameters.Add("@pan_product_bin_code", SqlDbType.Char).Value = product_pan_bin_code;
                    command.Parameters.Add("@pan_last_four", SqlDbType.Char).Value = pan_last_four;
                    command.Parameters.Add("@expiry_date", SqlDbType.VarChar).Value = expiry_date;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    command.ExecuteNonQuery();

                    return (SystemResponseCode)int.Parse(command.Parameters["@ResultCode"].Value.ToString());

                }
            }

            //int resultCode = int.Parse(ResultCode.Value.ToString());

            //return (SystemResponseCode)resultCode;
        }

        public SystemResponseCode CreateRestParams(RestWebservicesHandler restDetails, long auditUserId, string auditWorkstation)
        {
            //ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_insert_restwebservices_parameters]";

                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = restDetails.issuer_id;
                    command.Parameters.Add("@webservice_type", SqlDbType.VarChar).Value = restDetails.webservices_type;
                    command.Parameters.Add("@rest_url", SqlDbType.VarChar).Value = restDetails.rest_url;
                    
                    command.Parameters.Add("@param_one_name", SqlDbType.VarChar).Value = restDetails.param_one_name;
                    command.Parameters.Add("@param_one_value", SqlDbType.VarChar).Value = restDetails.param_one_value;
                    command.Parameters.Add("@param_two_name", SqlDbType.VarChar).Value = restDetails.param_two_name;
                    command.Parameters.Add("@param_two_value", SqlDbType.VarChar).Value = restDetails.param_two_value;
                    command.Parameters.Add("@param_three_name", SqlDbType.VarChar).Value = restDetails.param_three_name;
                    command.Parameters.Add("@param_three_value", SqlDbType.VarChar).Value = restDetails.param_three_value;
                    command.Parameters.Add("@param_four_name", SqlDbType.VarChar).Value = restDetails.param_four_name;
                    command.Parameters.Add("@param_four_value", SqlDbType.VarChar).Value = restDetails.param_four_value;
                    command.Parameters.Add("@param_five_name", SqlDbType.VarChar).Value = restDetails.param_five_name;
                    command.Parameters.Add("@param_five_value", SqlDbType.VarChar).Value = restDetails.param_five_value;
                    command.Parameters.Add("@param_six_name", SqlDbType.VarChar).Value = restDetails.param_six_name;
                    command.Parameters.Add("@param_six_value", SqlDbType.VarChar).Value = restDetails.param_six_value;
                    command.Parameters.Add("@param_seven_name", SqlDbType.VarChar).Value = restDetails.param_seven_name;
                    command.Parameters.Add("@param_seven_value", SqlDbType.VarChar).Value = restDetails.param_seven_value;
                    command.Parameters.Add("@param_eight_name", SqlDbType.VarChar).Value = restDetails.param_eight_name;
                    command.Parameters.Add("@param_eight_value", SqlDbType.VarChar).Value = restDetails.param_eight_value;
                    command.Parameters.Add("@param_nine_name", SqlDbType.VarChar).Value = restDetails.param_nine_name;
                    command.Parameters.Add("@param_nine_value", SqlDbType.VarChar).Value = restDetails.param_nine_value;
                    command.Parameters.Add("@param_ten_name", SqlDbType.VarChar).Value = restDetails.param_ten_name;
                    command.Parameters.Add("@param_ten_value", SqlDbType.VarChar).Value = restDetails.param_ten_value;

                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value =auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;


                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    command.ExecuteNonQuery();

                    return (SystemResponseCode)int.Parse(command.Parameters["@ResultCode"].Value.ToString());

                }
            }

            //int resultCode = int.Parse(ResultCode.Value.ToString());

            //return (SystemResponseCode)resultCode;
        }

        public SystemResponseCode UpdateRestParams(RestWebservicesHandler restDetails, long auditUserId, string auditWorkstation)
        {
            //ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_update_restwebservices_parameters]";

                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = restDetails.issuer_id;
                    command.Parameters.Add("@webservice_type", SqlDbType.VarChar).Value = restDetails.webservices_type;
                    command.Parameters.Add("@rest_url", SqlDbType.VarChar).Value = restDetails.rest_url;

                    command.Parameters.Add("@param_one_name", SqlDbType.VarChar).Value = restDetails.param_one_name;
                    command.Parameters.Add("@param_one_value", SqlDbType.VarChar).Value = restDetails.param_one_value;
                    command.Parameters.Add("@param_two_name", SqlDbType.VarChar).Value = restDetails.param_two_name;
                    command.Parameters.Add("@param_two_value", SqlDbType.VarChar).Value = restDetails.param_two_value;
                    command.Parameters.Add("@param_three_name", SqlDbType.VarChar).Value = restDetails.param_three_name;
                    command.Parameters.Add("@param_three_value", SqlDbType.VarChar).Value = restDetails.param_three_value;
                    command.Parameters.Add("@param_four_name", SqlDbType.VarChar).Value = restDetails.param_four_name;
                    command.Parameters.Add("@param_four_value", SqlDbType.VarChar).Value = restDetails.param_four_value;
                    command.Parameters.Add("@param_five_name", SqlDbType.VarChar).Value = restDetails.param_five_name;
                    command.Parameters.Add("@param_five_value", SqlDbType.VarChar).Value = restDetails.param_five_value;
                    command.Parameters.Add("@param_six_name", SqlDbType.VarChar).Value = restDetails.param_six_name;
                    command.Parameters.Add("@param_six_value", SqlDbType.VarChar).Value = restDetails.param_six_value;
                    command.Parameters.Add("@param_seven_name", SqlDbType.VarChar).Value = restDetails.param_seven_name;
                    command.Parameters.Add("@param_seven_value", SqlDbType.VarChar).Value = restDetails.param_seven_value;
                    command.Parameters.Add("@param_eight_name", SqlDbType.VarChar).Value = restDetails.param_eight_name;
                    command.Parameters.Add("@param_eight_value", SqlDbType.VarChar).Value = restDetails.param_eight_value;
                    command.Parameters.Add("@param_nine_name", SqlDbType.VarChar).Value = restDetails.param_nine_name;
                    command.Parameters.Add("@param_nine_value", SqlDbType.VarChar).Value = restDetails.param_nine_value;
                    command.Parameters.Add("@param_ten_name", SqlDbType.VarChar).Value = restDetails.param_ten_name;
                    command.Parameters.Add("@param_ten_value", SqlDbType.VarChar).Value = restDetails.param_ten_value;

                    command.Parameters.Add("@webservice_params_id", SqlDbType.Int).Value = restDetails.webservice_params_id;

                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;


                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    command.ExecuteNonQuery();

                    return (SystemResponseCode)int.Parse(command.Parameters["@ResultCode"].Value.ToString());

                }
            }

            //int resultCode = int.Parse(ResultCode.Value.ToString());

            //return (SystemResponseCode)resultCode;
        }

        /// <summary>
        /// Set card to printed.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public SystemResponseCode IssueCardPrinted(long cardId, long auditUserId, string auditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_issue_card_printed(cardId, auditUserId, auditWorkstation, ResultCode);
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;
        }

        /// <summary>
        /// Upload PIN to Indigo an mark it as having pin captured.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public SystemResponseCode IssueCardPinCaptured(long cardId, long? pinAuthUserId, long auditUserId, string auditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_issue_card_PIN_captured(cardId, pinAuthUserId, auditUserId, auditWorkstation, ResultCode);
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;
        }

        /// <summary>
        /// Set card to print error.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public SystemResponseCode IssueCardPrintError(long cardId, long auditUserId, string auditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_issue_card_print_error(cardId, auditUserId, auditWorkstation, ResultCode);
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;
        }

        /// <summary>
        /// Spoil an issue card.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public SystemResponseCode IssueCardSpoil(long cardId, int spoilResaonId, string spoilComments, long auditUserId, string auditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_issue_card_spoil(cardId, spoilResaonId, spoilComments, auditUserId, auditWorkstation, ResultCode);
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;
        }

        /// <summary>
        /// Mark issue card as ISSUED. Last status.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public SystemResponseCode IssueCardComplete(long cardId, long auditUserId, string auditWorkstation)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_issue_card_complete(cardId, auditUserId, auditWorkstation, ResultCode);
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;
        }

        /// <summary>
        /// Card Management Linking card to customer failed.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="responseError"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        public void IssueCardLinkFail(long cardId, string responseError, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_issue_card_cms_fail(cardId, responseError, auditUserId, auditWorkstation);
            }
        }

        public void IssueCardLinkRetry(long cardId, string responseError, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_issue_card_cms_retry(cardId, responseError, auditUserId, auditWorkstation);
            }
        }
        /// <summary>
        /// Card Management Linking card to customer Success.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        public void IssueCardLinkSuccess(long cardId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_issue_card_cms_success(cardId, auditUserId, auditWorkstation);
            }
        }


        //public void PINReissue(int issuerId, int branchId, int productId, string pan, long? authoriseUserId,
        //                            bool failed, string notes, long auditUserId, string auditWorkstation)
        //{
        //    using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
        //    {
        //        var results = context.usp_insert_pin_reissue(issuerId, branchId, productId, pan, authoriseUserId, failed, notes,
        //                                                        auditUserId, auditWorkstation);
        //    }
        //}

        public List<PinReissueResult> PINReissueSearch(int? issuerId, int? branchId, int? userRoleId, int? pinReissueStatusId, int? pin_reissue_type_id, long? operatorUserId, bool operatorInProgress, long? authoriseUserId, DateTime? dateFrom, DateTime? dateTo, int languageId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {

                var results = context.usp_search_pin_reissue(issuerId, branchId, userRoleId, pinReissueStatusId, operatorUserId, operatorInProgress, authoriseUserId, dateFrom, dateTo, languageId, pageIndex, rowsPerPage, pin_reissue_type_id, auditUserId, auditWorkstation);
                return results.ToList();
            }
        }


        public PinReissueResult GetPINReissue(long pinReissueId, int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_get_pin_reissue(pinReissueId, languageId, auditUserId, auditWorkstation);
                return result.FirstOrDefault();
            }
        }

        public void UpdateCardPVV(long cardId, string pvv, int languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_card_pvv(cardId, pvv, languageId, auditUserId, auditWorkstation);
            }
        }


        public SystemResponseCode RequestPINReissue(int issuerId, int branchId, int productId, string pan, byte[] index, string mobile_number, int? pin_reissue_type, int languageId, long auditUserId, string auditWorkstation, out PinReissueResult result)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_request_pin_reissue(issuerId, branchId, productId, pan, index, mobile_number, languageId, auditUserId, auditWorkstation, pin_reissue_type, ResultCode);
                result = results.FirstOrDefault();
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;
        }


        public SystemResponseCode ApprovePINReissue(long pinReissueId, string notes, int languageId, long auditUserId, string auditWorkstation, out PinReissueResult result)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_pin_reissue_approve(pinReissueId, notes, languageId, auditUserId, auditWorkstation, ResultCode);
                result = results.FirstOrDefault();
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;
        }


        public SystemResponseCode RejectPINReissue(long pinReissueId, string notes, int languageId, long auditUserId, string auditWorkstation, out PinReissueResult result)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_pin_reissue_reject(pinReissueId, notes, languageId, auditUserId, auditWorkstation, ResultCode);
                result = results.FirstOrDefault();
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;
        }
        public SystemResponseCode CancelPINReissue(long pinReissueId, string notes, int languageId, long auditUserId, string auditWorkstation, out PinReissueResult result)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_pin_reissue_cancel(pinReissueId, notes, languageId, auditUserId, auditWorkstation, ResultCode);
                result = results.FirstOrDefault();
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;
        }
        public SystemResponseCode ExpirePINReissue(long pinReissueId, string notes, int languageId, long auditUserId, string auditWorkstation, out PinReissueResult result)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_pin_reissue_expired(pinReissueId, notes, languageId, auditUserId, auditWorkstation, ResultCode);
                result = results.FirstOrDefault();
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;
        }


        public SystemResponseCode CompletePINReissue(long pinReissueId, string notes, int languageId, long auditUserId, string auditWorkstation, out PinReissueResult result)
        {
            ObjectParameter ResultCode = new ObjectParameter("ResultCode", typeof(int));

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_pin_reissue_complete(pinReissueId, notes, languageId, auditUserId, auditWorkstation, ResultCode);
                result = results.FirstOrDefault();
            }

            int resultCode = int.Parse(ResultCode.Value.ToString());

            return (SystemResponseCode)resultCode;
        }

        ///// <summary>
        ///// If customer detail is linked to a card it will be returned.
        ///// </summary>
        ///// <param name="cardId"></param>
        ///// <param name="auditUserId"></param>
        ///// <param name="auditWorkstation"></param>
        ///// <returns></returns>
        //public List<CustomerAccountResult> GetCustomerAccDetailForCard(long cardId, long auditUserId, string auditWorkstation)
        //{
        //    List<CustomerAccountResult> rtnValue = new List<CustomerAccountResult>();

        //    using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
        //    {
        //        var results = context.usp_get_customer_account(cardId, auditUserId, auditWorkstation);

        //        foreach (var result in results)
        //        {
        //            rtnValue.Add(result);
        //        }
        //    }

        //    return rtnValue;
        //}

        /// <summary>
        /// Get card detail, load batch detail, dist batch detail and customer account detail for a card.
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public CardDetailResult GetCardDetails(long cardId, bool checkMasking, int languageId, long auditUserId, string auditWorkstation)
        {
            List<CardDetailResult> rtnValue = new List<CardDetailResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_card(cardId, checkMasking, languageId, auditUserId, auditWorkstation);

                foreach (var result in results)
                {
                    rtnValue.Add(result);
                }
            }

            if (rtnValue.Count > 0)
                return rtnValue[0];

            return null;
        }


        public RequestDetailResult GetRequestDetails(long cardId, bool checkMasking, int languageId, long auditUserId, string auditWorkstation)
        {
            List<RequestDetailResult> rtnValue = new List<RequestDetailResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_request(cardId, checkMasking, languageId, auditUserId, auditWorkstation);

                foreach (var result in results)
                {
                    rtnValue.Add(result);
                }
            }

            if (rtnValue.Count > 0)
                return rtnValue[0];

            return null;
        }

        /// <summary>
        /// Search cards currently at a branch.
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="cardNumber"> May be null</param>
        /// <param name="branchCardStatus">May be null</param>
        /// <param name="operatorUserId">May be null</param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<CardSearchResult> SearchBranchCards(int? issuer_id, int? branchId, int? user_role_id, int? productId, int? priorityId, int? cardIssueMethodId, string cardNumber, int? branchCardStatusId,
                                                                 long? operatorUserId, int pageIndex, int rowsPerpPage, int? languageId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_search_branch_cards(issuer_id, branchId, user_role_id, productId, priorityId, cardIssueMethodId, cardNumber, branchCardStatusId, operatorUserId,
                                                              pageIndex, rowsPerpPage, languageId, auditUserId, auditWorkstation);

                return results.ToList();
            }
        }

        /// <summary>
        /// Persist checked in and out cards for an operator to the DB.
        /// </summary>
        /// <param name="operatorUserId"></param>
        /// <param name="checkedOutCards"></param>
        /// <param name="checkedInCards"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        public List<SearchBranchCardsResult> CheckInOutCards(long operatorUserId, int branchId, int productId, List<long> checkedOutCards, List<long> checkedInCards, long auditUserId, string auditWorkstation)
        {
            List<SearchBranchCardsResult> rtnList = new List<SearchBranchCardsResult>();

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    DataTable dt_CardList = new DataTable();
                    dt_CardList.Columns.Add("card_id", typeof(long));
                    dt_CardList.Columns.Add("branch_card_statuses_id", typeof(int));
                    DataRow workRow;

                    foreach (var item in checkedOutCards)
                    {
                        workRow = dt_CardList.NewRow();
                        workRow["card_id"] = item;
                        workRow["branch_card_statuses_id"] = BranchCardStatus.AVAILABLE_FOR_ISSUE;
                        dt_CardList.Rows.Add(workRow);
                    }

                    foreach (var item in checkedInCards)
                    {
                        workRow = dt_CardList.NewRow();
                        workRow["card_id"] = item;
                        workRow["branch_card_statuses_id"] = BranchCardStatus.CHECKED_IN;
                        dt_CardList.Rows.Add(workRow);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_cards_checkInOut]";

                    command.Parameters.Add("@operator_user_id", SqlDbType.BigInt).Value = operatorUserId;
                    command.Parameters.Add("@branch_id", SqlDbType.Int).Value = branchId;
                    command.Parameters.Add("@product_id", SqlDbType.Int).Value = productId;
                    command.Parameters.AddWithValue("@card_id_array", dt_CardList);
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                SearchBranchCardsResult outCard = new SearchBranchCardsResult();

                                outCard.card_id = long.Parse(dataReader["card_id"].ToString());
                                outCard.branch_card_statuses_id = int.Parse(dataReader["branch_card_statuses_id"].ToString());
                                outCard.current_card_status = dataReader["branch_card_statuses_name"].ToString();
                                outCard.card_number = dataReader["card_number"].ToString();
                                outCard.card_request_reference = dataReader["card_reference_number"].ToString();
                                //outCard.operator_user_id = dataReader["operator_user_id"] != null ? long.Parse(dataReader["operator_user_id"].ToString()) : (long?)null;

                                rtnList.Add(outCard);
                            }
                        }
                    }
                }
            }

            return rtnList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="issuerID"></param>
        /// <param name="firstname"></param>
        /// <param name="lastname"></param>
        /// <param name="customerAccount"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<IssueCard> GetIssueCards(string user, int issuerID, string firstname, string lastname,
                                               string customerAccount, DateTime dateFrom, DateTime dateTo,
                                               DistCardStatus status)
        {
            return null;
            //if (dateFrom == Convert.ToDateTime("0001/01/01 12:00:00 AM"))
            //    dateFrom = DateTime.Today.AddYears(-1);

            //if (dateTo == Convert.ToDateTime("0001/01/01 12:00:00 AM"))
            //    dateTo = DateTime.Today;

            //if (status == DistCardStatus.AVAILABLE_FOR_ISSUE || status == DistCardStatus.RECEIVED_AT_BRANCH)
            //{
            //    dateFrom = DateTime.Today.AddYears(-1);
            //    dateTo = DateTime.Today;
            //}

            //try
            //{
            //    using (SqlConnection con = _dbObject.SQLConnection)
            //    {
            //        using (SqlCommand command = con.CreateCommand())
            //        {


            //            dateTo = dateTo.AddDays(1);

            //            //check to see if  is parseable and can be a branch code
            //            short branchCode = 0;
            //            bool succeeded = Int16.TryParse(user, out branchCode);
            //            string sql = "";

            //            if (String.IsNullOrEmpty(user))
            //            {
            //                sql = @"SELECT card_id, dist_cards.card_number, dist_cards.seqeunce,dist_cards.dist_batch_reference,dist_cards.card_status, dist_cards.date_issued, dist_cards.issued_by, dist_cards.customer_first_name,dist_cards.customer_last_name, dist_cards.customer_account,dist_cards.account_type, dist_cards.name_on_card,dist_cards.issuer_id, dist_cards.branch_code,dist_cards.reason_for_issue,dist_cards.customer_title, dist_cards.assigned_operator " +
            //                                        "FROM dist_cards inner join distribution_batch ON dist_cards.dist_batch_reference = distribution_batch.batch_refrence " +
            //                                        "WHERE dist_cards.card_status = '" + status + "' AND distribution_batch.issuer_id = " + issuerID + " ";

            //            }
            //            else
            //            {
            //                if (succeeded)
            //                {
            //                    sql = @"SELECT card_id, dist_cards.card_number, dist_cards.seqeunce,dist_cards.dist_batch_reference,dist_cards.card_status, dist_cards.date_issued, dist_cards.issued_by, dist_cards.customer_first_name,dist_cards.customer_last_name, dist_cards.customer_account,dist_cards.account_type, dist_cards.name_on_card,dist_cards.issuer_id, dist_cards.branch_code,dist_cards.reason_for_issue,dist_cards.customer_title, dist_cards.assigned_operator " +
            //                            "FROM dist_cards inner join distribution_batch ON dist_cards.dist_batch_reference = distribution_batch.batch_refrence " +
            //                            "WHERE dist_cards.card_status = '" + status + "' AND distribution_batch.issuer_id = " + issuerID + " AND dist_cards.branch_code = '" + user + "'";
            //                    //AND dist_cards.date_issued >='" + dateFrom + "' AND dist_cards.date_issued <= '" + dateTo + "'

            //                }
            //                else
            //                {
            //                    sql = @"SELECT card_id, dist_cards.card_number, dist_cards.seqeunce,dist_cards.dist_batch_reference,dist_cards.card_status, dist_cards.date_issued, dist_cards.issued_by, dist_cards.customer_first_name,dist_cards.customer_last_name, dist_cards.customer_account,dist_cards.account_type, dist_cards.name_on_card,dist_cards.issuer_id, dist_cards.branch_code,dist_cards.reason_for_issue,dist_cards.customer_title, dist_cards.assigned_operator " +
            //                            "FROM dist_cards inner join distribution_batch ON dist_cards.dist_batch_reference = distribution_batch.batch_refrence " +
            //                            "WHERE dist_cards.card_status = '" + status + "' AND distribution_batch.issuer_id = " + issuerID + " AND issued_by = '" + user + "'";
            //                    //AND dist_cards.date_issued >='" + dateFrom + "' AND dist_cards.date_issued <= '" + dateTo + "'
            //                }
            //            }

            //            if(status != IssueCardStatus.AVAILABLE_FOR_ISSUE && status != IssueCardStatus.RECEIVED_AT_BRANCH)
            //            {
            //                sql += " AND dist_cards.date_issued >='" + dateFrom + "' AND dist_cards.date_issued <= '" + dateTo + "'";
            //            }

            //            command.CommandText =sql;

            //            using (SqlDataReader dataReader = command.ExecuteReader())
            //            {
            //                if (dataReader.HasRows)
            //                {
            //                    return CreateIssueCardList(dataReader);
            //                }
            //                else
            //                {
            //                    return null;
            //                }
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        /// <summary>
        /// getting products list from db
        /// </summary>
        /// <param name="issuerID"></param>
        /// <param name="pageIndex"></param>
        /// <param name="RowsPerpage"></param>
        /// <returns></returns>
        public List<ProductlistResult> GetProductsList(int issuerID, int? cardIssueMethodId, bool? deletedYN, int pageIndex, int RowsPerpage)
        {
            var context = new IssuerManagementDataAccess();
            List<ProductlistResult> rtnValue = context.usp_get_productlist(issuerID, cardIssueMethodId, deletedYN, pageIndex, RowsPerpage);
            return rtnValue;
        }

        /// <summary>
        /// getting Font list to fill dropdown
        /// </summary>
        /// <returns></returns>
        public List<Issuer_product_font> GetFontFamilyList()
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_getFontsList();

                return results.ToList();
            }
        }

        /// <summary>
        /// getting service request code1
        /// </summary>
        /// <returns></returns>
        public List<ServiceRequestCode> GetServiceRequestCode1()
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_product_service_request_code1();
                return results.ToList();
            }
        }

        /// <summary>
        /// getting service request code2
        /// </summary>
        /// <returns></returns>
        public List<ServiceRequestCode1> GetServiceRequestCode2()
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_product_service_request_code2();

                return results.ToList();
            }
        }

        /// <summary>
        /// getting service request code3
        /// </summary>
        /// <returns></returns>
        public List<ServiceRequestCode2> GetServiceRequestCode3()
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_product_service_request_code3();
                return results.ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productid"></param>
        /// <returns></returns>
        public List<product_currency1> GetCurreniesbyProduct(int productid)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_currenies_product(productid);

                return results.ToList();
            }
        }

        public List<ProductCurrencyResult> GetProductCurrencies(int productid, int? currencyId, bool? active)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_product_currency(productid, currencyId, active, 0, 0, "");

                return results.ToList();
            }
        }
        public List<ProductExternalSystemResult> GetProductExternalFields(int productid, int? currencyId, bool? active)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var results = context.usp_get_product_external_system_fields(null, productid, 0, 0, "");

                return results.ToList();
            }
        }
        public List<product_interface> GetProductInterfaces(int productId, int? interfaceTypeId, int? interfaceAreaId, long auditUserId, string auditWorkstation)
        {
            var result = new IssuerManagementDataAccess().usp_get_product_interfaces(productId, interfaceTypeId, interfaceAreaId, auditUserId, auditWorkstation);
            return result;
        }

        public List<ProductAccountTypesResult> GetProductAccountTypes(int productId, int languageId, long auditUserId, string auditWorkstation)
        {
            var result = new IssuerManagementDataAccess().usp_get_product_account_types(productId, languageId, auditUserId, auditWorkstation);
            return result;
        }

        public List<ProductIssueReasonsResult> GetProductIssueReasons(int productId, int languageId, long auditUserId, string auditWorkstation)
        {
            var result = new IssuerManagementDataAccess().usp_get_product_issue_reasons(productId, languageId, auditUserId, auditWorkstation);
            return result;
        }

        public List<CurrencyResult> GetCurrencyList(int languageId, long auditUserId, string auditWorkstation)
        {
            var result = new IssuerManagementDataAccess().usp_get_currency_list(languageId, auditUserId, auditWorkstation);
            return result;
        }

        public List<DistBatchFlows> GetDistBatchFlowList(int card_issue_method_id, int languageId, long auditUserId, string auditWorkstation)
        {
            var result = new IssuerManagementDataAccess().usp_get_dist_batch_flows(card_issue_method_id, auditUserId, auditWorkstation);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Productid"></param>
        /// <returns></returns>
        public ProductResult GetProduct(int productId, long auditUserId, string auditWorkstation)
        {
            ProductResult rtnValue = new ProductResult();

            ProductlistResult result = new IssuerManagementDataAccess().usp_getProductbyProductid(productId);

            if (result != null)
            {
                rtnValue.Product = result;

                //Get issue Reasons
                rtnValue.CardIssueReasons = this.GetProductIssueReasons(productId, 0, auditUserId, auditWorkstation).Select(s => s.card_issue_reason_id).ToList();

                //Get account types
                rtnValue.AccountTypes = this.GetProductAccountTypes(productId, 0, auditUserId, auditWorkstation).Select(s => s.account_type_id).ToList();

                //Currency for card                    
                rtnValue.Currency = this.GetProductCurrencies(productId, null, true); //this.GetCurreniesbyProduct(productId).Select(s => s.currency_id).ToList();

                //external_system_fields
                rtnValue.ExternalSystemFields = this.GetProductExternalFields(productId, null, true); //this.GetCurreniesbyProduct(productId).Select(s => s.currency_id).ToList();

                //Interfaces
                rtnValue.Interfaces = this.GetProductInterfaces(productId, null, null, auditUserId, auditWorkstation);

                rtnValue.AccountType_Mappings = this.GetProductAccountTypeMappings(productId, null, auditUserId, auditWorkstation);
            }

            return rtnValue;
        }

        /// <summary>
        /// Inserting Product into issuer_product table
        /// </summary>
        /// <param name="productlistresult"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="dbResponse"></param>
        /// <returns></returns>
        /// 
        public SystemResponseCode InsertProduct(ProductResult productResult, long auditUserId, string auditWorkstation, out long productId)
        {
            int resultCode;

            List<Tuple<long, long, string>> prodInterfaceParams = new List<Tuple<long, long, string>>();
            foreach (var item in productResult.Interfaces.Where(w => w.interface_area == 0))
            {
                prodInterfaceParams.Add(new Tuple<long, long, string>(item.interface_type_id, item.connection_parameter_id, item.interface_guid));
            }

            List<Tuple<long, long, string>> issueInterfaceParams = new List<Tuple<long, long, string>>();
            foreach (var item in productResult.Interfaces.Where(w => w.interface_area == 1))
            {
                issueInterfaceParams.Add(new Tuple<long, long, string>(item.interface_type_id, item.connection_parameter_id, item.interface_guid));
            }

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    DataTable dt_currency = new DataTable();
                    dt_currency.Columns.Add("currency_id", typeof(int));
                    dt_currency.Columns.Add("is_base", typeof(bool));
                    dt_currency.Columns.Add("usr_field_name_1", typeof(string));
                    dt_currency.Columns.Add("usr_field_val_1", typeof(string));
                    dt_currency.Columns.Add("usr_field_name_2", typeof(string));
                    dt_currency.Columns.Add("usr_field_val_2", typeof(string));
                    DataRow workRow;

                    foreach (var item in productResult.Currency)
                    {
                        workRow = dt_currency.NewRow();
                        workRow["currency_id"] = item.currency_id;
                        workRow["is_base"] = item.is_base;
                        workRow["usr_field_name_1"] = item.usr_field_name_1;
                        workRow["usr_field_val_1"] = item.usr_field_val_1;
                        workRow["usr_field_name_2"] = item.usr_field_name_2;
                        workRow["usr_field_val_2"] = item.usr_field_val_2;
                        dt_currency.Rows.Add(workRow);
                    }


                    DataTable dt_externalsystemfields = new DataTable();
                    dt_externalsystemfields.Columns.Add("external_system_field_id", typeof(int));
                    dt_externalsystemfields.Columns.Add("field_name", typeof(string));
                    dt_externalsystemfields.Columns.Add("field_value", typeof(string));
                    DataRow Row;

                    foreach (var item in productResult.ExternalSystemFields)
                    {
                        Row = dt_externalsystemfields.NewRow();
                        Row["external_system_field_id"] = item.external_system_field_id;
                        Row["field_name"] = item.field_name;
                        Row["field_value"] = item.field_value;
                        dt_externalsystemfields.Rows.Add(Row);
                    }

                    DataTable dt_accounttpes = new DataTable();
                    dt_accounttpes.Columns.Add("cbs_account_type", typeof(string));
                    dt_accounttpes.Columns.Add("indigo_account_type", typeof(string));
                    dt_accounttpes.Columns.Add("cms_account_type", typeof(string));

                    DataRow dr;

                    foreach (var item in productResult.AccountType_Mappings)
                    {
                        dr = dt_accounttpes.NewRow();
                        dr["cbs_account_type"] = item.CbsAccountType;
                        dr["cms_account_type"] = item.CmsAccountType;
                        dr["indigo_account_type"] = item.IndigoAccountTypeId;

                        dt_accounttpes.Rows.Add(dr);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_insert_product]";

                    command.Parameters.Add("@product_name", SqlDbType.VarChar).Value = productResult.Product.product_name;
                    command.Parameters.Add("@product_code", SqlDbType.VarChar).Value = productResult.Product.product_code;
                    command.Parameters.Add("@product_bin_code", SqlDbType.VarChar).Value = productResult.Product.product_bin_code;
                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = productResult.Product.issuer_id;
                    command.Parameters.Add("@pan_length", SqlDbType.SmallInt).Value = productResult.Product.pan_length;
                    command.Parameters.Add("@sub_product_code", SqlDbType.VarChar).Value = productResult.Product.sub_product_code;

                    command.Parameters.Add("@name_on_card_top", SqlDbType.Decimal).Value = productResult.Product.name_on_card_top;
                    command.Parameters.Add("@name_on_card_left", SqlDbType.Decimal).Value = productResult.Product.name_on_card_left;
                    command.Parameters.Add("@Name_on_card_font_size", SqlDbType.Int).Value = productResult.Product.Name_on_card_font_size;
                    command.Parameters.Add("@font_id", SqlDbType.Int).Value = productResult.Product.font_id;
                    command.Parameters.Add("@card_issue_method_id", SqlDbType.Int).Value = productResult.Product.card_issue_method_id;

                    command.Parameters.Add("@src1_id", SqlDbType.Int).Value = productResult.Product.src1_id;
                    command.Parameters.Add("@src2_id", SqlDbType.Int).Value = productResult.Product.src2_id;
                    command.Parameters.Add("@src3_id", SqlDbType.Int).Value = productResult.Product.src3_id;
                    command.Parameters.Add("@PVKI", SqlDbType.VarChar).Value = productResult.Product.PVKI;
                    command.Parameters.Add("@PVK", SqlDbType.VarChar).Value = productResult.Product.PVK;
                    command.Parameters.Add("@CVKA", SqlDbType.VarChar).Value = productResult.Product.CVKA;
                    command.Parameters.Add("@CVKB", SqlDbType.VarChar).Value = productResult.Product.CVKB;
                    command.Parameters.Add("@expiry_months", SqlDbType.Int).Value = productResult.Product.expiry_months;

                    command.Parameters.Add("@pin_mailer_printing_YN", SqlDbType.Bit).Value = productResult.Product.pin_mailer_printing_YN;
                    command.Parameters.Add("@pin_mailer_reprint_YN", SqlDbType.Bit).Value = productResult.Product.pin_mailer_reprint_YN;
                    command.Parameters.Add("@pin_calc_method_id", SqlDbType.Int).Value = productResult.Product.pin_calc_method_id;
                    command.Parameters.Add("@fee_scheme_id", SqlDbType.Int).Value = productResult.Product.fee_scheme_id;
                    command.Parameters.Add("@enable_instant_pin_YN", SqlDbType.Bit).Value = productResult.Product.enable_instant_pin_YN;

                    command.Parameters.Add("@enable_instant_pin_reissue_YN", SqlDbType.Bit).Value = productResult.Product.enable_instant_pin_reissue_YN;
                    command.Parameters.Add("@min_pin_length", SqlDbType.Int).Value = productResult.Product.min_pin_length;
                    command.Parameters.Add("@max_pin_length", SqlDbType.Int).Value = productResult.Product.max_pin_length;
                    command.Parameters.Add("@cms_exportable_YN", SqlDbType.Bit).Value = productResult.Product.cms_exportable_YN;
                    command.Parameters.Add("@product_load_type_id", SqlDbType.Int).Value = productResult.Product.product_load_type_id;
                    command.Parameters.Add("@print_issue_card_YN", SqlDbType.Bit).Value = productResult.Product.print_issue_card_YN;
                    command.Parameters.Add("@e_pin_request_YN", SqlDbType.Bit).Value = productResult.Product.e_pin_request_YN ?? false;

                    command.Parameters.Add("@auto_approve_batch_YN", SqlDbType.Bit).Value = productResult.Product.auto_approve_batch_YN;
                    command.Parameters.Add("@account_validation_YN", SqlDbType.Bit).Value = productResult.Product.account_validation_YN;

                    command.Parameters.AddWithValue("@card_issue_reasons_list", UtilityClass.CreateKeyValueTable(productResult.CardIssueReasons));
                    command.Parameters.AddWithValue("@account_types_list", UtilityClass.CreateKeyValueTable(productResult.AccountTypes));

                    command.Parameters.AddWithValue("@prod_interface_parameters_list", UtilityClass.CreateBiKeyValueTable(prodInterfaceParams));
                    command.Parameters.AddWithValue("@issue_interface_parameters_list", UtilityClass.CreateBiKeyValueTable(issueInterfaceParams));
                    command.Parameters.AddWithValue("@accounttype_interface_parameters_list", dt_accounttpes);

                    command.Parameters.Add("@decimalisation_table", SqlDbType.VarChar).Value = productResult.Product.decimalisation_table;
                    command.Parameters.Add("@pin_validation_data", SqlDbType.VarChar).Value = productResult.Product.pin_validation_data;
                    command.Parameters.Add("@pin_block_formatid", SqlDbType.VarChar).Value = productResult.Product.pin_block_formatid;

                    command.Parameters.Add("@production_dist_batch_status_flow_id", SqlDbType.Int).Value = productResult.Product.production_dist_batch_status_flow;
                    command.Parameters.Add("@distribution_dist_batch_status_flow_id", SqlDbType.Int).Value = productResult.Product.distribution_dist_batch_status_flow;
                    command.Parameters.Add("@charge_fee_to_issuing_branch_YN", SqlDbType.Bit).Value = productResult.Product.charge_fee_to_issuing_branch_YN;
                    command.Parameters.Add("@charge_fee_at_cardrequest", SqlDbType.Bit).Value = productResult.Product.charge_fee_at_cardrequest;
                   
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;
                    command.Parameters.AddWithValue("@currencylist", dt_currency);
                    command.Parameters.AddWithValue("@external_system_fields", dt_externalsystemfields);

                    command.Parameters.Add("@allow_manual_uploaded_YN", SqlDbType.Bit).Value = productResult.Product.allow_manual_uploaded_YN;
                    command.Parameters.Add("@allow_reupload_YN", SqlDbType.Bit).Value = productResult.Product.allow_reupload_YN;
                    command.Parameters.Add("@remote_cms_update_YN", SqlDbType.Bit).Value = productResult.Product.remote_cms_update_YN;
                    command.Parameters.Add("@pin_account_validation_YN", SqlDbType.Bit).Value = productResult.Product.pin_account_validation_YN;

                    command.Parameters.Add("@renewal_activate_card", SqlDbType.Bit).Value = productResult.Product.renewal_activate_card;
                    command.Parameters.Add("@renewal_charge_card", SqlDbType.Bit).Value = productResult.Product.renewal_charge_card;

                    command.Parameters.Add("@credit_limit_approve", SqlDbType.Bit).Value = productResult.Product.credit_limit_approve;
                    command.Parameters.Add("@credit_limit_capture", SqlDbType.Bit).Value = productResult.Product.credit_limit_capture;
                    command.Parameters.Add("@credit_limit_update", SqlDbType.Bit).Value = productResult.Product.credit_limit_update;

                    command.Parameters.Add("@email_required", SqlDbType.Bit).Value = productResult.Product.email_required;
                    command.Parameters.Add("@generate_contract_number", SqlDbType.Bit).Value = productResult.Product.generate_contract_number;
                    command.Parameters.Add("@manual_contract_number", SqlDbType.Bit).Value = productResult.Product.manual_contract_number;
                    command.Parameters.Add("@parallel_approval", SqlDbType.Bit).Value = productResult.Product.parallel_approval;
                    command.Parameters.Add("@activation_by_center_operator", SqlDbType.Bit).Value = productResult.Product.activation_by_center_operator;

                    command.Parameters.Add("@credit_contract_prefix", SqlDbType.VarChar).Value = productResult.Product.credit_contract_prefix;
                    command.Parameters.Add("@credit_contract_suffix_format", SqlDbType.VarChar).Value = productResult.Product.credit_contract_suffix_format;
                    command.Parameters.Add("@credit_contract_last_sequence", SqlDbType.BigInt).Value = productResult.Product.credit_contract_last_sequence;

                    command.Parameters.Add("@production_dist_batch_status_flow_renewal", SqlDbType.Int).Value = productResult.Product.production_dist_batch_status_flow_renewal;
                    command.Parameters.Add("@distribution_dist_batch_status_flow_renewal", SqlDbType.Int).Value = productResult.Product.distribution_dist_batch_status_flow_renewal;
                    command.Parameters.Add("@is_m_twenty_printed", SqlDbType.Bit).Value = productResult.Product.Is_mtwenty_printed;

                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                    command.Parameters.Add("@new_product_id", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    productId = int.Parse(command.Parameters["@new_product_id"].Value.ToString());
                    resultCode = int.Parse(command.Parameters["@ResultCode"].Value.ToString());
                }
            }

            return (SystemResponseCode)resultCode;
        }

        /// <summary>
        /// Inserting Product into issuer_product table
        /// </summary>
        /// <param name="productlistresult"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="dbResponse"></param>
        /// <returns></returns>
        /// 
        public SystemResponseCode UpdateProduct(ProductResult productResult, long auditUserId, string auditWorkstation)
        {
            int resultCode;

            List<Tuple<long, long, string>> prodInterfaceParams = new List<Tuple<long, long, string>>();
            foreach (var item in productResult.Interfaces.Where(w => w.interface_area == 0))
            {
                prodInterfaceParams.Add(new Tuple<long, long, string>(item.interface_type_id, item.connection_parameter_id, item.interface_guid));
            }

            List<Tuple<long, long, string>> issueInterfaceParams = new List<Tuple<long, long, string>>();
            foreach (var item in productResult.Interfaces.Where(w => w.interface_area == 1))
            {
                issueInterfaceParams.Add(new Tuple<long, long, string>(item.interface_type_id, item.connection_parameter_id, item.interface_guid));
            }

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    DataTable dt_currency = new DataTable();
                    dt_currency.Columns.Add("currency_id", typeof(int));
                    dt_currency.Columns.Add("is_base", typeof(bool));
                    dt_currency.Columns.Add("usr_field_name_1", typeof(string));
                    dt_currency.Columns.Add("usr_field_val_1", typeof(string));
                    dt_currency.Columns.Add("usr_field_name_2", typeof(string));
                    dt_currency.Columns.Add("usr_field_val_2", typeof(string));
                    DataRow workRow;

                    foreach (var item in productResult.Currency)
                    {
                        workRow = dt_currency.NewRow();
                        workRow["currency_id"] = item.currency_id;
                        workRow["is_base"] = item.is_base;
                        workRow["usr_field_name_1"] = item.usr_field_name_1;
                        workRow["usr_field_val_1"] = item.usr_field_val_1;
                        workRow["usr_field_name_2"] = item.usr_field_name_2;
                        workRow["usr_field_val_2"] = item.usr_field_val_2;
                        dt_currency.Rows.Add(workRow);
                    }
                    DataTable dt_externalsystemfields = new DataTable();
                    dt_externalsystemfields.Columns.Add("external_system_field_id", typeof(int));
                    dt_externalsystemfields.Columns.Add("field_name", typeof(string));
                    dt_externalsystemfields.Columns.Add("field_value", typeof(string));
                    DataRow Row;

                    foreach (var item in productResult.ExternalSystemFields)
                    {
                        Row = dt_externalsystemfields.NewRow();
                        Row["external_system_field_id"] = item.external_system_field_id;
                        Row["field_name"] = item.field_name;
                        Row["field_value"] = item.field_value;
                        dt_externalsystemfields.Rows.Add(Row);
                    }

                    DataTable dt_accounttpes = new DataTable();
                    dt_accounttpes.Columns.Add("cbs_account_type", typeof(string));
                    dt_accounttpes.Columns.Add("indigo_account_type", typeof(string));
                    dt_accounttpes.Columns.Add("cms_account_type", typeof(string));

                    DataRow dr;

                    foreach (var item in productResult.AccountType_Mappings)
                    {
                        dr = dt_accounttpes.NewRow();
                        dr["cbs_account_type"] = item.CbsAccountType;
                        dr["cms_account_type"] = item.CmsAccountType;
                        dr["indigo_account_type"] = item.IndigoAccountTypeId;

                        dt_accounttpes.Rows.Add(dr);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_update_product]";
                    command.Parameters.Add("@product_id", SqlDbType.VarChar).Value = productResult.Product.product_id;
                    command.Parameters.Add("@product_name", SqlDbType.VarChar).Value = productResult.Product.product_name;
                    command.Parameters.Add("@product_code", SqlDbType.VarChar).Value = productResult.Product.product_code;
                    command.Parameters.Add("@product_bin_code", SqlDbType.VarChar).Value = productResult.Product.product_bin_code;
                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = productResult.Product.issuer_id;
                    command.Parameters.Add("@pan_length", SqlDbType.SmallInt).Value = productResult.Product.pan_length;
                    command.Parameters.Add("@sub_product_code", SqlDbType.VarChar).Value = productResult.Product.sub_product_code;
                    command.Parameters.Add("@expiry_months", SqlDbType.Int).Value = productResult.Product.expiry_months;
                    command.Parameters.Add("@fee_scheme_id", SqlDbType.Int).Value = productResult.Product.fee_scheme_id;

                    command.Parameters.Add("@name_on_card_top", SqlDbType.Decimal).Value = productResult.Product.name_on_card_top;
                    command.Parameters.Add("@name_on_card_left", SqlDbType.Decimal).Value = productResult.Product.name_on_card_left;
                    command.Parameters.Add("@Name_on_card_font_size", SqlDbType.Int).Value = productResult.Product.Name_on_card_font_size;
                    command.Parameters.Add("@font_id", SqlDbType.Int).Value = productResult.Product.font_id;
                    command.Parameters.Add("@card_issue_method_id", SqlDbType.Int).Value = productResult.Product.card_issue_method_id;
                    command.Parameters.Add("@print_issue_card_YN", SqlDbType.Bit).Value = productResult.Product.print_issue_card_YN;

                    command.Parameters.Add("@src1_id", SqlDbType.Int).Value = productResult.Product.src1_id;
                    command.Parameters.Add("@src2_id", SqlDbType.Int).Value = productResult.Product.src2_id;
                    command.Parameters.Add("@src3_id", SqlDbType.Int).Value = productResult.Product.src3_id;
                    command.Parameters.Add("@PVKI", SqlDbType.VarChar).Value = productResult.Product.PVKI;
                    command.Parameters.Add("@PVK", SqlDbType.VarChar).Value = productResult.Product.PVK;
                    command.Parameters.Add("@CVKA", SqlDbType.VarChar).Value = productResult.Product.CVKA;
                    command.Parameters.Add("@CVKB", SqlDbType.VarChar).Value = productResult.Product.CVKB;

                    command.Parameters.Add("@enable_instant_pin_YN", SqlDbType.Bit).Value = productResult.Product.enable_instant_pin_YN;
                    command.Parameters.Add("@enable_instant_pin_reissue_YN", SqlDbType.Bit).Value = productResult.Product.enable_instant_pin_reissue_YN;
                    command.Parameters.Add("@pin_calc_method_id", SqlDbType.Int).Value = productResult.Product.pin_calc_method_id;
                    command.Parameters.Add("@min_pin_length", SqlDbType.Int).Value = productResult.Product.min_pin_length;
                    command.Parameters.Add("@max_pin_length", SqlDbType.Int).Value = productResult.Product.max_pin_length;

                    command.Parameters.Add("@cms_exportable_YN", SqlDbType.Bit).Value = productResult.Product.cms_exportable_YN;
                    command.Parameters.Add("@product_load_type_id", SqlDbType.Int).Value = productResult.Product.product_load_type_id;

                    command.Parameters.Add("@auto_approve_batch_YN", SqlDbType.Bit).Value = productResult.Product.auto_approve_batch_YN;
                    command.Parameters.Add("@account_validation_YN", SqlDbType.Bit).Value = productResult.Product.account_validation_YN;

                    command.Parameters.Add("@pin_mailer_printing_YN", SqlDbType.Bit).Value = productResult.Product.pin_mailer_printing_YN;
                    command.Parameters.Add("@pin_mailer_reprint_YN", SqlDbType.Bit).Value = productResult.Product.pin_mailer_reprint_YN;
                    command.Parameters.Add("@e_pin_request_YN", SqlDbType.Bit).Value = productResult.Product.e_pin_request_YN ?? false;

                    command.Parameters.Add("@production_dist_batch_status_flow_id", SqlDbType.Int).Value = productResult.Product.production_dist_batch_status_flow;
                    command.Parameters.Add("@distribution_dist_batch_status_flow_id", SqlDbType.Int).Value = productResult.Product.distribution_dist_batch_status_flow;
                    command.Parameters.Add("@charge_fee_to_issuing_branch_YN", SqlDbType.Bit).Value = productResult.Product.charge_fee_to_issuing_branch_YN;
                    command.Parameters.Add("@charge_fee_at_cardrequest", SqlDbType.Bit).Value = productResult.Product.charge_fee_at_cardrequest;
                   
                    command.Parameters.AddWithValue("@card_issue_reasons_list", UtilityClass.CreateKeyValueTable(productResult.CardIssueReasons));
                    command.Parameters.AddWithValue("@account_types_list", UtilityClass.CreateKeyValueTable(productResult.AccountTypes));
                    command.Parameters.AddWithValue("@currencylist", dt_currency);
                    command.Parameters.AddWithValue("@external_system_fields", dt_externalsystemfields);

                    command.Parameters.AddWithValue("@prod_interface_parameters_list", UtilityClass.CreateBiKeyValueTable(prodInterfaceParams));
                    command.Parameters.AddWithValue("@issue_interface_parameters_list", UtilityClass.CreateBiKeyValueTable(issueInterfaceParams));
                    command.Parameters.AddWithValue("@accounttype_interface_parameters_list", dt_accounttpes);

                    command.Parameters.Add("@decimalisation_table", SqlDbType.VarChar).Value = productResult.Product.decimalisation_table;
                    command.Parameters.Add("@pin_validation_data", SqlDbType.VarChar).Value = productResult.Product.pin_validation_data;
                    command.Parameters.Add("@pin_block_formatid", SqlDbType.VarChar).Value = productResult.Product.pin_block_formatid;

                    command.Parameters.Add("@allow_manual_uploaded_YN", SqlDbType.Bit).Value = productResult.Product.allow_manual_uploaded_YN;
                    command.Parameters.Add("@allow_reupload_YN", SqlDbType.Bit).Value = productResult.Product.allow_reupload_YN;
                    command.Parameters.Add("@remote_cms_update_YN", SqlDbType.Bit).Value = productResult.Product.remote_cms_update_YN;
                    command.Parameters.Add("@pin_account_validation_YN", SqlDbType.Bit).Value = productResult.Product.pin_account_validation_YN;

                    command.Parameters.Add("@renewal_activate_card", SqlDbType.Bit).Value = productResult.Product.renewal_activate_card;
                    command.Parameters.Add("@renewal_charge_card", SqlDbType.Bit).Value = productResult.Product.renewal_charge_card;

                    command.Parameters.Add("@credit_limit_approve", SqlDbType.Bit).Value = productResult.Product.credit_limit_approve;
                    command.Parameters.Add("@credit_limit_capture", SqlDbType.Bit).Value = productResult.Product.credit_limit_capture;
                    command.Parameters.Add("@credit_limit_update", SqlDbType.Bit).Value = productResult.Product.credit_limit_update;

                    command.Parameters.Add("@email_required", SqlDbType.Bit).Value = productResult.Product.email_required;
                    command.Parameters.Add("@generate_contract_number", SqlDbType.Bit).Value = productResult.Product.generate_contract_number;
                    command.Parameters.Add("@manual_contract_number", SqlDbType.Bit).Value = productResult.Product.manual_contract_number;
                    command.Parameters.Add("@parallel_approval", SqlDbType.Bit).Value = productResult.Product.parallel_approval;
                    command.Parameters.Add("@activation_by_center_operator", SqlDbType.Bit).Value = productResult.Product.activation_by_center_operator;

                    command.Parameters.Add("@credit_contract_prefix", SqlDbType.VarChar).Value = productResult.Product.credit_contract_prefix;
                    command.Parameters.Add("@credit_contract_suffix_format", SqlDbType.VarChar).Value = productResult.Product.credit_contract_suffix_format;
                    command.Parameters.Add("@credit_contract_last_sequence", SqlDbType.BigInt).Value = productResult.Product.credit_contract_last_sequence;
                    command.Parameters.Add("@is_m_twenty_printed", SqlDbType.Bit).Value = productResult.Product.Is_mtwenty_printed;

                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.Parameters.Add("@production_dist_batch_status_flow_renewal", SqlDbType.Int).Value = productResult.Product.production_dist_batch_status_flow_renewal;
                    command.Parameters.Add("@distribution_dist_batch_status_flow_renewal", SqlDbType.Int).Value = productResult.Product.distribution_dist_batch_status_flow_renewal;

                    command.ExecuteNonQuery();


                    resultCode = int.Parse(command.Parameters["@ResultCode"].Value.ToString());
                }
            }

            return (SystemResponseCode)resultCode;
        }

        /// <summary>
        /// Deleteing Product
        /// </summary>
        /// <param name="Productid"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public DBResponseMessage DeleteProduct(int Productid, long auditUserId, string auditWorkstation)
        {
            var result_code = new ObjectParameter("result_code", "0");
            //var commited = false;
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {

                context.usp_delete_product(Productid, auditUserId, auditWorkstation, result_code);

            }
            int resultCode = 0;
            if (int.TryParse(result_code.Value.ToString(), out resultCode))
            {

            }
            return (DBResponseMessage)resultCode;
        }

        public SystemResponseCode ActivateProduct(int Productid, long auditUserId, string auditWorkstation)
        {
            var result_code = new ObjectParameter("result_code", "0");
            //var commited = false;
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_activate_product(Productid, auditUserId, auditWorkstation, result_code);
            }
            int resultCode = 0;
            if (int.TryParse(result_code.Value.ToString(), out resultCode))
            {

            }
            return (SystemResponseCode)resultCode;
        }

        /// <summary>
        /// Deleting font from table
        /// </summary>
        /// <param name="Fontid"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public DBResponseMessage DeleteFont(int Fontid, long auditUserId, string auditWorkstation)
        {
            var result_code = new ObjectParameter("result_code", "0");

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {

                context.usp_delete_font(Fontid, auditUserId, auditWorkstation, result_code);

            }
            int resultCode = 0;
            if (int.TryParse(result_code.Value.ToString(), out resultCode))
            {

            }
            return (DBResponseMessage)resultCode;
        }

        /// <summary>
        /// Delete sub product
        /// </summary>
        /// <param name="Productid"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public DBResponseMessage DeleteSubProduct(int Productid, int subproductid, long auditUserId, string auditWorkstation)
        {
            var result_code = new ObjectParameter("ResultCode", "0");
            //var commited = false;
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {

                context.usp_delete_subproduct(Productid, subproductid, auditUserId, auditWorkstation, result_code);

            }
            int resultCode = 0;
            if (int.TryParse(result_code.Value.ToString(), out resultCode))
            {

            }
            return (DBResponseMessage)resultCode;
        }

        /// <summary>
        ///  reading subproduct list for product
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="RowsPerpage"></param>
        /// <returns></returns>

        public List<SubProduct_Result> GetSubProductList(int issuer_id, int? product_id, int? cardIssueMethidId, Boolean? deletedYN, int pageIndex, int RowsPerpage)
        {

            List<SubProduct_Result> rtnValue = new List<SubProduct_Result>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<SubProduct_Result> results = context.usp_get_subproduct_list(issuer_id, product_id, cardIssueMethidId, deletedYN, pageIndex, RowsPerpage);


                foreach (SubProduct_Result result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;


        }

        /// <summary>
        /// reading subproduct details in editmode
        /// </summary>
        /// <param name="fontid"></param>
        /// <returns></returns>
        public SubProduct_Result GetSubProduct(int? product_id, int sub_productid)
        {

            SubProduct_Result rtnValue = new SubProduct_Result();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<SubProduct_Result> results = context.usp_get_subproduct(product_id, sub_productid);


                foreach (SubProduct_Result result in results)
                {
                    rtnValue = result;
                }
            }
            return rtnValue;
        }

        /// <summary>
        ///  getting Font list for grid
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="RowsPerpage"></param>
        /// <returns></returns>
        public List<FontResult> GetFontListBypage(int pageIndex, int RowsPerpage)
        {
            List<FontResult> rtnValue = new List<FontResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<FontResult> results = context.usp_get_fonts_list(pageIndex, RowsPerpage);


                foreach (FontResult result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;
        }

        /// <summary>
        /// getting Font Details by font id
        /// </summary>
        /// <param name="fontid"></param>
        /// <returns></returns>
        public FontResult GetFont(int fontid)
        {
            FontResult rtnValue = new FontResult();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<FontResult> results = context.usp_getFont_by_fontid(fontid);


                foreach (FontResult result in results)
                {
                    rtnValue = result;
                }
            }

            return rtnValue;
        }

        /// <summary>
        /// Inserting Font in issuer_product_font
        /// </summary>
        /// <param name="fontresult"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="dbResponse"></param>
        /// <returns></returns>
        public long InsertFont(FontResult fontresult, long auditUserId, string auditWorkstation, out DBResponseMessage dbResponse)
        {

            var result_code = new ObjectParameter("ResultCode", "0");
            var new_font_id = new ObjectParameter("new_font_id", "0");

            //var commited = false;
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {

                context.usp_insert_font(fontresult.font_name, fontresult.resource_path,
                                       auditUserId, auditWorkstation, result_code, new_font_id);
            }

            int resultCode = 0, newfontid = 0;
            if (int.TryParse(result_code.Value.ToString(), out resultCode))
            {

            }

            if (int.TryParse(new_font_id.Value.ToString(), out newfontid))
            {

            }

            dbResponse = (DBResponseMessage)resultCode;
            return newfontid;
        }

        /// <summary>
        /// UpdateFont
        /// </summary>
        /// <param name="fontresult"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public DBResponseMessage UpdateFont(FontResult fontresult, long auditUserId, string auditWorkstation)
        {
            var result_code = new ObjectParameter("ResultCode", "0");

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {

                context.usp_update_font(fontresult.font_id, fontresult.font_name, fontresult.resource_path,
                                       auditUserId, auditWorkstation, result_code);
            }

            int resultCode = 0;
            if (int.TryParse(result_code.Value.ToString(), out resultCode))
            {

            }
            return (DBResponseMessage)resultCode;
        }


        public List<ProductFeeAccountingResult> GetFeeAccountingList(int? issuerId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<ProductFeeAccountingResult> results = context.usp_get_product_fee_accounting_list(issuerId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);

                return results.ToList();
            }
        }

        public ProductFeeAccountingResult GetFeeAccounting(int feeAccountingId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<ProductFeeAccountingResult> results = context.usp_get_product_fee_accounting(feeAccountingId);

                return results.First();
            }
        }

        public SystemResponseCode CreateFeeAccounting(ProductFeeAccountingResult feeAccountingDetails, long auditUserId, string auditWorkstation, out int feeAccountingId)
        {
            var result_code = new ObjectParameter("ResultCode", "0");
            var newId = new ObjectParameter("new_fee_accounting_id", "0");

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_create_product_fee_accounting(
                    feeAccountingDetails.fee_accounting_name,
                    feeAccountingDetails.issuer_id,
                    feeAccountingDetails.fee_revenue_account_no,
                    feeAccountingDetails.fee_revenue_account_type_id,
                    feeAccountingDetails.fee_revenue_branch_code,
                    feeAccountingDetails.fee_revenue_narration_en,
                    feeAccountingDetails.fee_revenue_narration_fr,
                    feeAccountingDetails.fee_revenue_narration_pt,
                    feeAccountingDetails.fee_revenue_narration_es,
                    feeAccountingDetails.vat_account_no,
                    feeAccountingDetails.vat_account_type_id,
                    feeAccountingDetails.vat_account_branch_code,
                    feeAccountingDetails.vat_narration_en,
                    feeAccountingDetails.vat_narration_fr,
                    feeAccountingDetails.vat_narration_pt,
                    feeAccountingDetails.vat_narration_es,
                    auditUserId, auditWorkstation, newId, result_code);

                feeAccountingId = int.Parse(newId.Value.ToString());

                return (SystemResponseCode)int.Parse(result_code.Value.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="feeSchemeDetails"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public SystemResponseCode UpdateFeeAccounting(ProductFeeAccountingResult feeAccountingDetails, long auditUserId, string auditWorkstation)
        {
            var result_code = new ObjectParameter("ResultCode", "0");

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_update_product_fee_accounting(
                    feeAccountingDetails.fee_accounting_id,
                    feeAccountingDetails.fee_accounting_name,
                    feeAccountingDetails.issuer_id,
                    feeAccountingDetails.fee_revenue_account_no,
                    feeAccountingDetails.fee_revenue_account_type_id,
                    feeAccountingDetails.fee_revenue_branch_code,
                    feeAccountingDetails.fee_revenue_narration_en,
                    feeAccountingDetails.fee_revenue_narration_fr,
                    feeAccountingDetails.fee_revenue_narration_pt,
                    feeAccountingDetails.fee_revenue_narration_es,
                    feeAccountingDetails.vat_account_no,
                    feeAccountingDetails.vat_account_type_id,
                    feeAccountingDetails.vat_account_branch_code,
                    feeAccountingDetails.vat_narration_en,
                    feeAccountingDetails.vat_narration_fr,
                    feeAccountingDetails.vat_narration_pt,
                    feeAccountingDetails.vat_narration_es,
                    auditUserId, auditWorkstation, result_code);

                return (SystemResponseCode)int.Parse(result_code.Value.ToString());
            }
        }

        /// <summary>
        /// Delete a fee accounting
        /// </summary>
        /// <param name="feeAccounting"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public SystemResponseCode DeleteFeeAccounting(int feeAccountingId, long auditUserId, string auditWorkstation)
        {
            var result_code = new ObjectParameter("ResultCode", "0");

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_delete_product_fee_accounting(feeAccountingId, auditUserId, auditWorkstation, result_code);

                return (SystemResponseCode)int.Parse(result_code.Value.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<FeeSchemeResult> GetFeeSchemes(int? issuerId, int pageIndex, int rowsPerPage, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<FeeSchemeResult> results = context.usp_get_product_fee_scheme_list(issuerId, pageIndex, rowsPerPage, auditUserId, auditWorkstation);

                return results.ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="feeSchemeId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public FeeSchemeDetails GetFeeSchemeDetails(int feeSchemeId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<FeeSchemeResult> results = context.usp_get_product_fee_scheme(feeSchemeId, auditUserId, auditWorkstation);

                FeeSchemeDetails feeSchemeDetails = new FeeSchemeDetails(results.First());

                var feeDetails = this.GetFeeDetails(feeSchemeDetails.fee_scheme_id, auditUserId, auditWorkstation);

                feeSchemeDetails.FeeDetails = feeDetails;

                return feeSchemeDetails;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="feeSchemeId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<FeeDetailResult> GetFeeDetails(int feeSchemeId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<FeeDetailResult> results = context.usp_get_product_fee_details(feeSchemeId, auditUserId, auditWorkstation);

                return results.ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="subproductId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<ProductFeeDetailsResult> GetFeeDetailByProduct(int productId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<ProductFeeDetailsResult> results = context.usp_get_product_details_by_product(productId);

                return results.ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="feeDetailId"></param>
        /// <param name="currencyId"></param>
        /// <param name="CardIssueReasonId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public ProductChargeResult GetCurrentFees(int feeDetailId, int currencyId, int CardIssueReasonId, string CBSAccountType, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<ProductChargeResult> results = context.usp_get_current_product_fee(feeDetailId, currencyId, CardIssueReasonId, CBSAccountType, auditUserId, auditWorkstation);

                return results.FirstOrDefault();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="feeDetailId"></param>
        /// <param name="issueReasonId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<FeeChargeResult> GetFeeCharges(int feeDetailId, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<FeeChargeResult> results = context.usp_get_product_fee_charges(feeDetailId);

                return results.ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="feeDetailId"></param>
        /// <param name="cardIssueReasonId"></param>
        /// <param name="fees"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public SystemResponseCode UpdateFeeCharges(int feeDetailId, List<FeeChargeResult> fees, long auditUserId, string auditWorkstation)
        {
            int resultCode;

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    DataTable dt_fees_list = new DataTable();
                    dt_fees_list.Columns.Add("currency_id", typeof(int));
                    dt_fees_list.Columns.Add("fee_charge", typeof(decimal));
                    dt_fees_list.Columns.Add("vat", typeof(decimal));
                    dt_fees_list.Columns.Add("cbs_account_type", typeof(string));
                    dt_fees_list.Columns.Add("card_issue_reason_id", typeof(int));
                    DataRow workRow;


                    foreach (var item in fees)
                    {
                        workRow = dt_fees_list.NewRow();
                        workRow["currency_id"] = item.currency_id;
                        workRow["fee_charge"] = item.fee_charge;
                        workRow["vat"] = item.vat;
                        workRow["cbs_account_type"] = item.cbs_account_type;
                        workRow["card_issue_reason_id"] = item.card_issue_reason_id;
                        dt_fees_list.Rows.Add(workRow);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_update_fee_charge]";
                    command.Parameters.Add("@fee_detail_id", SqlDbType.Int).Value = feeDetailId;


                    command.Parameters.AddWithValue("@fee_list", dt_fees_list);
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    resultCode = int.Parse(command.Parameters["@ResultCode"].Value.ToString());
                }
            }

            return (SystemResponseCode)resultCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="feeSchemeDetails"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <param name="feeSchemeId"></param>
        /// <returns></returns>
        public SystemResponseCode InsertFeeScheme(FeeSchemeDetails feeSchemeDetails, long auditUserId, string auditWorkstation, out int feeSchemeId)
        {
            int resultCode;

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    DataTable dt_fees_list = new DataTable();
                    dt_fees_list.Columns.Add("fee_scheme_id", typeof(int));
                    dt_fees_list.Columns.Add("fee_detail_id", typeof(int));
                    dt_fees_list.Columns.Add("fee_detail_name", typeof(string));
                    dt_fees_list.Columns.Add("fee_waiver_YN", typeof(bool));
                    dt_fees_list.Columns.Add("fee_editable_TN", typeof(bool));

                    //dt_fees_list.Columns.Add("effective_from", typeof(DateTime));
                    //dt_fees_list.Columns.Add("effective_to", typeof(DateTime));                    

                    DataRow workRow;

                    foreach (var item in feeSchemeDetails.FeeDetails)
                    {
                        workRow = dt_fees_list.NewRow();
                        workRow["fee_scheme_id"] = 0;
                        workRow["fee_detail_id"] = 0;
                        workRow["fee_detail_name"] = item.fee_detail_name;
                        workRow["fee_waiver_YN"] = item.fee_waiver_YN;
                        workRow["fee_editable_TN"] = item.fee_editable_YN;

                        //workRow["effective_from"] = item.effective_from;
                        //workRow["effective_to"] = item.effective_to;
                        dt_fees_list.Rows.Add(workRow);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_create_fee_scheme]";
                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = feeSchemeDetails.issuer_id;
                    command.Parameters.Add("@fee_accounting_id", SqlDbType.Int).Value = feeSchemeDetails.fee_accounting_id;
                    command.Parameters.Add("@fee_scheme_name", SqlDbType.VarChar).Value = feeSchemeDetails.fee_scheme_name;

                    command.Parameters.AddWithValue("@fee_detail_list", dt_fees_list);
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;
                    command.Parameters.Add("@new_fee_scheme_id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    feeSchemeId = int.Parse(command.Parameters["@new_fee_scheme_id"].Value.ToString());
                    resultCode = int.Parse(command.Parameters["@ResultCode"].Value.ToString());
                }
            }
            return (SystemResponseCode)resultCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="feeSchemeDetails"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public SystemResponseCode UpdateFeeScheme(FeeSchemeDetails feeSchemeDetails, long auditUserId, string auditWorkstation)
        {
            int resultCode;

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    DataTable dt_fees_list = new DataTable();
                    dt_fees_list.Columns.Add("fee_scheme_id", typeof(int));
                    dt_fees_list.Columns.Add("fee_detail_id", typeof(int));
                    dt_fees_list.Columns.Add("fee_detail_name", typeof(string));
                    dt_fees_list.Columns.Add("fee_waiver_YN", typeof(bool));
                    dt_fees_list.Columns.Add("fee_editable_TN", typeof(bool));

                    //dt_fees_list.Columns.Add("effective_from", typeof(DateTime));
                    //dt_fees_list.Columns.Add("effective_to", typeof(DateTime));                    

                    DataRow workRow;

                    foreach (var item in feeSchemeDetails.FeeDetails)
                    {
                        workRow = dt_fees_list.NewRow();
                        workRow["fee_scheme_id"] = feeSchemeDetails.fee_scheme_id;
                        workRow["fee_detail_id"] = item.fee_detail_id;
                        workRow["fee_detail_name"] = item.fee_detail_name;
                        workRow["fee_waiver_YN"] = item.fee_waiver_YN;
                        workRow["fee_editable_TN"] = item.fee_editable_YN;

                        //workRow["effective_from"] = item.effective_from;
                        //workRow["effective_to"] = item.effective_to;
                        dt_fees_list.Rows.Add(workRow);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_update_fee_scheme]";
                    command.Parameters.Add("@fee_scheme_id", SqlDbType.Int).Value = feeSchemeDetails.fee_scheme_id;
                    command.Parameters.Add("@issuer_id", SqlDbType.Int).Value = feeSchemeDetails.issuer_id;
                    command.Parameters.Add("@fee_accounting_id", SqlDbType.Int).Value = feeSchemeDetails.fee_accounting_id;
                    command.Parameters.Add("@fee_scheme_name", SqlDbType.VarChar).Value = feeSchemeDetails.fee_scheme_name;

                    command.Parameters.AddWithValue("@fee_detail_list", dt_fees_list);
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    resultCode = int.Parse(command.Parameters["@ResultCode"].Value.ToString());
                }
            }

            return (SystemResponseCode)resultCode;
        }

        /// <summary>
        /// Delete a fee scheme
        /// </summary>
        /// <param name="feeSchemeId"></param>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public SystemResponseCode DeleteFeeScheme(int feeSchemeId, long auditUserId, string auditWorkstation)
        {
            var result_code = new ObjectParameter("ResultCode", "0");

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_delete_fee_scheme(feeSchemeId, auditUserId, auditWorkstation, result_code);

                return (SystemResponseCode)int.Parse(result_code.Value.ToString());
            }
        }

        public void UpdateFeeChargeStatus(long cardId, int cardFeeChargeStatusId, string feeReferenceNumber, string feeReversalRefNumber, long auditUserId, string auditWorkstation)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                context.usp_fee_charge_update_status(cardId, cardFeeChargeStatusId, feeReferenceNumber, feeReversalRefNumber, auditUserId, auditWorkstation);
            }
        }

        /// <summary>
        ///     gets the latest PWK from issuer table
        /// </summary>
        /// <param name="issuerID">int: issuer ID</param>
        /// <returns>PWK as string or literal empty string if not found</returns>
        public string GetPWKey(int issuerID)
        {
            string defaultKey = "";


            using (SqlConnection connection = _dbObject.SQLConnection)
            {
                string sql = "SELECT encrypted_PWK From issuer WHERE issuer_id = '" + issuerID + "'";

                using (var command = new SqlCommand(sql, connection))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        if (dataReader.HasRows)
                        {
                            dataReader.Read();
                            defaultKey = dataReader[0] != null ? dataReader[0].ToString() : "";
                        }
                        //return null;
                    }
                }
            }

            return defaultKey;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTable"></param>
        /// <param name="issueCardStatus"></param>
        /// <returns></returns>
        private DataRow addCardStatus(DataTable dateTable, DistCardStatus issueCardStatus)
        {
            DataRow workRow;
            workRow = dateTable.NewRow();
            workRow["card_status"] = issueCardStatus;

            return workRow;
        }

        /// <summary>
        ///     reserves a card for a customer
        /// </summary>
        /// <param name="reserveRequest">a new status to set for card</param>
        /// <returns></returns>
        public string ReserveCardForCustomer(CardIssueRequest reserveRequest, DistCardStatus nextReservationStage)
        {
            DataTable dt_parm_card_expected_status = new DataTable();
            dt_parm_card_expected_status.Columns.Add("card_status", typeof(String));

            //var expectedStatus = IssueCardStatus.INVALID;

            //switch (nextReservationStage)
            //{
            //    case DistCardStatus.ALLOCATED_TO_CUST:
            //        //expectedStatus = IssueCardStatus.AVAILABLE_FOR_ISSUE;
            //        dt_parm_card_expected_status.Rows.Add(addCardStatus(dt_parm_card_expected_status, IssueCardStatus.AVAILABLE_FOR_ISSUE));
            //        break;
            //    case IssueCardStatus.SPOILED:
            //        //expectedStatus = IssueCardStatus.ALLOCATED_TO_CUST;
            //        dt_parm_card_expected_status.Rows.Add(addCardStatus(dt_parm_card_expected_status, IssueCardStatus.ALLOCATED_TO_CUST));
            //        dt_parm_card_expected_status.Rows.Add(addCardStatus(dt_parm_card_expected_status, IssueCardStatus.CARD_PRINTED));
            //        break;
            //    case IssueCardStatus.CARD_PRINTED:
            //        //expectedStatus = IssueCardStatus.ALLOCATED_TO_CUST;
            //        dt_parm_card_expected_status.Rows.Add(addCardStatus(dt_parm_card_expected_status, IssueCardStatus.ALLOCATED_TO_CUST));
            //        break;
            //    case IssueCardStatus.ISSUED:
            //        //expectedStatus = IssueCardStatus.CARD_PRINTED;
            //        dt_parm_card_expected_status.Rows.Add(addCardStatus(dt_parm_card_expected_status, IssueCardStatus.CARD_PRINTED));
            //        break;
            //    default:
            //        dt_parm_card_expected_status.Rows.Add(addCardStatus(dt_parm_card_expected_status, IssueCardStatus.INVALID));
            //        break;
            //}
            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "usp_issue_card_to_customer";
                    command.Parameters.Add("@card_number", SqlDbType.VarChar).Value = reserveRequest.CardNumber;
                    //command.Parameters.Add("@card_expected_status", SqlDbType.VarChar).Value = expectedStatus;
                    command.Parameters.AddWithValue("@card_expected_status_list", dt_parm_card_expected_status);
                    command.Parameters.Add("@card_issued_status", SqlDbType.VarChar).Value = nextReservationStage;
                    command.Parameters.Add("@user", SqlDbType.VarChar).Value = reserveRequest.UserIssuing;
                    command.Parameters.Add("@user_branch_code", SqlDbType.VarChar).Value = reserveRequest.BranchCode;
                    command.Parameters.Add("@customer_first_name", SqlDbType.VarChar).Value =
                        reserveRequest.Customer.FirstName;
                    command.Parameters.Add("@customer_last_name", SqlDbType.VarChar).Value =
                        reserveRequest.Customer.LastName;
                    command.Parameters.Add("@account_number", SqlDbType.VarChar).Value =
                        reserveRequest.Customer.PrimaryAccountNumber;
                    command.Parameters.Add("@account_type", SqlDbType.VarChar).Value =
                        reserveRequest.Customer.AccountType.ToString();
                    command.Parameters.Add("@reason_for_issue", SqlDbType.VarChar).Value =
                        reserveRequest.ReasonForIssue.ToString(CultureInfo.InvariantCulture);
                    command.Parameters.Add("@customer_title", SqlDbType.VarChar).Value =
                        reserveRequest.Customer.Title.ToString(CultureInfo.InvariantCulture);
                    command.Parameters.Add("@response_message", SqlDbType.Int).Direction =
                        ParameterDirection.ReturnValue;



                    int resutl = command.ExecuteNonQuery();

                    string dbresponseCode = command.Parameters["@response_message"].Value.ToString();
                    return dbresponseCode;
                }
            }
        }

        /// <summary>
        /// Searching Customer card Details 
        /// </summary>
        /// <param name="cardrefno"></param>
        /// <param name="customeraccountno"></param>
        /// <param name="pageIndex"></param>
        /// <param name="RowsPerPage"></param>
        /// <returns></returns>
        public List<CustomercardsearchResult> SearchCustomerCardsList(int? issuerid, int? branchid, int? productid, int? cardissuemethodid, int? priorityid, 
            string cardrefno, string customeraccountno, int pageIndex, int RowsPerPage, long auditUserId, string auditWorkstation, bool renewalSearch, bool activationSearch)
        {
            List<CustomercardsearchResult> rtnValue = new List<CustomercardsearchResult>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<CustomercardsearchResult> results = null;
                if (renewalSearch == false && activationSearch == false)
                {
                    results = context.usp_get_customercardsearch_list(issuerid, branchid, cardrefno,
                        customeraccountno, productid, priorityid, cardissuemethodid, pageIndex, RowsPerPage, auditUserId, auditWorkstation);
                }
                else if (renewalSearch==true)
                {
                    results = context.usp_get_customercardsearch_list_renewal(issuerid, branchid, cardrefno,
                        customeraccountno, productid, priorityid, cardissuemethodid, pageIndex, RowsPerPage, auditUserId, auditWorkstation);
                }
                else if (activationSearch==true)
                {
                    results = context.usp_get_customercardsearch_list_activation(issuerid, branchid, cardrefno,
                        customeraccountno, productid, priorityid, cardissuemethodid, pageIndex, RowsPerPage, auditUserId, auditWorkstation);
                }


                foreach (CustomercardsearchResult result in results)
                {
                    rtnValue.Add(result);
                }
            }

            return rtnValue;

        }

        #endregion

        #region CARD FOR DISTRIBUTION BATCH

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cardList"></param>
        /// <param name="batchReference"></param>
        /// <param name="currentCardStatus"></param>
        /// <param name="newLoadCardStatus"></param>
        /// <returns></returns>
        public string ReserveLoadCardsForDistBatch(List<string> cardList, string batchReference,
                                                     string currentCardStatus, string newLoadCardStatus)
        {

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "usp_reserve_load_cards_for_dist_batch";

                    command.Parameters.Add("@batch_reference", SqlDbType.VarChar).Value = batchReference;
                    command.Parameters.Add("@current_card_status", SqlDbType.VarChar).Value = currentCardStatus;
                    command.Parameters.Add("@new_load_card_status", SqlDbType.VarChar).Value = newLoadCardStatus;

                    //string strCardList = "";
                    //if (cardList != null)
                    //{
                    //    for (int i = 0; i < cardList.Count; i++)
                    //    {
                    //        strCardList = strCardList + cardList[i] + ",";
                    //    }
                    //}

                    DataTable dt_DistBatchCards = new DataTable();
                    dt_DistBatchCards.Columns.Add("card_number", typeof(String));
                    DataRow workRow;

                    foreach (string line in cardList)
                    {
                        workRow = dt_DistBatchCards.NewRow();

                        workRow["card_number"] = line.Trim();

                        dt_DistBatchCards.Rows.Add(workRow);
                    }

                    command.Parameters.AddWithValue("@card_list", dt_DistBatchCards);
                    command.Parameters.Add("@response_message", SqlDbType.Int).Direction =
                        ParameterDirection.ReturnValue;

                    int rows = command.ExecuteNonQuery();

                    return command.Parameters["@response_message"].Value.ToString();
                }
            }
        }

        #endregion

        #region CARDS FOR LOAD BATCH

        /// <summary>
        /// 
        /// </summary>
        /// <param name="issuerID"></param>
        /// <param name="_loadDateFrom"></param>
        /// <param name="_loadDateTo"></param>
        /// <param name="cardNumberPrefix"></param>
        /// <param name="loadBatchReference"></param>
        /// <param name="loadCardStatus"></param>
        /// <returns></returns>
        public List<LoadCard> GetLoadCards(int issuerID, DateTime _loadDateFrom, DateTime _loadDateTo,
                                             string cardNumberPrefix, string loadBatchReference, string loadCardStatus)
        {
            //RL --- validate if anyone uses this Method.
            var cards = new List<LoadCard>();


            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "usp_get_Load_cards";

                    command.Parameters.Add("@issuerID", SqlDbType.Int).Value = issuerID;
                    command.Parameters.Add("@cardPrefix", SqlDbType.VarChar).Value = cardNumberPrefix != null
                                                                                         ? cardNumberPrefix
                                                                                         : "";
                    command.Parameters.Add("@batchRefrence", SqlDbType.VarChar).Value = cardNumberPrefix != null
                                                                                            ? cardNumberPrefix
                                                                                            : "";
                    command.Parameters.Add("@status", SqlDbType.VarChar).Value = loadCardStatus != null
                                                                                     ? loadCardStatus
                                                                                     : "";

                    command.Parameters.Add("@dateFrom", SqlDbType.DateTime).Value = _loadDateFrom;
                    command.Parameters.Add("@dateTo", SqlDbType.DateTime).Value = _loadDateTo;

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            LoadCard card = CreateLoadCardObject(dataReader);
                            cards.Add(card);
                        }
                    }
                }
            }
            return cards;
        }

        #endregion

        #region PRIVATE FUNCTIONS

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        private IssueCard CreateIssueCardObject(SqlDataReader dataReader)
        {
            return null;
            //string cardID = dataReader["card_id"] != null ? dataReader["card_id"].ToString() : null;
            //string cardNumber = dataReader["card_number"] != null
            //                        ? CommonGeneral.Storage_DecryptValue((byte[]) dataReader["card_number"])
            //                        : null;
            //string seqeunce = dataReader["seqeunce"] != null ? dataReader["seqeunce"].ToString() : null;
            //string batchReference = dataReader["dist_batch_reference"] != null
            //                            ? dataReader["dist_batch_reference"].ToString()
            //                            : null;
            //string cardStatus = dataReader["card_status"] != null ? dataReader["card_status"].ToString() : null;
            //string dateIssued = dataReader["date_issued"] != null ? dataReader["date_issued"].ToString() : null;
            //string issuedBy = dataReader["issued_by"] != null ? dataReader["issued_by"].ToString() : null;
            //string customerFirstName = dataReader["customer_first_name"] != null
            //                               ? dataReader["customer_first_name"].ToString()
            //                               : null;
            //string customerLastName = dataReader["customer_last_name"] != null
            //                              ? dataReader["customer_last_name"].ToString()
            //                              : null;

            ////am I missing something here because this line throws and exception
            ////will troubleshoot it sometime. don't have time for this right now
            ////string accountNumber = dataReader["customer_account"] != null ? IndigoCardIssuanceCommon.CommonGeneral.Storage_DecryptValue((byte[])dataReader["customer_account"]) : null;
            //string accountNumber = null;
            //if (dataReader["customer_account"] != null)
            //{
            //    byte[] byteAccount = !dataReader.GetSqlBytes(9).IsNull ? dataReader.GetSqlBytes(9).Value : null;

            //    if (byteAccount != null)
            //        accountNumber = CommonGeneral.Storage_DecryptValue(byteAccount);

            //    //byte?[] byteValue = (byte?[])dataReader["customer_account"];
            //    //if (byteValue != null)
            //    //{

            //    //}
            //    //accountNumber = IndigoCardIssuanceCommon.CommonGeneral.Storage_DecryptValue((byte[])dataReader["customer_account"]);
            //}


            //string accountType = dataReader["account_type"] != null ? dataReader["account_type"].ToString() : null;
            //string nameOnCard = dataReader["name_on_card"] != null ? dataReader["name_on_card"].ToString() : null;
            //string issuerID = dataReader["issuer_id"] != null ? dataReader["issuer_id"].ToString() : null;
            //string branchCode = dataReader["branch_code"] != null ? dataReader["branch_code"].ToString() : null;
            //string reasonForIssue = dataReader["reason_for_issue"] != null
            //                            ? dataReader["reason_for_issue"].ToString()
            //                            : null;
            //string assigned_operator = dataReader["assigned_operator"] != null ?
            //    dataReader["assigned_operator"].ToString() : null;

            //string customer_title = dataReader["customer_title"] != null ?
            //    dataReader["customer_title"].ToString() : null;

            //DistCardStatus status = csGeneral.GetIssueCardStatus(cardStatus);
            //int intCardID = 0;
            //Int32.TryParse(cardID, out intCardID);

            //int intIssuerID = 0;
            //Int32.TryParse(issuerID, out intIssuerID);

            //DateTime issueDate = DateTime.MinValue;
            //if (dateIssued != null && !dateIssued.Equals(""))
            //    issueDate = DateTime.Parse(dateIssued);

            //var card = new IssueCard(intCardID, cardNumber, intIssuerID, batchReference, issueDate,
            //                         status, issuedBy, branchCode, customerFirstName, customerLastName, accountNumber,
            //                         accountType, reasonForIssue,assigned_operator,customer_title);

            //return card;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        private List<IssueCard> CreateIssueCardList(SqlDataReader dataReader)
        {
            var loadBatchList = new List<IssueCard>();

            while (dataReader.Read())
            {
                IssueCard batch = CreateIssueCardObject(dataReader);
                loadBatchList.Add(batch);
            }
            return loadBatchList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        private LoadCard CreateLoadCardObject(SqlDataReader dataReader)
        {
            string cardNumber = dataReader["card_number"] != null
                                    ? CommonGeneral.Storage_DecryptValue((byte[])dataReader["card_number"])
                                    : null;
            //   string seqeunce = dataReader["seqeunce"] != null ? dataReader["seqeunce"].ToString() : null;
            string loadBatchRefrence = dataReader["load_batch_reference"] != null
                                           ? CommonGeneral.Storage_DecryptValue(
                                               (byte[])dataReader["load_batch_reference"])
                                           : null;
            string cardStatus = dataReader["card_status"] != null
                                    ? CommonGeneral.Storage_DecryptValue((byte[])dataReader["card_status"])
                                    : null;
            string dateLoaded = dataReader["load_date"] != null ? dataReader["load_date"].ToString() : null;
            string issuerID = dataReader["issuer_id"] != null ? dataReader["issuer_id"].ToString() : null;


            LoadCardStatus status = csGeneral.GetLoadCardStatus(cardStatus);

            int issuerIntID = 0;
            Int32.TryParse(issuerID, out issuerIntID);

            DateTime loadDate = DateTime.MinValue;
            if (dateLoaded != null)
                loadDate = DateTime.Parse(dateLoaded);

            var card = new LoadCard(0, cardNumber, "00", issuerIntID, loadBatchRefrence, loadDate, status);

            return card;
        }

        #endregion

        #region Product Print Fields

        public SystemResponseCode CreateProductPrintFields(List<ProductPrintFieldResult> productPrintFields, long auditUserId, string auditWorkstation)
        {
            int resultCode;

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    DataTable dt_product_field_list = new DataTable();
                    dt_product_field_list.Columns.Add("product_field_id", typeof(int));
                    dt_product_field_list.Columns.Add("product_id", typeof(int));
                    dt_product_field_list.Columns.Add("field_name", typeof(string));
                    dt_product_field_list.Columns.Add("print_field_type_id", typeof(int));
                    dt_product_field_list.Columns.Add("X", typeof(decimal));
                    dt_product_field_list.Columns.Add("Y", typeof(decimal));
                    dt_product_field_list.Columns.Add("width", typeof(decimal));
                    dt_product_field_list.Columns.Add("height", typeof(decimal));
                    dt_product_field_list.Columns.Add("font", typeof(string));
                    dt_product_field_list.Columns.Add("font_size", typeof(int));
                    dt_product_field_list.Columns.Add("mapped_name", typeof(string));
                    dt_product_field_list.Columns.Add("editable", typeof(bool));
                    dt_product_field_list.Columns.Add("deleted", typeof(bool));
                    dt_product_field_list.Columns.Add("label", typeof(string));
                    dt_product_field_list.Columns.Add("max_length", typeof(int));
                    dt_product_field_list.Columns.Add("printable", typeof(bool));
                    dt_product_field_list.Columns.Add("printside", typeof(int));
                    DataRow workRow;

                    foreach (var item in productPrintFields)
                    {
                        workRow = dt_product_field_list.NewRow();
                        workRow["product_field_id"] = item.product_field_id;
                        workRow["product_id"] = item.product_id;
                        workRow["print_field_type_id"] = item.print_field_type_id;
                        workRow["field_name"] = item.field_name;
                        workRow["mapped_name"] = item.mapped_name;
                        workRow["X"] = item.X;
                        workRow["Y"] = item.Y;
                        workRow["width"] = item.width;
                        workRow["height"] = item.height;
                        workRow["font"] = item.font;
                        workRow["font_size"] = item.font_size;
                        workRow["deleted"] = item.deleted;
                        workRow["editable"] = item.editable;
                        workRow["label"] = item.label;
                        workRow["max_length"] = item.max_length;
                        workRow["printable"] = item.printable;
                        workRow["printside"] = item.printside;
                        dt_product_field_list.Rows.Add(workRow);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_update_product_print_fields]";



                    command.Parameters.Add("@product_id", SqlDbType.BigInt).Value = productPrintFields[0].product_id;
                    command.Parameters.AddWithValue("@product_fields_list", dt_product_field_list);
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    resultCode = int.Parse(command.Parameters["@ResultCode"].Value.ToString());
                }
            }

            return (SystemResponseCode)resultCode;
        }

        public SystemResponseCode UpdateProductPrintFields(List<ProductPrintFieldResult> productPrintFields, long auditUserId, string auditWorkstation)
        {
            int resultCode;

            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    DataTable dt_product_field_list = new DataTable();
                    dt_product_field_list.Columns.Add("product_field_id", typeof(int));
                    dt_product_field_list.Columns.Add("product_id", typeof(int));
                    dt_product_field_list.Columns.Add("field_name", typeof(string));
                    dt_product_field_list.Columns.Add("print_field_type_id", typeof(int));
                    dt_product_field_list.Columns.Add("X", typeof(decimal));
                    dt_product_field_list.Columns.Add("Y", typeof(decimal));
                    dt_product_field_list.Columns.Add("width", typeof(decimal));
                    dt_product_field_list.Columns.Add("height", typeof(decimal));
                    dt_product_field_list.Columns.Add("font", typeof(string));
                    dt_product_field_list.Columns.Add("font_size", typeof(int));
                    dt_product_field_list.Columns.Add("mapped_name", typeof(string));
                    dt_product_field_list.Columns.Add("editable", typeof(bool));
                    dt_product_field_list.Columns.Add("deleted", typeof(bool));
                    dt_product_field_list.Columns.Add("label", typeof(string));
                    dt_product_field_list.Columns.Add("max_length", typeof(int));
                    dt_product_field_list.Columns.Add("printable", typeof(bool));
                    dt_product_field_list.Columns.Add("printside", typeof(int));

                    DataRow workRow;

                    foreach (var item in productPrintFields)
                    {
                        workRow = dt_product_field_list.NewRow();
                        workRow["product_id"] = item.product_id;
                        workRow["product_field_id"] = item.product_field_id;
                        workRow["print_field_type_id"] = item.print_field_type_id;
                        workRow["field_name"] = item.field_name;
                        workRow["mapped_name"] = item.mapped_name;
                        workRow["X"] = item.X;
                        workRow["Y"] = item.Y;
                        workRow["width"] = item.width;
                        workRow["height"] = item.height;
                        workRow["font"] = item.font;
                        workRow["font_size"] = item.font_size;
                        workRow["editable"] = item.editable;
                        workRow["deleted"] = item.deleted;
                        workRow["label"] = item.label;
                        workRow["max_length"] = item.max_length;
                        workRow["printable"] = item.printable;
                        workRow["printside"] = item.printside;
                        dt_product_field_list.Rows.Add(workRow);
                    }

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_update_product_print_fields]";

                    command.Parameters.Add("@product_id", SqlDbType.BigInt).Value = productPrintFields[0].product_id;
                    command.Parameters.AddWithValue("@product_fields_list", dt_product_field_list);
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = auditUserId;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditWorkstation;
                    command.Parameters.Add("@ResultCode", SqlDbType.Int).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    resultCode = int.Parse(command.Parameters["@ResultCode"].Value.ToString());
                }
            }

            return (SystemResponseCode)resultCode;
        }

        public List<ProductAccountTypeMapping> GetProductAccountTypeMappings(int? productId, string cbsAccountType, long audit_userId, string audit_workstation)
        {
            List<ProductAccountTypeMapping> result = new List<ProductAccountTypeMapping>();

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var mappings = context.usp_get_product_accounttypes_mappings(productId, cbsAccountType, audit_userId, audit_workstation);

                foreach (var mapping in mappings)
                {
                    result.Add(new ProductAccountTypeMapping()
                    {
                        CbsAccountType = mapping.cbs_account_type,
                        CmsAccountType = mapping.cms_account_type,
                        IndigoAccountTypeId = int.Parse(mapping.indigo_account_type),
                        ProductId = (int)productId
                    });
                }
            }

            return result;
        }
        public List<ProductPrintFieldResult> GetProductPrintFields(long? productId, long? cardId, long? requestId)
        {
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                if (productId != null)
                {
                    var result = context.usp_get_product_print_fields(productId);
                    return result.ToList();
                }
                else if (cardId != null)
                {
                    var result = context.usp_get_product_print_fields_value(cardId);
                    return result.ToList();
                }
                else if (requestId != null)
                {
                    var result = context.usp_get_product_print_fields_value_byrequest(requestId);
                    return result.ToList();
                }

                return null;
            }
        }
        //public List<ProductField> GetPrintFieldsByProductid(int? cardId)
        //{
        //    using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
        //    {
        //        var result = context.usp_get_printfields_byproductid(cardId);
        //        return result.ToList();
        //    }
        //}

        public void UpdatePrintFieldValue(IProductPrintField[] printfields, long customeraccountid, long audituserid, string auditworkstation)
        {
            using (SqlConnection con = _dbObject.SQLConnection)
            {
                using (SqlCommand command = con.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[usp_update_printfieldvalues]";
                    command.Parameters.AddWithValue("@product_fields", UtilityClass.CreateKeyValueTable2<byte[]>(printfields.ToDictionary(k => k.ProductPrintFieldId, v => ((ProductField)v).Value)));
                    command.Parameters.Add("@customeraccountid", SqlDbType.BigInt).Value = customeraccountid;
                    command.Parameters.Add("@audit_user_id", SqlDbType.BigInt).Value = audituserid;
                    command.Parameters.Add("@audit_workstation", SqlDbType.VarChar).Value = auditworkstation;

                    command.ExecuteNonQuery();
                }

            }


        }
        /// <summary>
        /// to get pin block formats
        /// </summary>
        /// <param name="auditUserId"></param>
        /// <param name="auditWorkstation"></param>
        /// <returns></returns>
        public List<pin_block_formatResult> LookupPinBlockFormat(long auditUserId, string auditWorkstation)
        {
            //List<ProductInterfacesResult> result = new List<ProductInterfacesResult>();
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_get_pin_block_format(auditUserId, auditWorkstation);
                return result.ToList();
            }
        }

        public List<LangLookup> LookupPrintFieldTypes(int languageId, long auditUserId, string auditWorkstation)
        {
            //List<ProductInterfacesResult> result = new List<ProductInterfacesResult>();
            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                var result = context.usp_lang_lookup_print_field_types(languageId, auditUserId, auditWorkstation);
                return result.ToList();
            }
        }

        public List<BillingReportResult> GetBillingReport(int? IssuerId, string month, string year, long auditUserId, string auditWorkstation)
        {

            using (var context = new indigo_databaseEntities(_dbObject.EFSQLConnectionString))
            {
                ObjectResult<BillingReportResult> results = context.usp_get_billing_report(int.Parse(year), int.Parse(month));


                return results.ToList();
            }
        }
        #endregion
    }
}
