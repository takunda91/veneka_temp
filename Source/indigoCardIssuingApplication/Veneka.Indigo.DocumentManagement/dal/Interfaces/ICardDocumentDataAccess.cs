using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.DocumentManagement.dal
{
    public interface ICardDocumentDataAccess
    {
        long Create(CardDocument entity);
        long Update(CardDocument entity);
        bool Delete(long id);
        CardDocument Retrieve(long id);
        IList<CardDocument> All(long cardId);
    }
}
