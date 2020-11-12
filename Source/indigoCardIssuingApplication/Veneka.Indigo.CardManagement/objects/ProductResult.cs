using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veneka.Indigo.Common.Models;

namespace Veneka.Indigo.CardManagement.objects
{
    public sealed class ProductResult
    {
        public ProductlistResult Product { get; set; }
        public List<int> CardIssueReasons { get; set; }
        public List<int> AccountTypes { get; set; }
        public List<ProductCurrencyResult> Currency { get; set; }
        public List<ProductExternalSystemResult> ExternalSystemFields { get; set; }

        public List<ProductAccountTypeMapping> AccountType_Mappings { get; set; }
        public List<product_interface> Interfaces { get; set; }
    }
}
