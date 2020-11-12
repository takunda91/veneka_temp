using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Veneka.Indigo.Common.Models;
using Veneka.Indigo.COMS.Core;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Integration.FileLoader.Objects;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.Integration.ProductPrinting;

namespace Veneka.Indigo.COMS.DataSource.COMS
{
    public class CallbackDataSource : IComsCallback
    {
        private LocalDataSource _dataSource = new LocalDataSource();
        
        #region ProductDAL
        Product IProductDAL.FindBestMatch(int? issuerId, string pan, bool onlyActiveRecords, long auditUserId, string auditWorkstation)
        {
            return _dataSource.ProductDAL.FindBestMatch(issuerId, pan, onlyActiveRecords, auditUserId, auditWorkstation);
        }

        Product IProductDAL.FindBestMatch(string pan, List<Product> products)
        {
            return _dataSource.ProductDAL.FindBestMatch(pan, products);
        }

        Product IProductDAL.GetProduct(int productId, bool onlyActiveRecords, long auditUserId, string auditWorkStation)
        {
            return _dataSource.ProductDAL.GetProduct(productId, onlyActiveRecords, auditUserId, auditWorkStation);
        }

        List<IProductPrintField> IProductDAL.GetProductPrintFieldsByCode(string productCode, long auditUserId, string auditWorkStation)
        {
            return _dataSource.ProductDAL.GetProductPrintFieldsByCode(productCode, auditUserId, auditWorkStation);
        }

        List<Product> IProductDAL.GetProducts(int? issuerId, string bin, bool onlyActiveRecords, long auditUserId, string auditWorkStation)
        {
            return _dataSource.ProductDAL.GetProducts(issuerId, bin, onlyActiveRecords, auditUserId, auditWorkStation);
        }

        List<Product> IProductDAL.GetProductsByCode(int? issuerId, string productCode, bool onlyActiveRecords, long auditUserId, string auditWorkStation)
        {
            return _dataSource.ProductDAL.GetProductsByCode(issuerId, productCode, onlyActiveRecords, auditUserId, auditWorkStation);
        }

        List<Product> IProductDAL.GetProductsForExport(int? issuerId, bool onlyActiveRecords, long auditUserId, string auditWorkStation)
        {
            return _dataSource.ProductDAL.GetProductsForExport(issuerId, onlyActiveRecords, auditUserId, auditWorkStation);
        }
        #endregion

        #region ExportBatchDAL
        List<CardObject> IExportBatchDAL.FetchCardObjectsForExportBatch(long exportBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _dataSource.ExportBatchDAL.FetchCardObjectsForExportBatch(exportBatchId, languageId, auditUserId, auditWorkstation);
        }

        Dictionary<long, string> IExportBatchDAL.FindExportBatches(int issuerId, int? productId, int exportBatchStatusesId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _dataSource.ExportBatchDAL.FindExportBatches(issuerId, productId, exportBatchStatusesId, languageId, auditUserId, auditWorkstation);
        }

        ExportBatchGeneration IExportBatchDAL.GenerateBatches(int? issuerId, long auditUserId, string auditWorkStation)
        {            
            return _dataSource.ExportBatchDAL.GenerateBatches(issuerId, auditUserId, auditWorkStation);
        }

        int IExportBatchDAL.StatusChangeExported(long exportBatchId, int languageId, long auditUserId, string auditWorkStation)
        {
            return _dataSource.ExportBatchDAL.StatusChangeExported(exportBatchId, languageId, auditUserId, auditWorkStation);
        }
        #endregion

        #region BranchDAL
        BranchLookup IBranchDAL.GetBranch(string branchCode, string branchName, int issuerId)
        {
            return _dataSource.BranchDAL.GetBranch(branchCode, branchName, issuerId);
        }

        BranchLookup IBranchDAL.GetBranchesForIssuer(int issuerId)
        {
            return _dataSource.BranchDAL.GetBranchesForIssuer(issuerId);
        }

        List<BranchLookup> IBranchDAL.GetCardCentreList(int issuerId)
        {
            return _dataSource.BranchDAL.GetCardCentreList(issuerId);
        }

        public List<BranchLookup> GetBranchesForIssuerByIssuerCode(string issuerCode)
        {
            return _dataSource.BranchDAL.GetBranchesForIssuerByIssuerCode(issuerCode);
        }

        #endregion

        #region CardsDAL
        CardObject ICardsDAL.GetCardObject(long cardId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _dataSource.CardsDAL.GetCardObject(cardId, languageId, auditUserId, auditWorkstation);
        }

        CardObject ICardsDAL.GetCardObjectFromExport(long exportBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _dataSource.CardsDAL.GetCardObjectFromExport(exportBatchId, languageId, auditUserId, auditWorkstation);
        }
        void ICardsDAL.UpdateCardFeeReferenceNumber(long cardId, string referenceNumber, long auditUserId, string auditWorkStation)
        {
            _dataSource.CardsDAL.UpdateCardFeeReferenceNumber(cardId, referenceNumber, auditUserId, auditWorkStation);
        }

        void ICardsDAL.UpdateCardFeeReversalReferenceNumber(long cardId, string referenceNumber, long auditUserId, string auditWorkStation)
        {
            _dataSource.CardsDAL.UpdateCardFeeReversalReferenceNumber(cardId, referenceNumber, auditUserId, auditWorkStation);
        }
        List<CardObject> ICardsDAL.GetCardsByAccNo(int productId, string accountNumber, long auditUserId, string auditWorkStation)
        {
           return  _dataSource.CardsDAL.GetCardsByAccNo(productId, accountNumber, auditUserId, auditWorkStation);
        }
        #endregion

