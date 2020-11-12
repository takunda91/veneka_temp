using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using FluentAssertions;
using Veneka.Indigo.CardManagement;
using Veneka.Indigo.COMS.Core;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Integration.ProductPrinting;
using Veneka.Indigo.CardManagement.objects;
using IndigoCardIssuanceService.bll;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.CardManagement.dal;
using Veneka.Indigo.Common.Language;
using Veneka.Indigo.Integration.Config;
using Veneka.Indigo.Integration.External;
using IndigoCardIssuanceService.bll.Logic;
using Veneka.Indigo.Common.Models;

namespace Veneka.IndigoApp.Tests.IndigoCardIssuanceService.bll.Logic
{
    /// <summary>
    /// Summary description for RequestCardLogicTests
    /// </summary>
    [TestClass]
    public class RequestCardLogicTests
    {

        [TestMethod]
        public void RequestCardForCustomer_ReturnZero()
        {
            // arrange
            var cardManagement = Substitute.For<ICardManagementDAL>();
            var comsCore = Substitute.For<IComsCore>();
            var integration = Substitute.For<IIntegrationController>();
            var datasource = Substitute.For<IDataSource>();

            var translator = Substitute.For<IResponseTranslator>();

            cardManagement.GetProductPrintFields(1, null, null)
                .ReturnsForAnyArgs(new List<Veneka.Indigo.Common.Models.ProductPrintFieldResult>()
                { new Veneka.Indigo.Common.Models.ProductPrintFieldResult(){product_id=1 } });

            cardManagement.GetProduct(1, 10, "TEST-WORKSTATION").ReturnsForAnyArgs(new ProductResult()
            {
                Product = new Indigo.Common.Models.ProductlistResult()
                { charge_fee_at_cardrequest = false, product_id = 1 }
            }
               );
            cardManagement.RequestCardForCustomer(0, 0, 1, 0, "", 1, null, null, "", "", "", "", 1, null, null, null, "", "", "", "test", "1", false, false, null, false, new List<IProductPrintField>(), 1, 10, "test", out long cardId)
                .ReturnsForAnyArgs(Indigo.Common.SystemResponseCode.CONFIGURATION_ERROR);

            IssueCardController issueCardController = new IssueCardController(datasource, cardManagement, integration, comsCore, translator);

            CustomerDetails customerDetails = new CustomerDetails() { ProductId = 1, AccountNumber = "78887888", ProductFields = new List<ProductField>() };

            //act
            var result = issueCardController.RequestCardForCustomer(customerDetails, 1, 10, "TEST-WORKSTATION");

            //assert
            result.Value.Should().BeLessOrEqualTo(0, because: "Request Card For Customer is returing null.");



        }

