using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.DocumentManagement
{
    public class CardDocument
    {
        public long Id { get; set; }

        public long CardId { get; set; }

        public string Location { get; set; }

        public string Comment { get; set; }

        public int DocumentTypeId { get; set; }
    }
}
