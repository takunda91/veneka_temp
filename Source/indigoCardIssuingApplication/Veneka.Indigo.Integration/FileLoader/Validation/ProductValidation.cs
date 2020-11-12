using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Integration.DAL;
using Veneka.Indigo.Integration.FileLoader.Objects;
using Veneka.Indigo.Integration.Objects;

namespace Veneka.Indigo.Integration.FileLoader.Validation
{
    public sealed class ProductValidation : Validation
    {
        private const string VALIDATE_PRODUCTS_START_INFO = "Validate products started.";
        private const string VALIDATE_PRODUCTS_END_INFO = "Validate products completed with status: {0}";
        private const string NO_PRODUCT_FOUND = "Could not find product for PAN starting with {0}.";
        private const string PRODUCT_NOT_ACTIVE = "Product {0}-{1} is not active for PAN starting with {2}.";
        private const string NO_LOAD_FOR_PRODUCT = "Product {0}-{1} does not allow file loading.";

        private readonly ILog _logger;
        private readonly IProductDAL _productDal;

        private Dictionary<string, List<Product>> _products = new Dictionary<string, List<Product>>();//Key= BIN
        //Link product code with BIN so it can be lookuped up in _products
        private Dictionary<string, string> _productBin = new Dictionary<string, string>();

        public ProductValidation(IProductDAL productDAL, string logger)
        {            
            _productDal = productDAL;
            _logger = LogManager.GetLogger(logger);
        }

        public override FileStatuses ValidateCardFile(CardFile cardFile, List<FileCommentsObject> fileComments, long auditUserId, string auditWorkstation)
        {
            fileComments.Add(new FileCommentsObject(VALIDATE_PRODUCTS_START_INFO, _logger.Info));
            var result = base.ValidateCardFile(cardFile, fileComments, auditUserId, auditWorkstation);
            fileComments.Add(new FileCommentsObject(String.Format(VALIDATE_PRODUCTS_END_INFO, result.ToString()), _logger.Info));
            return result;
        }

        public override FileStatuses Validate(CardFileRecord record, long auditUserId, string auditWorkstation, out string fileComment)
        {
            fileComment = String.Empty;
            List<Product> products;
            //Has the product already been looked up?
            if(!_products.TryGetValue(record.BIN, out products))
            {
                _logger.DebugFormat("Searching for product {0} {1} card {2}", record.IssuerId, record.BIN, record.PAN);
                products = _productDal.GetProducts(record.IssuerId, record.BIN, false, auditUserId, auditWorkstation);
                _products.Add(record.BIN, products); //Add product to dictionary even if issuer is null, shows that we have already tried DB lookup
            }

            //record.BIN + record.SubProduct
            //Dont use PAN here as PAN might be psudopan. alway use BIN + Subproduct. 
            //File readers should always populate subproduct with the rest of the card number after BIN
            Product product = _productDal.FindBestMatch(record.BIN + record.SubProduct, products);

            //Could we find the product
            if (product == null)
            {
                _logger.Error(fileComment = String.Format(NO_PRODUCT_FOUND, record.BIN + record.SubProduct));
                return FileStatuses.NO_PRODUCT_FOUND_FOR_CARD;
            }

            //Is the product active?
            if (product.IsDeleted)
            {
                _logger.Error(fileComment = String.Format(PRODUCT_NOT_ACTIVE, product.ProductCode, product.ProductName, record.PAN.Substring(0, 6)));
                return FileStatuses.PRODUCT_NOT_ACTIVE;
            }


            //Does the product support file loading?
            if (product.ProductLoadTypeId == 0)
            {
                _logger.Error(fileComment = String.Format(NO_LOAD_FOR_PRODUCT, product.ProductCode, product.ProductName));
                return FileStatuses.NO_LOAD_FOR_PRODUCT;
            }

            _logger.DebugFormat("Using product {0} {1} {2} {3}", product.ProductId, product.ProductName, product.BIN, product.SubProductCode);

            //Do updates to file reocrd
            record.ProductId = product.ProductId;
            record.IssuerId = product.IssuerId;
            record.SubProductCode = product.SubProductCode ?? string.Empty;
            record.CardIssueMethodId = product.CardIssueMethodId;
            record.ProductLoadTypeId = product.ProductLoadTypeId;

            return FileStatuses.READ;            
        }

        public override FileStatuses Validate(BulkRequestRecord record, long auditUserId, string auditWorkstation, out string fileComment)
        {
            fileComment = String.Empty;
            List<Product> products;

            //Has the product already been looked up?
            if (!_products.TryGetValue(record.ProductCode, out products))
            {
                products = _productDal.GetProductsByCode(null, record.ProductCode, false, auditUserId, auditWorkstation);
                _products.Add(record.ProductCode, products); //Add product to dictionary even if issuer is null, shows that we have already tried DB lookup
            }

            Product product = products.Where(p => p.ProductCode.Equals(record.ProductCode, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            //Could we find the product
            if (product == null)
            {
                _logger.Error(fileComment = String.Format(NO_PRODUCT_FOUND, record.ProductCode));
                return FileStatuses.NO_PRODUCT_FOUND_FOR_CARD;
            }

            //Is the product active?
            if (product.IsDeleted)
            {
                _logger.Error(fileComment = String.Format(PRODUCT_NOT_ACTIVE, product.ProductCode, product.ProductName, product.BIN));
                return FileStatuses.PRODUCT_NOT_ACTIVE;
            }


            //Does the product support file loading?
            if (product.ProductLoadTypeId == 0)
            {
                _logger.Error(fileComment = String.Format(NO_LOAD_FOR_PRODUCT, product.ProductCode, product.ProductName));
                return FileStatuses.NO_LOAD_FOR_PRODUCT;
            }

            //Do updates to file reocrd
            record.ProductId = product.ProductId;
            record.IssuerId = product.IssuerId;
            record.BIN = product.BIN;
            record.SubProductCode = product.SubProductCode ?? string.Empty;
            record.ProductLoadTypeId = product.ProductLoadTypeId;            

            return FileStatuses.READ;        
        }

        public override void Clear()
        {
            _productBin.Clear();
            _products.Clear();
        }
    }
}
