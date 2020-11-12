using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndigoCardIssuanceService.bll.Logic;
using NSubstitute;
using FluentAssertions;
using Veneka.Indigo.CardManagement;
using Veneka.Indigo.COMS.Core;
using Veneka.Indigo.Integration;

namespace IndigoCardIssuanceService.bll.Logic.Tests
{
    /// <summary>
    /// Summary description for AccountLookupLogicTests
    /// </summary>
    [TestClass]
    public class AccountLookupLogicTests
    {
        [TestMethod]
        public void ValidateAccount_SuccessfulValidation()
        {
            // Arrange
            var cardManagement = Substitute.For<CardMangementService>();
            cardManagement.GetProductCurrencies(1, 1, true, 10, "TEST-WORKSTATION")
                .ReturnsForAnyArgs(new List<Veneka.Indigo.Common.Models.ProductCurrencyResult>() { new Veneka.Indigo.Common.Models.ProductCurrencyResult() });
            cardManagement.GetProductAccountTypes(1, 0, 10, "TEST-WORKSTATION")
                .ReturnsForAnyArgs(new List<Veneka.Indigo.Common.Models.ProductAccountTypesResult>() { new Veneka.Indigo.Common.Models.ProductAccountTypesResult() });

            var comsCore = Substitute.For<IComsCore>();
            var integration = Substitute.For<IIntegrationController>();

            AccountLookupLogic accLogic = new AccountLookupLogic(cardManagement, comsCore, integration);

            var accDetails = new Veneka.Indigo.Integration.Objects.AccountDetails
            {
                CurrencyId = 2,
                AccountTypeId = 3
            };

            // Act
            string respString;
            var result = accLogic.ValidateAccount(1, new Veneka.Indigo.Integration.Objects.AccountDetails(), out respString);

            // Assert
            result.Should().BeTrue(because: "All validations passed for account details");
            respString.Should().BeEmpty(because: "Validation passed an we expect and empty string back. response message used for validation issues.");
        }

        [TestMethod]
        public void ValidateAccount_IgnoredPerformingValidationForCurrency_WhenCurrencyIdSetToNegativeOne()
        {
            // Arrange
            var cardManagement = Substitute.For<CardMangementService>();            
            cardManagement.GetProductAccountTypes(1, 0, 10, "TEST-WORKSTATION")
                .ReturnsForAnyArgs(new List<Veneka.Indigo.Common.Models.ProductAccountTypesResult>() { new Veneka.Indigo.Common.Models.ProductAccountTypesResult() });

            var comsCore = Substitute.For<IComsCore>();
            var integration = Substitute.For<IIntegrationController>();

            AccountLookupLogic accLogic = new AccountLookupLogic(cardManagement, comsCore, integration);

            var accDetails = new Veneka.Indigo.Integration.Objects.AccountDetails
            {
                CurrencyId = -1,
                AccountTypeId = 3
            };

            // Act
            string respString;
            var result = accLogic.ValidateAccount(1, new Veneka.Indigo.Integration.Objects.AccountDetails(), out respString);

            // Assert
            result.Should().BeTrue(because: "All validations passed for account details");
            respString.Should().BeEmpty(because: "Validation passed and we expect an empty string back. response message used for validation issues.");
        }
    }
}
