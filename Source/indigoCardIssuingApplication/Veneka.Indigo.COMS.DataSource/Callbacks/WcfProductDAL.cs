using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.COMS.Core;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Integration.Objects;
using Veneka.Indigo.Integration.ProductPrinting;

namespace Veneka.Indigo.COMS.DataSource.Callbacks
{
    class WcfProductDAL:IProductDAL
    {
        private readonly IComsCallback _proxy;
        public WcfProductDAL(IComsCallback proxy)
        {
            _proxy = proxy;
        }
       public Product FindBestMatch(int? issuerId, string pan, bool onlyActiveRecords, long auditUserId, string auditWorkstation)
        {
            return _proxy.FindBestMatch(issuerId, pan, onlyActiveRecords, auditUserId, auditWorkstation);
        }

        public Product FindBestMatch(string pan, List<Product> products)
        {
            return _proxy.FindBestMatch(pan, products);
        }

        public Product GetProduct(int productId, bool onlyActiveRecords, long auditUserId, string auditWorkStation)
        {
            return _proxy.GetProduct(productId, onlyActiveRecords, auditUserId, auditWorkStation);
        }

        public List<IProductPrintField> GetProductPrintFieldsByCode(string productCode, long auditUserId, string auditWorkStation)
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
    }
}
