using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndigoCardIssuanceService.bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NSubstitute;

using Veneka.Indigo.CardManagement.dal;
using Veneka.Indigo.Integration;
using Veneka.Indigo.CardManagement.objects;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.COMS.Core;
using Veneka.Indigo.Common.Language;

namespace IndigoCardIssuanceService.bll.Tests
{
    [TestClass()]
    public class IssueCardControllerTests
    {
        [TestMethod()]
        public void GetAccountDetail_ReturnsSuccessfulWithPopulatedAccountDetails_AllValidationsPassed()
        {
            // Arrange
            var expectedResp = new DataContracts.Response<Veneka.Indigo.Integration.Objects.AccountDetails>(
                                        new Veneka.Indigo.Integration.Objects.AccountDetails(), ResponseType.SUCCESSFUL, "", "");

            // Arrange - ICardManagementDAL
            var cardManagementDal = Substitute.For<ICardManagementDAL>();
            cardManagementDal.GetProductAccountTypeMappings(1, "001", 70, "TEST-WORKSTATION")
                .ReturnsForAnyArgs(x => { return new List<ProductAccountTypeMapping>()
                   { new ProductAccountTypeMapping() { CbsAccountType = "001", CmsAccountType="101", IndigoAccountTypeId=1, ProductId = 4 } };  });

            cardManagementDal.GetProductPrintFields(1, null, null)
                .ReturnsForAnyArgs(x => { return new List<ProductPrintFieldResult>(){ new ProductPrintFieldResult() }; });

            cardManagementDal.GetProductCurrencies(1, 1, true)
                .ReturnsForAnyArgs(x => { return new List<ProductCurrencyResult>() { new ProductCurrencyResult() }; });

            cardManagementDal.GetProductAccountTypes(1, 1, 70, "TEST-WORKSTATION")
                .ReturnsForAnyArgs(x => { return new List<ProductAccountTypesResult>() { new ProductAccountTypesResult() }; });

            // Arrange - IIntegrationController
            var integrationController = Substitute.For<IIntegrationController>();

            // Arrange - IComsCore
            var comsCore = Substitute.For<IComsCore>();

            var dataSource = Substitute.For<IDataSource>();

            var translator = Substitute.For<IResponseTranslator>();
            IssueCardController issueCardController = new IssueCardController(dataSource, cardManagementDal, integrationController, comsCore, translator);

            // Act
            var response = issueCardController.GetAccountDetail(1, 23, 2, 3, "acc#", 4, 5, "TEST-WORKSTATION");

            // Assert
            response.Should().BeEquivalentTo(expectedResp);


            //IssueCardController issueCardController = new IssueCardController(new AccountLookupData.AccountLookup_CardManagementDAL(), new AccountLookupData.AccountLookup_IntegrationController());

            //string accountNo = "account#";

            //var response = issueCardController.GetAccountDetail(1, AccountLookupData.AccountLookup_CardManagementDAL.ProductId(), 0, 0, accountNo, 0, 0, "");

            //// Check response
            //response.ResponseType.Should().Be(ResponseType.SUCCESSFUL, because: "ResponseType should be ResponseType.SUCCESSFUL");
            //response.ResponseException.Should().BeEmpty(because: "ResponseException should be empty string");
            //response.ResponseMessage.Should().BeEmpty(because: "ResponseMessage should be empty string");

            //var expectedAccountDetails = AccountLookupData.AccountLookup_IntegrationController.CBS.CreateAccountDetails(accountNo, AccountLookupData.AccountLookup_CardManagementDAL.PrintFieldsData());

            //// Excluding NameOnCard, AccountTypeId and CMSAccountTypeId as GetAccountDetails will populate these after lookup to CMS
            //response.Value.Should().BeEquivalentTo(expectedAccountDetails, options =>
            //        options.Excluding(o => o.NameOnCard)
            //               .Excluding(o => o.AccountTypeId)
            //               .Excluding(o => o.CMSAccountTypeId),
            //        because: "The values of both objects should be the same except for those which are specificaly excluded");

            //// Check the excluded fields are equal to what we expect them to be
            //response.Value.NameOnCard.Should().Be("FIRSTNAME M LASTNAME", because: "NameOnCard should be: FIRSTNAME M LASTNAME ");
            //response.Value.AccountTypeId.Should().Be(1, because:"Test implementation of our DAL's returns 1 as the AccountTypeId.");
            //response.Value.CMSAccountTypeId.Should().Be("10", because: "Test implementation of our DAL's returns 10 as the CMSAccountTypeId.");
        }

