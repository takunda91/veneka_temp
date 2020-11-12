using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Veneka.Indigo.DocumentManagement;

namespace IndigoCardIssuanceService.Models
{
    public class ProductDocumentStructure
    {
        public DocumentStorageType StorageType { get; set; }

        public List<ProductDocumentListModel> ProductDocuments { get; set; }
    }
}