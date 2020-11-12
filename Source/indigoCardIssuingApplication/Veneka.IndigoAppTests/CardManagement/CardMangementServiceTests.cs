using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veneka.Indigo.CardManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.IndigoAppTests.CardManagement.DAL;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.CardManagement.objects;

namespace Veneka.Indigo.CardManagement.Tests
{
    [TestClass()]
    public class CardMangementServiceTests
    {
        #region Product Account Type Mapping Tests
        /// <summary>
        /// Positive test case
        /// </summary>
        [TestMethod()]
        public void TryGetProductAccountTypeMappingTest()
        {
            CardMangementService cardManagementService = new CardMangementService();

            ProductAccountTypeMapping mapping;
            var result = cardManagementService.TryGetProductAccountTypeMapping(AccountMappingsTestDAL.SingleRowForProductAndTypeTestData().Item1,
                                                                                AccountMappingsTestDAL.SingleRowForProductAndTypeTestData().Item2, 0, 0, "TEST", out mapping);

            Assert.IsTrue(result);
            Assert.AreEqual(AccountMappingsTestDAL.SingleRowForProductAndTypeTestData().Item3[0].cbs_account_type, mapping.CbsAccountType);
            Assert.AreEqual(AccountMappingsTestDAL.SingleRowForProductAndTypeTestData().Item3[0].cms_account_type, mapping.CmsAccountType);
            Assert.AreEqual(AccountMappingsTestDAL.SingleRowForProductAndTypeTestData().Item3[0].indigo_account_type, mapping.IndigoAccountTypeId);
        }

        /// <summary>
        /// Checks that an exception is thrown if the database returns more than a single row.
        /// </summary>
        [TestMethod()]
        public void TryGetProductAccountTypeMappingNegativeTest()
        {
            //SingleRowForProductAndTypeTestData()

            CardMangementService cardManagementService = new CardMangementService(new AccountMappingsTestDAL());

            try
            {
                accounttypes_mappings_Result mapping;
                var result = cardManagementService.TryGetProductAccountTypeMapping(AccountMappingsTestDAL.NegativeSingleRowForProductAndTypeTestData().Item1,
                                                                                    AccountMappingsTestDAL.NegativeSingleRowForProductAndTypeTestData().Item2, 0, 0, "TEST", out mapping);

                Assert.Fail("An expection should have been thrown because more than one row had been returned.");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("More than one account type mapping has bee returned. Expected only one result.", ex.Message);
            }
        }

        [TestMethod()]
        public void TryGetProductAccountTypeMappingNoresultTest()
        {
            //SingleRowForProductAndTypeTestData()

            CardMangementService cardManagementService = new CardMangementService(new AccountMappingsTestDAL());

            accounttypes_mappings_Result mapping;
            var result = cardManagementService.TryGetProductAccountTypeMapping(25,
                                                                                "aValue", 0, 0, "TEST", out mapping);

            Assert.IsFalse(result);
            Assert.IsNull(mapping);
        }
        #endregion

        #region Product Print Field Tests
        // Not sure how useful these tests are. Its a search method so could return anything back

        [TestMethod()]
        public void GetProductPrintFieldsEmptyTest()
        {
            CardMangementService cardManagementService = new CardMangementService(new ProductPrintFieldsEmptyTestDAL());

            var result1 = cardManagementService.GetProductPrintFields(null, null, null);
            var result2 = cardManagementService.GetProductPrintFields(1, null, null);
            var result3 = cardManagementService.GetProductPrintFields(null, 1, null);
            var result4 = cardManagementService.GetProductPrintFields(1, 1, 1);

            Assert.AreEqual(0, result1.Count);
            Assert.AreEqual(0, result2.Count);
            Assert.AreEqual(0, result3.Count);
            Assert.AreEqual(0, result4.Count);
        }

        [TestMethod()]
        public void GetProductPrintFieldsSingleTest()
        {
            CardMangementService cardManagementService = new CardMangementService(new ProductPrintFieldsSingleTestDAL());

            var result1 = cardManagementService.GetProductPrintFields(null, null,null);
            var result2 = cardManagementService.GetProductPrintFields(1, null,null);
            var result3 = cardManagementService.GetProductPrintFields(null, 1,null);
            var result4 = cardManagementService.GetProductPrintFields(1, 1,1);

            Assert.AreEqual(1, result1.Count);
            Assert.AreEqual(1, result2.Count);
            Assert.AreEqual(1, result3.Count);
            Assert.AreEqual(1, result4.Count);
        }

        [TestMethod()]
        public void GetProductPrintFieldsMultipleTest()
        {
            CardMangementService cardManagementService = new CardMangementService(new ProductPrintFieldsMultipleTestDAL());

            var result1 = cardManagementService.GetProductPrintFields(null, null,null);
            var result2 = cardManagementService.GetProductPrintFields(1, null,null);
            var result3 = cardManagementService.GetProductPrintFields(null, 1,null);
            var result4 = cardManagementService.GetProductPrintFields(1, 1,1);

            Assert.AreEqual(4, result1.Count);
            Assert.AreEqual(2, result2.Count);
            Assert.AreEqual(3, result3.Count);
            Assert.AreEqual(3, result4.Count);
        }
        #endregion
    }
}