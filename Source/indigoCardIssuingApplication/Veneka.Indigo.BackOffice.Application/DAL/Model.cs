using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.BackOffice.Application.DAL
{
    public class printbatch_requests
    {
        public int Id { get; set; }
        public long request_id { get; set; }

        public long print_batch_id { get; set; }

        public string pan { get; set; }

        public string printing_status { get; set; }

        

    }
}
