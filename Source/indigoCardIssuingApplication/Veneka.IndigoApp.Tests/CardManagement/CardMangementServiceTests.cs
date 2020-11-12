using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.QualityTools.Testing.Fakes;
using NSubstitute;
using FluentAssertions;
using Veneka.Indigo.CardManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.CardManagement.objects;
using Veneka.Indigo.CardManagement.dal;
using Veneka.Indigo.Common.Language;

namespace Veneka.Indigo.CardManagement.Tests
{
    [TestClass()]
    public class CardMangementServiceTests
    {
        #region Product Account Type Mapping Tests
        [TestMethod()]
        public void TryAccountTypeMappingForAProduct_ReturnsTrueWithSinglePopulatedAccountTypeMapping()
        {
            // Arrange
            var accountTypeMappings = new ProductAccountTypeMapping { CbsAccountType = "001", CmsAccountType = "201", IndigoAccountTypeId = 0, ProductId = 0 };

            var cardManagementDal = Substitute.For<ICardManagementDAL>();           

            cardManagementDal.GetProductAccountTypeMappings(0, "001", 1, "Test-PC")
                .Returns(callInfo =>
                {
                    return new List<ProductAccountTypeMapping>() { accountTypeMappings };
                });

            CardMangementService cardManagementService = new CardMangementService(null, cardManagementDal,null);

            // Act
            ProductAccountTypeMapping returned;
            var result = cardManagementService.TryGetProductAccountTypeMapping(0, "001", 0, 1, "Test-PC", out returned);
                        
            // Assert
            cardManagementDal.Received().GetProductAccountTypeMappings(0, "001", 1, "Test-PC"); // Check the correct arguments received
            
            result.Should().BeTrue(because: "A single mapping was found for the product");
            returned.Should().NotBeNull(because: "An account type mapping was found and must be returned");
            returned.Should().BeEquivalentTo(accountTypeMappings, because: "Whats returned from the DAL should not be altered");
        }

        /// <summary>
        /// Checks that an exception is thrown if the database returns more than a single row.
        /// </summary>
        [TestMethod()]
        public void TryGetAccountTypeMappingForAProduct_ExceptionThrown_MultipleMappingsReturned()
        {
            // Arrange
            var accountTypeMappings = new List<ProductAccountTypeMapping>()
            {
                new ProductAccountTypeMapping() { CbsAccountType = "001", CmsAccountType = "201", IndigoAccountTypeId = 0, ProductId = 0 },
                new ProductAccountTypeMapping() { CbsAccountType = "001", CmsAccountType = "201", IndigoAccountTypeId = 0, ProductId = 0 },
                new ProductAccountTypeMapping() { CbsAccountType = "001", CmsAccountType = "201", IndigoAccountTypeId = 0, ProductId = 0 }
            };

            var cardManagementDal = Substitute.For<ICardManagementDAL>();
            var translator = Substitute.For<IResponseTranslator>();

            cardManagementDal.GetProductAccountTypeMappings(0, "001", 1, "Test-PC")
                .Returns(callInfo =>
                {
                    return accountTypeMappings;
                });

            
            CardMangementService cardManagementService = new CardMangementService(null, cardManagementDal, translator);

            // Act
            Exception exceptionReceived = null;
            ProductAccountTypeMapping returned = null;
            try
            {
                var result = cardManagementService.TryGetProductAccountTypeMapping(0, "001", 0, 1, "Test-PC", out returned);
            }
            catch(Exception ex)
            {
                exceptionReceived = ex;
            }

            // Assert
            cardManagementDal.Received().GetProductAccountTypeMappings(0, "001", 1, "Test-PC"); // Check the correct arguments received

            exceptionReceived.Should().NotBeNull(because: "Method should have thrown an exception as more than one mapping was returned for the product");
            exceptionReceived.Should().BeOfType(typeof(Exception), because: "Method throws generic exception when it detects more than one mapping returned");
            exceptionReceived.Message.Should().Be("More than one account type mapping has bee returned. Expected only one result.", because: "This is the exception message that should be returned");
            returned.Should().BeNull(because: "No returned value should have been set");
        }

