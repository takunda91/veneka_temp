using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.COMS.Core.Indigo.DataContracts
{
    public class PINReissue
    {
        public long PinReissueId { get; set; }
        public DateTimeOffset ReissueDate { get; set; }
        public DateTimeOffset RequestExpiry { get; set; }
    }
}
