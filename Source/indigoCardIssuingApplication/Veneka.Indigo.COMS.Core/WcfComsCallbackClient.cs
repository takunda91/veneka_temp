using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.COMS.Core.Indigo.DataContracts;
using Veneka.Indigo.COMS.Core.Terminal;
using Veneka.Indigo.Integration.Common;
using Veneka.Indigo.Integration.FileLoader.Objects;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.COMS.Core
{
    class WcfComsCallbackClient : IComsCallback
    {
        private readonly IComsCallback _proxy;
        private readonly string urlPath = "NativeAPI.svc/soap";

        public WcfComsCallbackClient(Uri url)
        {
            Uri uri = null;
            if (Uri.TryCreate(url, urlPath, out uri))
            {
                WSHttpBinding binding = new WSHttpBinding();
                binding.Security.Mode = SecurityMode.Transport;

                EndpointAddress endpoint = new EndpointAddress(uri);

                ChannelFactory<IComsCallback> factory = new ChannelFactory<IComsCallback>("ComsCallback");
                _proxy = factory.CreateChannel();

                //IgnoreUntrustedSSL();
            }
            else
            {
                throw new ArgumentException("URL not in correct format: " + url);
            }
        }

        public List<Dictionary<string, string>> DataCall(string spname, Dictionary<string, string> inputparameters, Dictionary<string, string> outputparameters)
        {
            return _proxy.DataCall(spname, inputparameters,outputparameters);

        }

        public List<Dictionary<string, string>> DataCall(string spname, Dictionary<string, string> inputparameters, Dictionary<string, DataTable> inputkeyvaluetable, Dictionary<string, string> outputparameters)
        {
            return _proxy.DataCall(spname, inputparameters,inputkeyvaluetable, outputparameters);
        }

        public void Dispose()
        {
            _proxy.Dispose();
        }

        public List<CardObject> FetchCardObjectsForExportBatch(long exportBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _proxy.FetchCardObjectsForExportBatch(exportBatchId, languageId, auditUserId, auditWorkstation);
        }

        public Product FindBestMatch(int? issuerId, string pan, bool onlyActiveRecords, long auditUserId, string auditWorkstation)
        {
            return _proxy.FindBestMatch(issuerId, pan, onlyActiveRecords, auditUserId, auditWorkstation);
        }

        public Product FindBestMatch(string pan, List<Product> products)
        {
            return _proxy.FindBestMatch(pan, products);
        }

        public Dictionary<long, string> FindExportBatches(int issuerId, int? productId, int exportBatchStatusesId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _proxy.FindExportBatches(issuerId, productId, exportBatchStatusesId, languageId, auditUserId, auditWorkstation);
        }

        public ExportBatchGeneration GenerateBatches(int? issuerId, long auditUserId, string auditWorkStation)
        {
            return _proxy.GenerateBatches(issuerId, auditUserId, auditWorkStation);
        }

        public BranchLookup GetBranch(string branchCode, string branchName, int issuerId)
        {
            return _proxy.GetBranch(branchCode, branchName, issuerId);
        }

        public BranchLookup GetBranchesForIssuer(int issuerId)
        {
            return _proxy.GetBranchesForIssuer(issuerId);
        }

        public List<BranchLookup> GetBranchesForIssuerByIssuerCode(string issuerCode)
        {
            return _proxy.GetBranchesForIssuerByIssuerCode(issuerCode);
        }

        public CardObject GetCardByPan(string pan, int mbr, string referenceNumber, long auditUserId, string auditWorkstation)
        {
            return _proxy.GetCardByPan(pan, mbr, referenceNumber, auditUserId, auditWorkstation);
        }

        public List<BranchLookup> GetCardCentreList(int issuerId)
        {
            return _proxy.GetCardCentreList(issuerId);
        }

        public CardLimitData GetCardLimitDataByContractNumber(string contractNumber)
        {
            return _proxy.GetCardLimitDataByContractNumber(contractNumber);
        }

        public CardObject GetCardObject(long cardId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _proxy.GetCardObject(cardId, languageId, auditUserId, auditWorkstation);
        }

        public CardObject GetCardObjectFromExport(long exportBatchId, int languageId, long auditUserId, string auditWorkstation)
        {
            return _proxy.GetCardObjectFromExport(exportBatchId, languageId, auditUserId, auditWorkstation);
        }

        public List<CardObject> GetCardsByAccNo(int productId, string accountNumber, long auditUserId, string auditWorkStation)
        {
            return _proxy.GetCardsByAccNo(productId, accountNumber, auditUserId, auditWorkStation);
        }

        public Issuer GetIssuer(int issuerId, long auditUserId, string auditWorkStation)
        {
            return _proxy.GetIssuer(issuerId, auditUserId, auditWorkStation);
        }

        public int GetLatestSequenceNumber(int productId, long auditUserId, string auditWorkStation)
        {
            return _proxy.GetLatestSequenceNumber(productId, auditUserId, auditWorkStation);
        }

        public Parameters GetParameterIssuerInterface(int issuerId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
            return _proxy.GetParameterIssuerInterface(issuerId, interfaceTypeId, interfaceArea, interfaceGuid, auditUserId, auditWorkStation);
        }

        public Parameters GetParameterProductInterface(int productId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
            return _proxy.GetParameterProductInterface(productId, interfaceTypeId, interfaceArea, interfaceGuid, auditUserId, auditWorkStation);
        }

        public List<Parameters> GetParametersIssuerInterface(int? issuerId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
            return _proxy.GetParametersIssuerInterface(issuerId, interfaceTypeId, interfaceArea, interfaceGuid, auditUserId, auditWorkStation);
        }

        public List<Parameters> GetParametersProductInterface(int? productId, int interfaceTypeId, int interfaceArea, string interfaceGuid, long auditUserId, string auditWorkStation)
        {
            return _proxy.GetParametersProductInterface(productId, interfaceTypeId, interfaceArea, interfaceGuid, auditUserId, auditWorkStation);
        }

        public Product GetProduct(int productId, bool onlyActiveRecords, long auditUserId, string auditWorkStation)
        {
            return _proxy.GetProduct(productId, onlyActiveRecords, auditUserId, auditWorkStation);
        }

        public List<Veneka.Indigo.Integration.ProductPrinting.IProductPrintField> GetProductPrintFieldsByCode(string productCode, long auditUserId, string auditWorkStation)
        {
            return _proxy.GetProductPrintFieldsByCode(productCode, auditUserId, auditWorkStation);
        }

        public List<Product> GetProducts(int? issuerId, string bin, bool onlyActiveRecords, long auditUserId, string auditWorkStation)
        {
            return _proxy.GetProducts(issuerId, bin, onlyActiveRecords, auditUserId, auditWorkStation);
        }

        public List<Product> GetProductsByCode(int? issuerId, string productCode, bool onlyActiveRecords, long auditUserId, string auditWorkStation)
        {
            return _proxy.GetProductsByCode(issuerId, productCode, onlyActiveRecords, auditUserId, auditWorkStation);
        }

        public List<Product> GetProductsForExport(int? issuerId, bool onlyActiveRecords, long auditUserId, string auditWorkStation)
        {
            return _proxy.GetProductsForExport(issuerId, onlyActiveRecords, auditUserId, auditWorkStation);
        }

        public TerminalMK GetTerminalMasterKey(string deviceId, long auditUserId, string auditWorkStation)
        {
            return _proxy.GetTerminalMasterKey(deviceId, auditUserId, auditWorkStation);
        }

        public ZoneMasterKey GetZoneMasterKey(int issuerId, long auditUserId, string auditWorkStation)
        {
            return _proxy.GetZoneMasterKey(issuerId, auditUserId, auditWorkStation);
        }

        public string LookupBranchCode(int branchId)
        {
            return _proxy.LookupBranchCode(branchId);
        }

        public string LookupBranchName(int branchId)
        {
            return _proxy.LookupBranchName(branchId);
        }

        public string LookupCurrency(int currencyId)
        {
            return _proxy.LookupCurrency(currencyId);
        }

        public int LookupCurrency(string ccy)
        {
            return _proxy.LookupCurrency(ccy);
        }

        public string LookupCurrencyISONumericCode(int currencyId)
        {
            return _proxy.LookupCurrencyISONumericCode(currencyId);
        }

        public string LookupEmpBranchCode(int branchId)
        {
            return _proxy.LookupEmpBranchCode(branchId);
        }

        public int NextSequenceNumber(string sequenceName, ResetPeriod resetPeriod)
        {
            return _proxy.NextSequenceNumber(sequenceName, resetPeriod);
        }

        public long NextSequenceNumberLong(string sequenceName, ResetPeriod resetPeriod)
        {
            return _proxy.NextSequenceNumberLong(sequenceName, resetPeriod);
        }

        public int StatusChangeExported(long exportBatchId, int languageId, long auditUserId, string auditWorkStation)
        {
            return _proxy.StatusChangeExported(exportBatchId, languageId, auditUserId, auditWorkStation);
        }

        public void UpdateCardFeeReferenceNumber(long cardId, string referenceNumber, long auditUserId, string auditWorkStation)
        {
            _proxy.UpdateCardFeeReferenceNumber(cardId, referenceNumber, auditUserId, auditWorkStation);
        }

        public void UpdateCardFeeReversalReferenceNumber(long cardId, string referenceNumber, long auditUserId, string auditWorkStation)
        {
            _proxy.UpdateCardFeeReversalReferenceNumber(cardId, referenceNumber, auditUserId, auditWorkStation);
        }

        public void UpdateCardsAndSequenceNumber(Dictionary<long, string> cards, Dictionary<int, int> products, long auditUserId, string auditWorkStation)
        {
            _proxy.UpdateCardsAndSequenceNumber(cards, products, auditUserId, auditWorkStation);
        }
    }
}