        [TestMethod()]
        public void TryAccountTypeMappingForAProduct_NoAccountTypeMappingFoundForProduct()
        {
            // Arrange
            var accountTypeMappings = new List<ProductAccountTypeMapping>();

            var cardManagementDal = Substitute.For<ICardManagementDAL>();
            var translator = Substitute.For<IResponseTranslator>();
            cardManagementDal.GetProductAccountTypeMappings(0, "001", 1, "Test-PC")
                .Returns(callInfo =>
                {
                    return accountTypeMappings;
                });

            CardMangementService cardManagementService = new CardMangementService(null, cardManagementDal, translator);

            // Act            
            ProductAccountTypeMapping returned = null;
            var result = cardManagementService.TryGetProductAccountTypeMapping(0, "001", 0, 1, "Test-PC", out returned);


            // Assert
            cardManagementDal.Received().GetProductAccountTypeMappings(0, "001", 1, "Test-PC"); // Check the correct arguments received

            result.Should().BeFalse(because: "Must be false as no account mapping returned from database");
            returned.Should().BeNull(because: "Null value returned as no value found in database");
        }

        [TestMethod()]
        public void TryAccountTypeMappingForAProduct_NullReturnedFromDal()
        {
            // Arrange
            var cardManagementDal = Substitute.For<ICardManagementDAL>();

            cardManagementDal.GetProductAccountTypeMappings(0, "001", 1, "Test-PC")
                .Returns(callInfo =>
                {
                    return null;
                });
            var translator = Substitute.For<IResponseTranslator>();

            CardMangementService cardManagementService = new CardMangementService(null, cardManagementDal, translator);

            // Act            
            ProductAccountTypeMapping returned = null;
            var result = cardManagementService.TryGetProductAccountTypeMapping(0, "001", 0, 1, "Test-PC", out returned);


            // Assert
            cardManagementDal.Received().GetProductAccountTypeMappings(0, "001", 1, "Test-PC"); // Check the correct arguments received

            result.Should().BeFalse(because: "Must be false as no account mapping returned from database");
            returned.Should().BeNull(because: "Null value returned as no value found in database");
        }

        [TestMethod()]
        public void TryAccountTypeMappingForAProduct_UnhandledExceptionThrownInDAL()
        {
            // Arrange
            var cardManagementDal = Substitute.For<ICardManagementDAL>();

            cardManagementDal
                .When(x => x.GetProductAccountTypeMappings(0, "001", 1, "Test-PC"))
                .Do(x => { throw new Exception(); });

            var translator = Substitute.For<IResponseTranslator>();
            CardMangementService cardManagementService = new CardMangementService(null, cardManagementDal, translator);

            // Act            
            Exception thrownException = null;
            ProductAccountTypeMapping returned = null;
            bool? result = null;
            try
            {
                result = cardManagementService.TryGetProductAccountTypeMapping(0, "001", 0, 1, "Test-PC", out returned);
            }
            catch(Exception ex)
            {
                thrownException = ex;
            }


            // Assert
            cardManagementDal.Received().GetProductAccountTypeMappings(0, "001", 1, "Test-PC"); // Check the correct arguments received

            thrownException.Should().NotBeNull(because: "Unhandled exception should not have been caught in method");
            result.Should().BeNull();
            returned.Should().BeNull();
        }
        #endregion

        #region Product Print Field Tests
        // Not sure how useful these tests are. Its a search method so could return anything back

        [TestMethod()]
        public void GetProductPrintFields_DifferentSearchParameters_ReturnsNoResults()
        {
            // Arrange
            List<ProductPrintFieldResult> productPrintFields = new List<ProductPrintFieldResult>();
            var cardManagementDal = Substitute.For<ICardManagementDAL>();

            cardManagementDal.GetProductPrintFields(null, null, null).ReturnsForAnyArgs(productPrintFields);
            var translator = Substitute.For<IResponseTranslator>();
            CardMangementService cardManagementService = new CardMangementService(null, cardManagementDal, translator);

            // Act            
            var result1 = cardManagementService.GetProductPrintFields(null, null, null);
            var result2 = cardManagementService.GetProductPrintFields(null, 1, null);
            var result3 = cardManagementService.GetProductPrintFields(null, 1, 1);
            var result4 = cardManagementService.GetProductPrintFields(1, null, 1);

            // Assert
            result1.Should().BeEquivalentTo(productPrintFields, because: "DAL returned no results");
            result2.Should().BeEquivalentTo(productPrintFields, because: "DAL returned no results");
            result3.Should().BeEquivalentTo(productPrintFields, because: "DAL returned no results");
            result4.Should().BeEquivalentTo(productPrintFields, because: "DAL returned no results");
        }

