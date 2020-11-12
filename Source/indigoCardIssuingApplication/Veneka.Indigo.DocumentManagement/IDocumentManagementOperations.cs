using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veneka.Indigo.Common;

namespace Veneka.Indigo.DocumentManagement
{
    public interface IDocumentManagementOperations
    {
        int DocumentTypeCreate(DocumentType documentType, long auditUserId, string auditWorkstation);

        int DocumentTypeUpdate(DocumentType documentType, long auditUserId, string auditWorkstation);

        SystemResponseCode DocumentTypeDelete(int id, long auditUserId, string auditWorkstation);

        DocumentType DocumentTypeRetrieve(int id, int languageId, long auditUserId, string auditWorkstation);

        List<DocumentType> DocumentTypeRetrieveAll(bool activeOnly);

        int ProductDocumentCreate(ProductDocument productDocument, long auditUserId, string auditWorkstation);

        int ProductDocumentUpdate(ProductDocument productDocument, long auditUserId, string auditWorkstation);

        SystemResponseCode ProductDocumentDelete(int id, long auditUserId, string auditWorkstation);

        ProductDocumentListModel ProductDocumentRetrieve(int id, int languageId, long auditUserId, string auditWorkstation);

        List<ProductDocumentListModel> ProductDocumentRetrieveAll(int productId, bool activeOnly);

        long CardDocumentCreate(CardDocument cardDocument, long auditUserId, string auditWorkstation);

        long CardDocumentUpdate(CardDocument cardDocument, long auditUserId, string auditWorkstation);

        SystemResponseCode CardDocumentDelete(long id, long auditUserId, string auditWorkstation);

        CardDocument CardDocumentRetrieve(long id, int languageId, long auditUserId, string auditWorkstation);

        List<CardDocument> CardDocumentRetrieveAll(long cardId);

    }
}