        [TestMethod()]
        public void GetAccountDetailNullTest()
        {
            //IssueCardController issueCardController = new IssueCardController(new NullAccountLookupData.AccountLookup_CardManagementDAL(), new NullAccountLookupData.AccountLookup_IntegrationController());

            //var response = issueCardController.GetAccountDetail(1, NullAccountLookupData.AccountLookup_CardManagementDAL.ProductId(), 0, 0, "account#", 0, 0, "");

            //// Check response
            //response.Value.Should().BeNull(because:"Return null objects for value when something has gone wrong");
            //response.ResponseType.Should().Be(ResponseType.ERROR, because: "ResponseType should be ResponseType.ERROR");
            //response.ResponseException.Should().Be("Core Banking Interface responded successfuly but AccountDetail is null.", because: "This should indicate that AccountDetail object is null.");
            //response.ResponseMessage.Should().Be("Error when processing request.", because: "Default response");
        }

        [TestMethod()]
        public void GetAccountDetailNullProductFieldsTest()
        {
            //IssueCardController issueCardController = new IssueCardController(new NullProductFieldsAccountLookupData.AccountLookup_CardManagementDAL(), 
            //                                                                  new NullProductFieldsAccountLookupData.AccountLookup_IntegrationController());

            //var response = issueCardController.GetAccountDetail(1, NullProductFieldsAccountLookupData.AccountLookup_CardManagementDAL.ProductId(), 0, 0, "account#", 0, 0, "");

            //// Check response
            //response.Value.Should().BeNull(because: "Return null objects for value when something has gone wrong");
            //response.ResponseType.Should().Be(ResponseType.ERROR, because: "ResponseType should be ResponseType.ERROR");
            //response.ResponseException.Should().Be("Integration layer has not returned all Product Print Fields.", because: "This should indicate that ProductPrintFields returned is null.");
            //response.ResponseMessage.Should().Be("Error when processing request.", because: "Default response");
        }

        [TestMethod()]
        public void GetAccountDetailBadProductFieldsTest()
        {
            //IssueCardController issueCardController = new IssueCardController(new BadProductFieldsAccountLookupData.AccountLookup_CardManagementDAL(),
            //                                                                  new BadProductFieldsAccountLookupData.AccountLookup_IntegrationController());

            //var response = issueCardController.GetAccountDetail(1, BadProductFieldsAccountLookupData.AccountLookup_CardManagementDAL.ProductId(), 0, 0, "account#", 0, 0, "");

            //// Check response
            //response.Value.Should().BeNull(because: "Return null objects for value when something has gone wrong");
            //response.ResponseType.Should().Be(ResponseType.ERROR, because: "ResponseType should be ResponseType.ERROR");
            //response.ResponseException.Should().Be("Integration layer has not returned all Product Print Fields.", because: "This should indicate that ProductPrintFields returned is null.");
            //response.ResponseMessage.Should().Be("Error when processing request.", because: "Default response");
        }

        [TestMethod()]
        public void GetAccountDetailAccounTypeNotMappedTest()
        {            
            //IssueCardController issueCardController = new IssueCardController(new AccTypeMapAccountLookupData.AccountLookup_CardManagementDAL(),
            //                                                                  new AccTypeMapAccountLookupData.AccountLookup_IntegrationController());

            //var response = issueCardController.GetAccountDetail(1, AccTypeMapAccountLookupData.AccountLookup_CardManagementDAL.ProductId(), 0, 0, "account#", 0, 0, "");

            //// Check response
            //response.Value.Should().BeNull(because: "Return null objects for value when something has gone wrong");
            //response.ResponseType.Should().Be(ResponseType.ERROR, because: "ResponseType should be ResponseType.ERROR");
            //response.ResponseException.Should().Be("No mapping found for product 1 and CBS Account Type 503", because: "This should indicate what product and account type could not be mapped.");
            //response.ResponseMessage.Should().Be("Error when processing request.", because: "Default response");
        }

