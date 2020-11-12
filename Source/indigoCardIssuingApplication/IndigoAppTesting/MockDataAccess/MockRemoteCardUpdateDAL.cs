using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.Remote;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.RemoteManagement.DAL;
using Veneka.Indigo.Common.Models;

namespace IndigoAppTesting.MockDataAccess
{
    public class MockRemoteCardUpdateDAL : IRemoteCardUpdateDAL
    {
        public void ChangeRemoteCardsStatus(List<long> cardIds, int remoteUpdateStatusesId, string comment, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public RemoteCardUpdateDetailResult ChangeRemoteCardStatus(long cardId, int remoteUpdateStatusesId, string comment, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public RemoteCardUpdates GetPendingCardUpdates(int issuerId, string remoteComponentIP, long auditUserId, string auditWorkstation)
        {
            var rtn = new RemoteCardUpdates();
            rtn.Response.ResponseCode = 0;
            rtn.Response.ResponseMessage = "Success.";

            if (issuerId == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    rtn.Cards.Add(new CardDetail
                    {
                        account_type_id = 0,
                        branch_code = "001",
                        branch_name = "TestBranch001",
                        card_id = i,
                        card_issue_reason_id = 0,
                        card_number = "123456" + i.ToString("0000000000"),
                        card_priority_id = 1,
                        card_request_reference = "ABCD" + i.ToString("000000"),
                        cms_id = "CMSID" + i,
                        contact_number = "contact" + i,
                        contract_number = "contract" + 1,
                        country_code = "GH",
                        country_name = "GHANA",
                        currency_code = "GHS",
                        CustomerId = "customer" + i,
                        customer_account_number = i.ToString() + i.ToString("00000000"),
                        //customer_account_type_name = "SAVINGS",
                        customer_first_name = "Firstname" + i,
                        customer_last_name = "Lastname" + i,
                        customer_middle_name = "Middlename" + i,
                        //customer_residency_name = "Residency" + i,
                        customer_title_id = 0,
                        //customer_title_name = "MR",
                        customer_type_id = 0,
                        //customer_type_name = "CustomerTypeName",
                        date_issued = DateTime.Now,
                        //domicile_branch_code = "001",
                        //domicile_branch_name = "Accra",
                        id_number = "999" + i.ToString("0000000"),
                        iso_4217_numeric_code = "42",
                        issuer_code = "001",
                        issuer_name = "FirstBank",
                        name_on_card = "NameOnCard" + i,
                        product_bin_code = "123456",
                        product_code = "PRD001",
                        product_id = 1,
                        product_name = "Product One",
                        pvv = "pvv" + i,
                        resident_id = 0,
                        sub_product_code = ""
                    });
                }

                rtn.ProductSettings.Add(new Settings
                {
                    ConfigType = 0,
                    IntegrationGuid = MockIntegration.MockRemoteCMS.GUID.ToString(),
                    ProductId = 1,
                    WebServiceSettings = new WebServiceConfig(Guid.Parse(MockIntegration.MockRemoteCMS.GUID), Protocol.HTTP, "localhost", 1234, "localPath", null)
                });
            }

            return rtn;
        }

        public RemoteCardUpdateDetailResult GetRemoteCardDetail(long cardId, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public List<RemoteCardUpdateSearchResult> SearchRemoteCardUpdates(string pan, int? remoteUpdateStatusesId, int? issuerId, int? branchId, int? productId, DateTime? dateFrom, DateTime? dateTo, int pageIndex, int rowsPerPage, int languageId, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }

        public void SetCardUpdates(string remoteComponentAddress, List<CardDetailResponse> cardDetails, long auditUserId, string auditWorkstation)
        {
            throw new NotImplementedException();
        }
    }
}
