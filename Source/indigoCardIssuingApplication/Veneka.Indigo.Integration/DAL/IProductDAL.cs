using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.Integration.ProductPrinting;

namespace Veneka.Indigo.Integration.DAL
{
    [ServiceContract]
    public interface IProductDAL
    {
        [OperationContract(Name = "AddFloats")]
        Product FindBestMatch(int? issuerId, string pan, bool onlyActiveRecords, long auditUserId, string auditWorkstation);
        [OperationContract(Name = "FindBestMatchByProduct")]
        Product FindBestMatch(string pan, List<Product> products);
        [OperationContract]
        Product GetProduct(int productId, bool onlyActiveRecords, long auditUserId, string auditWorkStation);
        [OperationContract]
        List<IProductPrintField> GetProductPrintFieldsByCode(string productCode, long auditUserId, string auditWorkStation);
        [OperationContract]
        List<Product> GetProducts(int? issuerId, string bin, bool onlyActiveRecords, long auditUserId, string auditWorkStation);
        [OperationContract]
        List<Product> GetProductsByCode(int? issuerId, string productCode, bool onlyActiveRecords, long auditUserId, string auditWorkStation);
        [OperationContract]
        List<Product> GetProductsForExport(int? issuerId, bool onlyActiveRecords, long auditUserId, string auditWorkStation);
    }
}