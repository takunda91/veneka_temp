using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.DocumentManagement
{
    public class ProductDocumentListModel
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int DocumentTypeId { get; set; }

        public string ProductName { get; set; }

        public string DocumentTypeName { get; set; }

        public string DocumentTypeDescription { get; set; }

        public bool Included { get; set; }

        public bool IsRequired { get; set; }

        public bool IsActive { get; set; }
    }
}