        #region IssuerDAL
        Issuer IIssuerDAL.GetIssuer(int issuerId, long auditUserId, string auditWorkStation)
        {
            return _dataSource.IssuerDAL.GetIssuer(issuerId, auditUserId, auditWorkStation);
        }
        #endregion

        #region ParametersDAL
        Parameters IParametersDAL.GetParameterIssuerInterface(int issuerId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
            return _dataSource.ParametersDAL.GetParameterIssuerInterface(issuerId, interfaceTypeId, interfaceArea, interfaceGuid, auditUserId, auditWorkStation);
        }

        Parameters IParametersDAL.GetParameterProductInterface(int productId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
            return _dataSource.ParametersDAL.GetParameterProductInterface(productId, interfaceTypeId, interfaceArea, interfaceGuid, auditUserId, auditWorkStation);
        }

        List<Parameters> IParametersDAL.GetParametersIssuerInterface(int? issuerId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
            return _dataSource.ParametersDAL.GetParametersIssuerInterface(issuerId, interfaceTypeId, interfaceArea, interfaceGuid, auditUserId, auditWorkStation);
        }

        List<Parameters> IParametersDAL.GetParametersProductInterface(int? productId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
            return _dataSource.ParametersDAL.GetParametersProductInterface(productId, interfaceTypeId, interfaceArea, interfaceGuid, auditUserId, auditWorkStation);
        }
        #endregion

        #region TerminalDAL
        TerminalMK ITerminalDAL.GetTerminalMasterKey(string deviceId, long auditUserId, string auditWorkStation)
        {
            return _dataSource.TerminalDAL.GetTerminalMasterKey(deviceId, auditUserId, auditWorkStation);
        }

        ZoneMasterKey ITerminalDAL.GetZoneMasterKey(int issuerId, long auditUserId, string auditWorkStation)
        {
            return _dataSource.TerminalDAL.GetZoneMasterKey(issuerId, auditUserId, auditWorkStation);
        }
        #endregion

        #region LookupDAL
        string ILookupDAL.LookupBranchCode(int branchId)
        {
            return _dataSource.LookupDAL.LookupBranchCode(branchId);
        }

        string ILookupDAL.LookupBranchName(int branchId)
        {
            return _dataSource.LookupDAL.LookupBranchName(branchId);
        }

        string ILookupDAL.LookupCurrency(int currencyId)
        {
            return _dataSource.LookupDAL.LookupCurrency(currencyId);
        }

        int ILookupDAL.LookupCurrency(string ccy)
        {
            return _dataSource.LookupDAL.LookupCurrency(ccy);
        }

        string ILookupDAL.LookupCurrencyISONumericCode(int currencyId)
        {
            return _dataSource.LookupDAL.LookupCurrencyISONumericCode(currencyId);
        }

        string ILookupDAL.LookupEmpBranchCode(int branchId)
        {
            return _dataSource.LookupDAL.LookupEmpBranchCode(branchId);
        }
        #endregion

        #region TransactionSequenceDAL
        int ITransactionSequenceDAL.NextSequenceNumber(string sequenceName, ResetPeriod resetPeriod)
        {
            return _dataSource.TransactionSequenceDAL.NextSequenceNumber(sequenceName, resetPeriod);
        }

        long ITransactionSequenceDAL.NextSequenceNumberLong(string sequenceName, ResetPeriod resetPeriod)
        {
            return _dataSource.TransactionSequenceDAL.NextSequenceNumberLong(sequenceName, resetPeriod);
        }

        void ITransactionSequenceDAL.Dispose()
        {
            _dataSource.TransactionSequenceDAL.Dispose();
        }
        #endregion

        #region CardGeneratorDAL
        int ICardGeneratorDAL.GetLatestSequenceNumber(int productId, long auditUserId, string auditWorkStation)
        {
            return _dataSource.CardGeneratorDAL.GetLatestSequenceNumber(productId, auditUserId, auditWorkStation);
        }

        void ICardGeneratorDAL.UpdateCardsAndSequenceNumber(Dictionary<long, string> cards, Dictionary<int, int> products, long auditUserId, string auditWorkStation)
        {
            _dataSource.CardGeneratorDAL.UpdateCardsAndSequenceNumber(cards, products, auditUserId, auditWorkStation);
        }

        public List<Dictionary<string, string>> DataCall(string spname, Dictionary<string, string> inputparameters, Dictionary<string, string> outputparameters)
        {
            return _dataSource.CustomDataDAL.DataCall(spname,inputparameters ,outputparameters);

        }

        public List<Dictionary<string, string>> DataCall(string spname, Dictionary<string, string> inputparameters, Dictionary<string, DataTable> inputkeyvaluetable, Dictionary<string, string> outputparameters)
        {
            return _dataSource.CustomDataDAL.DataCall(spname, inputparameters,inputkeyvaluetable, outputparameters);

        }

        public CardLimitData GetCardLimitDataByContractNumber(string contractNumber)
        {
            return _dataSource.CardsDAL.GetCardLimitDataByContractNumber(contractNumber);
        }

        public CardObject GetCardByPan(string pan, int mbr, string referenceNumber, long auditUserId, string auditWorkstation)
        {
            return _dataSource.CardsDAL.GetCardByPan(pan, mbr, referenceNumber, auditUserId, auditWorkstation);
        }
        #endregion
    }
}