        [TestMethod]
        public void RequestCardForCustomer_ReturnOne_ForSucessful()
        {
            // arrange
            var cardManagement = Substitute.For<ICardManagementDAL>();
            var comsCore = Substitute.For<IComsCore>();
            var integration = Substitute.For<IIntegrationController>();
            var datasource = Substitute.For<IDataSource>();

            var translator = Substitute.For<IResponseTranslator>();

            cardManagement.GetProductPrintFields(1, null, null)
                .ReturnsForAnyArgs(new List<Veneka.Indigo.Common.Models.ProductPrintFieldResult>()
                { new Veneka.Indigo.Common.Models.ProductPrintFieldResult(){product_id=1 } });

            cardManagement.GetProduct(1, 10, "TEST-WORKSTATION").ReturnsForAnyArgs(new ProductResult()
            {
                Product = new Indigo.Common.Models.ProductlistResult()
                { charge_fee_at_cardrequest = false, product_id = 1 }
            }
               );
            long cardId;
            cardManagement.RequestCardForCustomer(0, 0, 1, 0, "", 1, null, null, "", "", "", "", 1, null, null, null, "", "", "", "test", "1", false, false, null, false, new List<IProductPrintField>(), 1, 10, "test", out cardId)
                .ReturnsForAnyArgs(Indigo.Common.SystemResponseCode.SUCCESS);
            cardId = 1;

            IssueCardController issueCardController = new IssueCardController(datasource, cardManagement, integration, comsCore, translator);

            CustomerDetails customerDetails = new CustomerDetails() { ProductId = 1, AccountNumber = "78887888", ProductFields = new List<ProductField>() };

            //act
            var result = issueCardController.RequestCardForCustomer(customerDetails, 1, 10, "TEST-WORKSTATION");

            //assert
            result.Value.Should().BeLessOrEqualTo(0, because: "Request Card For Customer is returing null.");



        }
        [TestMethod]
        public void RequestCardForCustomer_ChargeFee_ReturnTrue()
        {
            // arrange
            var cardManagement = Substitute.For<ICardManagementDAL>();

            cardManagement.GetProductPrintFields(1, null, null)
                .ReturnsForAnyArgs(new List<Veneka.Indigo.Common.Models.ProductPrintFieldResult>()
                { new Veneka.Indigo.Common.Models.ProductPrintFieldResult(){product_id=1 } });

            cardManagement.GetProduct(1, 10, "TEST-WORKSTATION").ReturnsForAnyArgs(new ProductResult()
            {
                Product = new Indigo.Common.Models.ProductlistResult()
                { charge_fee_at_cardrequest = true, product_id = 1 }
            }
               );
            long cardId;
            cardManagement.RequestCardForCustomer(0, 0, 1, 0, "", 1, null, null, "", "", "", "", 1, null, null, null, "", "", "", "test", "1", false, false, null, false, new List<IProductPrintField>(), 1, 10, "test", out cardId)
                .ReturnsForAnyArgs(Indigo.Common.SystemResponseCode.SUCCESS);
            cardId = 1;

            var cardMangementService = Substitute.For<CardMangementService>();
            cardManagement.GetProductCurrencies(1, 1, true)
                .ReturnsForAnyArgs(new List<Veneka.Indigo.Common.Models.ProductCurrencyResult>() { new Veneka.Indigo.Common.Models.ProductCurrencyResult() });
            cardManagement.GetProductAccountTypes(1, 0, 10, "TEST-WORKSTATION")
                .ReturnsForAnyArgs(new List<Veneka.Indigo.Common.Models.ProductAccountTypesResult>() { new Veneka.Indigo.Common.Models.ProductAccountTypesResult() });

            var comsCore = Substitute.For<IComsCore>();
            comsCore.GetAccountDetail(new CardObject(), 1, 1, 1, 1, new Indigo.Integration.External.ExternalSystemFields(), new InterfaceInfo(), new AuditInfo())
              .ReturnsForAnyArgs(new ComsResponse<AccountDetails>()
              {
                  ResponseCode = (int)ComsResponseCodes.SUCCESS,
                  Value = new Veneka.Indigo.Integration.Objects.AccountDetails
                  {
                      CurrencyId = 1,
                      AccountTypeId = 3
                  }
              });

            var integration = Substitute.For<IIntegrationController>();
            integration.When(x => x.CoreBankingSystem(
  1, Indigo.Common.InterfaceArea.ISSUING, out ExternalSystemFields _external, out IConfig config))
  .Do(x => x[3] = new WebServiceConfig(Guid.NewGuid(), Protocol.HTTP, "Indigo/path", 0, "path", 20000));

            var datasource = Substitute.For<IDataSource>();

            AccountLookupLogic accLogic = new AccountLookupLogic(cardMangementService, comsCore, integration);
            var translator = Substitute.For<IResponseTranslator>();

            // accLogic.ValidateAccount(1, new Veneka.Indigo.Integration.Objects.AccountDetails(), out respString).ReturnsForAnyArgs(true);




            IssueCardController issueCardController = new IssueCardController(datasource, cardManagement, integration, comsCore, translator);

            CustomerDetails customerDetails = new CustomerDetails() { ProductId = 1, AccountNumber = "78887888", ProductFields = new List<ProductField>(), IssuerId = 1, BranchId = 1, CardIssueReasonId = 1, CardIssueMethodId = 1 };

            //act
            var result = issueCardController.RequestCardForCustomer(customerDetails, 1, 10, "TEST-WORKSTATION");

            //assert
            result.Value.Should().BeLessOrEqualTo(1, because: "Request Card For Customer is returing null.");
        }

