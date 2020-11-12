using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Common;
using Veneka.Indigo.DocumentManagement.dal;

namespace Veneka.Indigo.DocumentManagement
{
    public class DocumentManagementOperations : IDocumentManagementOperations
    {
        ICardDocumentDataAccess cardDataAccess;
        IDocumentTypeDataAccess typeDataAccess;
        IProductDocumentDataAccess productDataAccess;

        public DocumentManagementOperations()
        {
            cardDataAccess = new CardDocumentDataAccess();
            productDataAccess = new ProductDocumentDataAccess();
            typeDataAccess = new DocumentTypeDataAccess();
        }

        public long CardDocumentCreate(CardDocument cardDocument, long auditUserId, string auditWorkstation)
        {
            return cardDataAccess.Create(cardDocument);
        }

        public SystemResponseCode CardDocumentDelete(long id, long auditUserId, string auditWorkstation)
        {
            if (cardDataAccess.Delete(id))
            {
                return SystemResponseCode.SUCCESS;
            }
            else
            {
                return SystemResponseCode.DELETE_FAIL;
            }
        }

        public CardDocument CardDocumentRetrieve(long id, int languageId, long auditUserId, string auditWorkstation)
        {
            return cardDataAccess.Retrieve(id);
        }

        public List<CardDocument> CardDocumentRetrieveAll(long cardId)
        {
            return cardDataAccess.All(cardId).ToList();
        }

        public long CardDocumentUpdate(CardDocument cardDocument, long auditUserId, string auditWorkstation)
        {
            return cardDataAccess.Update(cardDocument);
        }

        public int DocumentTypeCreate(DocumentType documentType, long auditUserId, string auditWorkstation)
        {
            return typeDataAccess.Create(documentType);
        }

        public SystemResponseCode DocumentTypeDelete(int id, long auditUserId, string auditWorkstation)
        {
            if (typeDataAccess.Delete(id))
            {
                return SystemResponseCode.SUCCESS;
            }
            else
            {
                return SystemResponseCode.DELETE_FAIL;
            }
        }

        public DocumentType DocumentTypeRetrieve(int id, int languageId, long auditUserId, string auditWorkstation)
        {
            return typeDataAccess.Retrieve(id);
        }

        public List<DocumentType> DocumentTypeRetrieveAll(bool activeOnly)
        {
            return typeDataAccess.All(activeOnly).ToList();
        }

        public int DocumentTypeUpdate(DocumentType documentType, long auditUserId, string auditWorkstation)
        {
            return typeDataAccess.Update(documentType);
        }

        public int ProductDocumentCreate(ProductDocument productDocument, long auditUserId, string auditWorkstation)
        {
            return productDataAccess.Create(productDocument);
        }

        public SystemResponseCode ProductDocumentDelete(int id, long auditUserId, string auditWorkstation)
        {
            if (productDataAccess.Delete(id))
            {
                return SystemResponseCode.SUCCESS;
            }
            else
            {
                return SystemResponseCode.DELETE_FAIL;
            }
        }

        public ProductDocumentListModel ProductDocumentRetrieve(int id, int languageId, long auditUserId, string auditWorkstation)
        {
            return productDataAccess.Retrieve(id);
        }

        public List<ProductDocumentListModel> ProductDocumentRetrieveAll(int productId, bool activeOnly)
        {
            return productDataAccess.All(productId, activeOnly).ToList();
        }

        public int ProductDocumentUpdate(ProductDocument productDocument, long auditUserId, string auditWorkstation)
        {
            return productDataAccess.Update(productDocument);
        }

    }
}