        [TestMethod()]
        public void GetProductPrintFields_ReturnsSingleRecord()
        {
            // Arrange
            List<ProductPrintFieldResult> productPrintFields = new List<ProductPrintFieldResult>()
            {
                new ProductPrintFieldResult() { deleted = false, editable = true, field_name = "Field Name", font = "Font", font_size = 10, height = 200,
                    label = "Label", mapped_name = "Mapped Name", max_length = 15, print_field_name = "Print Field Name", print_field_type_id = 0,
                    product_field_id = 2, product_id = 10, value = new byte[3] { 0, 1, 2 }, width = 563, X = 700, Y = 1200
                }
            };

            var cardManagementDal = Substitute.For<ICardManagementDAL>();
            cardManagementDal.GetProductPrintFields(1, 2, null).Returns(productPrintFields);

            var translator = Substitute.For<IResponseTranslator>();
            CardMangementService cardManagementService = new CardMangementService(null, cardManagementDal, translator);

            // Act            
            var result = cardManagementService.GetProductPrintFields(1, 2, null);


            // Assert
            cardManagementDal.Received().GetProductPrintFields(1, 2, null); // Check the correct arguments received

            result.Should().NotBeNull(because: "The DAL returned a single record");
            result.Should().HaveCount(productPrintFields.Count, because: "The same number of records that the DAL returned should be returned from this method.");
        }

        [TestMethod()]
        public void GetProductPrintFields_ReturnsMultipleRecords()
        {
            // Arrange
            List<ProductPrintFieldResult> productPrintFields = new List<ProductPrintFieldResult>()
            {
                new ProductPrintFieldResult() { deleted = false, editable = true, field_name = "Field Name", font = "Font", font_size = 10, height = 200,
                    label = "Label", mapped_name = "Mapped Name", max_length = 15, print_field_name = "Print Field Name", print_field_type_id = 0,
                    product_field_id = 2, product_id = 10, value = new byte[3] { 0, 1, 2 }, width = 563, X = 700, Y = 1200
                },
                new ProductPrintFieldResult() { deleted = true, editable = false, field_name = "Field Name2", font = "Font2", font_size = 102, height = 2002,
                    label = "Label2", mapped_name = "Mapped Name2", max_length = 152, print_field_name = "Print Field Name2", print_field_type_id = 1,
                    product_field_id = 22, product_id = 102, value = new byte[3] { 20, 21, 22 }, width = 2563, X = 2700, Y = 21200
                },
                new ProductPrintFieldResult() { deleted = false, editable = false, field_name = "Field Name3", font = "Font3", font_size = 103, height = 2003,
                    label = "Label3", mapped_name = "Mapped Name3", max_length = 153, print_field_name = "Print Field Name3", print_field_type_id = 1,
                    product_field_id = 23, product_id = 103, value = new byte[3] { 30, 31, 32 }, width = 3563, X = 3700, Y = 31200
                }
            };

            var cardManagementDal = Substitute.For<ICardManagementDAL>();
            var translator = Substitute.For<IResponseTranslator>();
            cardManagementDal.GetProductPrintFields(2, 3, null).Returns(productPrintFields);

            CardMangementService cardManagementService = new CardMangementService(null, cardManagementDal, translator);

            // Act            
            var result = cardManagementService.GetProductPrintFields(2, 3, null);

            // Assert
            cardManagementDal.Received().GetProductPrintFields(2, 3, null); // Check the correct arguments received

            result.Should().NotBeNull(because: "The DAL returned multiple record");
            result.Should().HaveCount(productPrintFields.Count, because: "The same number of records that the DAL returned should be returned from this method.");
        }
        #endregion
    }
}