        [TestMethod]
        public void RequestCardForCustomer_ChargeFee_updatefeereference()
        {
            // arrange
            var cardManagement = Substitute.For<ICardManagementDAL>();

            cardManagement.GetProductPrintFields(1, null, null)
                .ReturnsForAnyArgs(new List<Veneka.Indigo.Common.Models.ProductPrintFieldResult>()
                { new Veneka.Indigo.Common.Models.ProductPrintFieldResult(){product_id=1 } });

            cardManagement.GetProduct(1, 10, "TEST-WORKSTATION").ReturnsForAnyArgs(new ProductResult()
            {
                Product = new Indigo.Common.Models.ProductlistResult()
                { charge_fee_at_cardrequest = true, product_id = 1 }
            }
               );
            long cardId;
            cardManagement.RequestCardForCustomer(0, 0, 1, 0, "", 1, null, null, "", "", "", "", 1, null, null, null, "", "", "", "test", "1", false, false, null, false, new List<IProductPrintField>(), 1, 10, "test", out cardId)
                .ReturnsForAnyArgs(Indigo.Common.SystemResponseCode.SUCCESS);
            cardId = 1;
            cardManagement.UpdateFeeChargeStatus(1, 1, "test", "12345", -1, "test");
            
            var cardMangementService = Substitute.For<CardMangementService>();
            cardManagement.GetProductCurrencies(1, 1, true)
                .ReturnsForAnyArgs(new List<Veneka.Indigo.Common.Models.ProductCurrencyResult>() { new Veneka.Indigo.Common.Models.ProductCurrencyResult() });
            cardManagement.GetProductAccountTypes(1, 0, 10, "TEST-WORKSTATION")
                .ReturnsForAnyArgs(new List<Veneka.Indigo.Common.Models.ProductAccountTypesResult>() { new Veneka.Indigo.Common.Models.ProductAccountTypesResult() });

            var comsCore = Substitute.For<IComsCore>();
            comsCore.GetAccountDetail(new CardObject(), 1, 1, 1, 1, new Indigo.Integration.External.ExternalSystemFields(), new InterfaceInfo(), new AuditInfo())
              .ReturnsForAnyArgs(new ComsResponse<AccountDetails>()
              {
                  ResponseCode = (int)ComsResponseCodes.SUCCESS,
                  Value = new Veneka.Indigo.Integration.Objects.AccountDetails
                  {
                      CurrencyId = 1,
                      AccountTypeId = 3
                  }
              });

            var integration = Substitute.For<IIntegrationController>();
            integration.When(x => x.CoreBankingSystem(
  1, Indigo.Common.InterfaceArea.ISSUING, out ExternalSystemFields _external, out IConfig config))
  .Do(x => x[3] = new WebServiceConfig(Guid.NewGuid(), Protocol.HTTP, "Indigo/path", 0, "path", 20000));

            var datasource = Substitute.For<IDataSource>();

            AccountLookupLogic accLogic = new AccountLookupLogic(cardMangementService, comsCore, integration);
            var translator = Substitute.For<IResponseTranslator>();

            // accLogic.ValidateAccount(1, new Veneka.Indigo.Integration.Objects.AccountDetails(), out respString).ReturnsForAnyArgs(true);




            IssueCardController issueCardController = new IssueCardController(datasource, cardManagement, integration, comsCore, translator);

            CustomerDetails customerDetails = new CustomerDetails() { ProductId = 1, AccountNumber = "78887888", ProductFields = new List<ProductField>(), IssuerId = 1, BranchId = 1, CardIssueReasonId = 1, CardIssueMethodId = 1 };

            //act
            var result = issueCardController.RequestCardForCustomer(customerDetails, 1, 10, "TEST-WORKSTATION");

            //assert
            result.Value.Should().BeLessOrEqualTo(1, because: "Request Card For Customer is returing null.");
        }

