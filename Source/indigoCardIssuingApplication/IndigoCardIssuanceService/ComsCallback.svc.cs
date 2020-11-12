using IndigoCardIssuanceService.COMS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Veneka.Indigo.COMS.Core;
using Veneka.Indigo.COMS.Core.Indigo.DataContracts;
using Veneka.Indigo.COMS.Core.Terminal;
using Veneka.Indigo.COMS.DataSource.COMS;
using Veneka.Indigo.Integration;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Integration.FileLoader.Objects;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.Integration.ProductPrinting;

namespace IndigoCardIssuanceService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ComsCallback" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ComsCallback.svc or ComsCallback.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(Namespace = Constants.IndigoComsCallbackURL)]
    public class ComsCallback : IComsCallback
    {
        private CallbackDataSource _callBack = new CallbackDataSource();
        
        public List<Dictionary<string, string>> DataCall(string spname, Dictionary<string, string> inputparameters, Dictionary<string, string> outputparameters)
        {
            return ((IComsCallback)_callBack).DataCall(spname,inputparameters,outputparameters);

        }

        public void Dispose()
        {
            ((IComsCallback)_callBack).Dispose();
        }

        public List<CardObject> FetchCardObjectsForExportBatch(long exportBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            return ((IComsCallback)_callBack).FetchCardObjectsForExportBatch(exportBatchId, languageId, auditUserId, auditWorkstation);
        }
        public List<CardObject> GetCardsByAccNo(int productId, string accountNumber, long auditUserId, string auditWorkStation)
        {
            return ((IComsCallback)_callBack).GetCardsByAccNo(productId, accountNumber, auditUserId, auditWorkStation);
        }
        public Product FindBestMatch(int? issuerId, string pan, bool onlyActiveRecords, long auditUserId, string auditWorkstation)
        {
            return ((IComsCallback)_callBack).FindBestMatch(issuerId, pan, onlyActiveRecords, auditUserId, auditWorkstation);
        }

        public Product FindBestMatch(string pan, List<Product> products)
        {
            return ((IComsCallback)_callBack).FindBestMatch(pan, products);
        }

        public Dictionary<long, string> FindExportBatches(int issuerId, int? productId, int exportBatchStatusesId, int languageId, long auditUserId, string auditWorkstation)
        {
            return ((IComsCallback)_callBack).FindExportBatches(issuerId, productId, exportBatchStatusesId, languageId, auditUserId, auditWorkstation);
        }

        public ExportBatchGeneration GenerateBatches(int? issuerId, long auditUserId, string auditWorkStation)
        {
            return ((IComsCallback)_callBack).GenerateBatches(issuerId, auditUserId, auditWorkStation);
        }

        public BranchLookup GetBranch(string branchCode, string branchName, int issuerId)
        {
            return ((IComsCallback)_callBack).GetBranch(branchCode, branchName, issuerId);
        }

        public BranchLookup GetBranchesForIssuer(int issuerId)
        {
            return ((IComsCallback)_callBack).GetBranchesForIssuer(issuerId);
        }

        public List<BranchLookup> GetCardCentreList(int issuerId)
        {
            return ((IComsCallback)_callBack).GetCardCentreList(issuerId);
        }

        public CardObject GetCardObject(long cardId, int languageId, long auditUserId, string auditWorkstation)
        {
            return ((IComsCallback)_callBack).GetCardObject(cardId, languageId, auditUserId, auditWorkstation);
        }

        public CardObject GetCardObjectFromExport(long exportBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            return ((IComsCallback)_callBack).GetCardObjectFromExport(exportBatchId, languageId, auditUserId, auditWorkstation);
        }

        

        public Issuer GetIssuer(int issuerId, long auditUserId, string auditWorkStation)
        {
            return ((IComsCallback)_callBack).GetIssuer(issuerId, auditUserId, auditWorkStation);
        }

        public int GetLatestSequenceNumber(int productId, long auditUserId, string auditWorkStation)
        {
            return ((IComsCallback)_callBack).GetLatestSequenceNumber(productId, auditUserId, auditWorkStation);
        }

        public Parameters GetParameterIssuerInterface(int issuerId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
            return ((IComsCallback)_callBack).GetParameterIssuerInterface(issuerId, interfaceTypeId, interfaceArea, interfaceGuid, auditUserId, auditWorkStation);
        }

        public Parameters GetParameterProductInterface(int productId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
            return ((IComsCallback)_callBack).GetParameterProductInterface(productId, interfaceTypeId, interfaceArea, interfaceGuid, auditUserId, auditWorkStation);
        }

        public List<Parameters> GetParametersIssuerInterface(int? issuerId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
            return ((IComsCallback)_callBack).GetParametersIssuerInterface(issuerId, interfaceTypeId, interfaceArea, interfaceGuid, auditUserId, auditWorkStation);
        }

        public List<Parameters> GetParametersProductInterface(int? productId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
            return ((IComsCallback)_callBack).GetParametersProductInterface(productId, interfaceTypeId, interfaceArea, interfaceGuid, auditUserId, auditWorkStation);
        }

        public Product GetProduct(int productId, bool onlyActiveRecords, long auditUserId, string auditWorkStation)
        {
            return ((IComsCallback)_callBack).GetProduct(productId, onlyActiveRecords, auditUserId, auditWorkStation);
        }

        public List<IProductPrintField> GetProductPrintFieldsByCode(string productCode, long auditUserId, string auditWorkStation)
        {
            return ((IComsCallback)_callBack).GetProductPrintFieldsByCode(productCode, auditUserId, auditWorkStation);
        }

        public List<Product> GetProducts(int? issuerId, string bin, bool onlyActiveRecords, long auditUserId, string auditWorkStation)
        {
            return ((IComsCallback)_callBack).GetProducts(issuerId, bin, onlyActiveRecords, auditUserId, auditWorkStation);
        }

        public List<Product> GetProductsByCode(int? issuerId, string productCode, bool onlyActiveRecords, long auditUserId, string auditWorkStation)
        {
            return ((IComsCallback)_callBack).GetProductsByCode(issuerId, productCode, onlyActiveRecords, auditUserId, auditWorkStation);
        }

        public List<Product> GetProductsForExport(int? issuerId, bool onlyActiveRecords, long auditUserId, string auditWorkStation)
        {
            return ((IComsCallback)_callBack).GetProductsForExport(issuerId, onlyActiveRecords, auditUserId, auditWorkStation);
        }

        public TerminalMK GetTerminalMasterKey(string deviceId, long auditUserId, string auditWorkStation)
        {
            return ((IComsCallback)_callBack).GetTerminalMasterKey(deviceId, auditUserId, auditWorkStation);
        }

        public ZoneMasterKey GetZoneMasterKey(int issuerId, long auditUserId, string auditWorkStation)
        {
            return ((IComsCallback)_callBack).GetZoneMasterKey(issuerId, auditUserId, auditWorkStation);
        }

        public string LookupBranchCode(int branchId)
        {
            return ((IComsCallback)_callBack).LookupBranchCode(branchId);
        }

        public string LookupBranchName(int branchId)
        {
            return ((IComsCallback)_callBack).LookupBranchName(branchId);
        }

        public string LookupCurrency(int currencyId)
        {
            return ((IComsCallback)_callBack).LookupCurrency(currencyId);
        }

        public int LookupCurrency(string ccy)
        {
            return ((IComsCallback)_callBack).LookupCurrency(ccy);
        }

        public string LookupCurrencyISONumericCode(int currencyId)
        {
            return ((IComsCallback)_callBack).LookupCurrencyISONumericCode(currencyId);
        }

        public string LookupEmpBranchCode(int branchId)
        {
            return ((IComsCallback)_callBack).LookupEmpBranchCode(branchId);
        }

        public int NextSequenceNumber(string sequenceName, ResetPeriod resetPeriod)
        {
            return ((IComsCallback)_callBack).NextSequenceNumber(sequenceName, resetPeriod);
        }

        public long NextSequenceNumberLong(string sequenceName, ResetPeriod resetPeriod)
        {
            return ((IComsCallback)_callBack).NextSequenceNumberLong(sequenceName, resetPeriod);
        }

        public int StatusChangeExported(long exportBatchId, int languageId, long auditUserId, string auditWorkStation)
        {
            return ((IComsCallback)_callBack).StatusChangeExported(exportBatchId, languageId, auditUserId, auditWorkStation);
        }

        public void UpdateCardFeeReferenceNumber(long cardId, string referenceNumber, long auditUserId, string auditWorkStation)
        {
            ((IComsCallback)_callBack).UpdateCardFeeReferenceNumber(cardId, referenceNumber, auditUserId, auditWorkStation);
        }

        public void UpdateCardFeeReversalReferenceNumber(long cardId, string referenceNumber, long auditUserId, string auditWorkStation)
        {
            ((IComsCallback)_callBack).UpdateCardFeeReversalReferenceNumber(cardId, referenceNumber, auditUserId, auditWorkStation);
        }

        public void UpdateCardsAndSequenceNumber(Dictionary<long, string> cards, Dictionary<int, int> products, long auditUserId, string auditWorkStation)
        {
            ((IComsCallback)_callBack).UpdateCardsAndSequenceNumber(cards, products, auditUserId, auditWorkStation);
        }

        public List<Dictionary<string, string>> DataCall(string spname, Dictionary<string, string> inputparameters, Dictionary<string, DataTable> inputkeyvaluetable, Dictionary<string, string> outputparameters)
        {
          return  ((IComsCallback)_callBack).DataCall(spname, inputparameters, inputkeyvaluetable, outputparameters);

        }

        public List<BranchLookup> GetBranchesForIssuerByIssuerCode(string issuerCode)
        {
            return ((IComsCallback)_callBack).GetBranchesForIssuerByIssuerCode(issuerCode);
        }

        public CardLimitData GetCardLimitDataByContractNumber(string contractNumber)
        {
            return ((IComsCallback)_callBack).GetCardLimitDataByContractNumber(contractNumber);
        }

        public CardObject GetCardByPan(string pan, int mbr, string referenceNumber, long auditUserId, string auditWorkstation)
        {
            return ((IComsCallback)_callBack).GetCardByPan(pan, mbr, referenceNumber, auditUserId, auditWorkstation);
        }
    }
}
