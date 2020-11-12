using System.Collections.Generic;

namespace Veneka.Indigo.DocumentManagement.dal
{
    public interface IDocumentTypeDataAccess
    {
        int Create(DocumentType entity);
        int Update(DocumentType entity);
        bool Delete(int id);
        DocumentType Retrieve(int id);
        IList<DocumentType> All(bool activeOnly);
    }
}