        [TestMethod]
        public void FeeChange_NOBalance()
        {
            // arrange
            var cardManagement = Substitute.For<ICardManagementDAL>();
            //var cardManagementService = Substitute.For<CardMangementService>();
            cardManagement.GetProductPrintFields(1, null, null)
    .ReturnsForAnyArgs(new List<Veneka.Indigo.Common.Models.ProductPrintFieldResult>()
    { new Veneka.Indigo.Common.Models.ProductPrintFieldResult(){product_id=1,deleted=false,editable=false,
        field_name ="test",font="Arial",font_size=12,height=10,label="test",mapped_name="test_1",max_length=10,print_field_name="test",
        print_field_type_id =1,product_field_id=1,value=System.Text.Encoding.Default.GetBytes("test"),width=10,X=1,Y=20} });
            var cardMangementService = new CardMangementService(Substitute.For<IDataSource>(), cardManagement, Substitute.For<IResponseTranslator>());

           // cardMangementService.GetProductPrintFields(1, null, null).ReturnsForAnyArgs(new List<PrintStringField>() {  });


            cardManagement.GetProduct(1, 10, "TEST-WORKSTATION").ReturnsForAnyArgs(new ProductResult()
            {
                Product = new Indigo.Common.Models.ProductlistResult()
                { charge_fee_at_cardrequest = true, product_id = 1 }
            }
               );

            CustomerDetails customerDetails = new CustomerDetails() { ProductId = 1, AccountNumber = "78887888", ProductFields = new List<ProductField>() };

            // cardManagement.GetProductPrintFields(1, null, null).ReturnsForAnyArgs(new List<ProductPrintFieldResult>());
            // 
            

            var comsCore = Substitute.For<IComsCore>();
            var integration = Substitute.For<IIntegrationController>();

            integration.When(x => x.CoreBankingSystem(
    1, Indigo.Common.InterfaceArea.ISSUING, out ExternalSystemFields _external, out IConfig config))
    .Do(x => x[3] = new WebServiceConfig(Guid.NewGuid(), Protocol.HTTP, "Indigo/path", 0, "path", 20000));
            
                
                
        
            comsCore.CheckBalance(customerDetails, new ExternalSystemFields(), new InterfaceInfo(), new AuditInfo()).ReturnsForAnyArgs(new ComsResponse<bool>() {ResponseCode=(int)ComsResponseCodes.WARNING,ResponseMessage="No Balance" });

            FeeChargeLogic feelogic = new FeeChargeLogic(cardMangementService, comsCore, integration);
            //Act
            var result = feelogic.FeeCharge(customerDetails, new AuditInfo());


            //Assert

            result.ResponseType.Should().BeEquivalentTo(1, because:"no balance in account");
        }

        [TestMethod]
        public void FeeChange_Balance_avaliable()
        {
            // arrange
            var cardManagement = Substitute.For<ICardManagementDAL>();
            //var cardManagementService = Substitute.For<CardMangementService>();
            cardManagement.GetProductPrintFields(1, null, null)
    .ReturnsForAnyArgs(new List<Veneka.Indigo.Common.Models.ProductPrintFieldResult>()
    { new Veneka.Indigo.Common.Models.ProductPrintFieldResult(){product_id=1,deleted=false,editable=false,
        field_name ="test",font="Arial",font_size=12,height=10,label="test",mapped_name="test_1",max_length=10,print_field_name="test",
        print_field_type_id =1,product_field_id=1,value=System.Text.Encoding.Default.GetBytes("test"),width=10,X=1,Y=20} });
            var cardMangementService = new CardMangementService(Substitute.For<IDataSource>(), cardManagement, Substitute.For<IResponseTranslator>());

            // cardMangementService.GetProductPrintFields(1, null, null).ReturnsForAnyArgs(new List<PrintStringField>() {  });


            cardManagement.GetProduct(1, 10, "TEST-WORKSTATION").ReturnsForAnyArgs(new ProductResult()
            {
                Product = new Indigo.Common.Models.ProductlistResult()
                { charge_fee_at_cardrequest = true, product_id = 1 }
            }
               );

            CustomerDetails customerDetails = new CustomerDetails() { ProductId = 1, AccountNumber = "78887888", ProductFields = new List<ProductField>() };

            // cardManagement.GetProductPrintFields(1, null, null).ReturnsForAnyArgs(new List<ProductPrintFieldResult>());
            // 


            var comsCore = Substitute.For<IComsCore>();
            var integration = Substitute.For<IIntegrationController>();

            integration.When(x => x.CoreBankingSystem(
    1, Indigo.Common.InterfaceArea.ISSUING, out ExternalSystemFields _external, out IConfig config))
    .Do(x => x[3] = new WebServiceConfig(Guid.NewGuid(), Protocol.HTTP, "Indigo/path", 0, "path", 20000));




            comsCore.CheckBalance(customerDetails, new ExternalSystemFields(), new InterfaceInfo(), new AuditInfo()).ReturnsForAnyArgs(new ComsResponse<bool>() { ResponseCode = (int)ComsResponseCodes.SUCCESS, ResponseMessage = "No Balance" });

            comsCore.ChargeFee(customerDetails, new ExternalSystemFields(), new InterfaceInfo(), new AuditInfo()).ReturnsForAnyArgs(new ComsResponse<bool>() { ResponseCode = (int)ComsResponseCodes.SUCCESS, ResponseMessage ="Fee Charged." });
            FeeChargeLogic feelogic = new FeeChargeLogic(cardMangementService, comsCore, integration);
            //Act
            var result = feelogic.FeeCharge(customerDetails, new AuditInfo());
            
            //Assert
            result.ResponseType.Should().BeEquivalentTo(0, because: "no balance in account");
        }
    }

    
}