        [TestMethod()]
        public void GetAccountDetailNameOnCardTest()
        {
            //IssueCardController issueCardController = new IssueCardController(new NameOnCardAccountLookupData.AccountLookup_CardManagementDAL(),
            //                                                                  new NameOnCardAccountLookupData.AccountLookup_IntegrationController());
            //string accountNo = "account#";

            //var response = issueCardController.GetAccountDetail(1, NameOnCardAccountLookupData.AccountLookup_CardManagementDAL.ProductId(), 0, 0, accountNo, 0, 0, "");

            //// Check response            
            //response.ResponseType.Should().Be(ResponseType.SUCCESSFUL, because: "ResponseType should be ResponseType.SUCCESSFUL");
            //response.ResponseException.Should().BeEmpty(because: "ResponseException should be empty on success");
            //response.ResponseMessage.Should().BeEmpty(because: "ResponseMessage should be empty on success");

            //var expectedAccountDetails = NameOnCardAccountLookupData.AccountLookup_IntegrationController.CBS.CreateAccountDetails(accountNo, NameOnCardAccountLookupData.AccountLookup_CardManagementDAL.PrintFieldsData());

            //// Excluding NameOnCard, AccountTypeId and CMSAccountTypeId as GetAccountDetails will populate these after lookup to CMS
            //response.Value.Should().BeEquivalentTo(expectedAccountDetails, options =>
            //        options.Excluding(o => o.NameOnCard)
            //               .Excluding(o => o.AccountTypeId)
            //               .Excluding(o => o.CMSAccountTypeId),
            //        because: "The values of both objects should be the same except for those which are specificaly excluded");

            //// Check the excluded fields are equal to what we expect them to be
            //response.Value.NameOnCard.Should().Be("NAMEONCARD", because: "NameOnCard should be: NameOnCard as the CBS integration returned this.");
            //response.Value.AccountTypeId.Should().Be(1, because: "Test implementation of our DAL's returns 1 as the AccountTypeId.");
            //response.Value.CMSAccountTypeId.Should().Be("10", because: "Test implementation of our DAL's returns 10 as the CMSAccountTypeId.");
        }

        [TestMethod()]
        public void GetAccountDetailFailedCBSTest()
        {
            //IssueCardController issueCardController = new IssueCardController(new FailedCBSAccountLookupData.AccountLookup_CardManagementDAL(),
            //                                                                  new FailedCBSAccountLookupData.AccountLookup_IntegrationController());
            //string accountNo = "account#";

            //var response = issueCardController.GetAccountDetail(1, FailedCBSAccountLookupData.AccountLookup_CardManagementDAL.ProductId(), 0, 0, accountNo, 0, 0, "");

            //// Check response            
            //response.ResponseType.Should().Be(ResponseType.UNSUCCESSFUL, because: "ResponseType should be ResponseType.UNSUCCESSFUL");
            //response.ResponseException.Should().BeEmpty(because: "ResponseException should be empty on unsuccessful");
            //response.ResponseMessage.Should().Be("Something unexpected happened", because: "ResponseMessage should show the response message from CBS");
            //response.Value.Should().BeNull(because: "The CBS lookup wasnt successful and should not be returning anything");
        }

