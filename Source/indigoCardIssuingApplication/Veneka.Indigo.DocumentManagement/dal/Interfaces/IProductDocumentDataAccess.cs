using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.DocumentManagement.dal
{
    public interface IProductDocumentDataAccess
    {
        int Create(ProductDocument entity);
        int Update(ProductDocument entity);
        bool Delete(int id);
        ProductDocumentListModel Retrieve(int id);
        IList<ProductDocumentListModel> All(int productId, bool activeOnly);
    }
}
