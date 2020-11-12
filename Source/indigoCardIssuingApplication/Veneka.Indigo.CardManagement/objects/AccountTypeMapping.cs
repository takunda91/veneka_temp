using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.CardManagement.objects
{
    public class ProductAccountTypeMapping
    {
        public int ProductId { get; set; }
        public string CbsAccountType { get; set; }
        public string CmsAccountType { get; set; }
        public int IndigoAccountTypeId { get; set; }
    }
}