        [TestMethod()]
        public void GetAccountDetailFailedCMSTest()
        {
            //IssueCardController issueCardController = new IssueCardController(new FailedCMSAccountLookupData.AccountLookup_CardManagementDAL(),
            //                                                                  new FailedCMSAccountLookupData.AccountLookup_IntegrationController());
            //string accountNo = "account#";

            //var response = issueCardController.GetAccountDetail(1, FailedCMSAccountLookupData.AccountLookup_CardManagementDAL.ProductId(), 0, 0, accountNo, 0, 0, "");

            //// Check response            
            //response.ResponseType.Should().Be(ResponseType.UNSUCCESSFUL, because: "ResponseType should be ResponseType.UNSUCCESSFUL");
            //response.ResponseException.Should().BeEmpty(because: "ResponseException should be empty on unsuccessful");
            //response.ResponseMessage.Should().Be("Something unexpected happened", because: "ResponseMessage should show the response message from CmS");            

            //var expectedAccountDetails = FailedCMSAccountLookupData.AccountLookup_IntegrationController.CBS.CreateAccountDetails(accountNo, FailedCMSAccountLookupData.AccountLookup_CardManagementDAL.PrintFieldsData());

            //// Excluding NameOnCard, AccountTypeId and CMSAccountTypeId as GetAccountDetails will populate these after lookup to CMS
            //response.Value.Should().BeEquivalentTo(expectedAccountDetails, options =>
            //        options.Excluding(o => o.NameOnCard)
            //               .Excluding(o => o.AccountTypeId)
            //               .Excluding(o => o.CMSAccountTypeId),
            //        because: "The values of both objects should be the same except for those which are specificaly excluded");

            //// Check the excluded fields are equal to what we expect them to be
            //response.Value.NameOnCard.Should().Be("FIRSTNAME M LASTNAME", because: "NameOnCard should be: FIRSTNAME M LASTNAME ");
            //response.Value.AccountTypeId.Should().Be(1, because: "Test implementation of our DAL's returns 1 as the AccountTypeId.");
            //response.Value.CMSAccountTypeId.Should().Be("10", because: "Test implementation of our DAL's returns 10 as the CMSAccountTypeId.");
        }

        [TestMethod()]
        public void GetAccountDetailCmsNullTest()
        {
            //IssueCardController issueCardController = new IssueCardController(new CmsNullAccountLookupData.AccountLookup_CardManagementDAL(),
            //                                                                  new CmsNullAccountLookupData.AccountLookup_IntegrationController());
            //string accountNo = "account#";

            //var response = issueCardController.GetAccountDetail(1, CmsNullAccountLookupData.AccountLookup_CardManagementDAL.ProductId(), 0, 0, accountNo, 0, 0, "");

            //// Check response
            //response.Value.Should().BeNull(because: "Return null objects for value when something has gone wrong");
            //response.ResponseType.Should().Be(ResponseType.ERROR, because: "ResponseType should be ResponseType.ERROR");
            //response.ResponseException.Should().Be("Card Management Interface responded successfuly but AccountDetail is null.", because: "This should indicate that AccountDetail object is null.");
            //response.ResponseMessage.Should().Be("Error when processing request.", because: "Default response");
        }

        [TestMethod()]
        public void GetAccountDetailCurrencymapTest()
        {
            //IssueCardController issueCardController = new IssueCardController(new CurrencyMapAccountLookupData.AccountLookup_CardManagementDAL(),
            //                                                                  new CurrencyMapAccountLookupData.AccountLookup_IntegrationController());
            //string accountNo = "account#";

            //var response = issueCardController.GetAccountDetail(1, CurrencyMapAccountLookupData.AccountLookup_CardManagementDAL.ProductId(), 0, 0, accountNo, 0, 0, "");
                        
            //// Check response           
            //response.ResponseType.Should().Be(ResponseType.UNSUCCESSFUL, because: "ResponseType should be ResponseType.ERROR");
            //response.ResponseException.Should().BeEmpty(because: "This should be empty as its not an exception and has failed gracefuly");
            //response.ResponseMessage.Should().Be("Product does not support returned currency.", because: "Default response");

            // I think this should be null? we shouldnt return the accountdetails if theres a problem with them?
            //response.Value.Should().BeNull(because: "Return null objects for value when something has gone wrong");
        }
    }
}