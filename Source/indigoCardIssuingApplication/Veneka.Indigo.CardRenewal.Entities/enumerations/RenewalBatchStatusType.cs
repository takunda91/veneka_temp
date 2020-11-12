using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Renewal.Entities
{
    public enum RenewalBatchStatusType
    {
        Created = 0,
        Exported = 1,
        Received = 2,
        Approved = 3,
        Rejected = 4,
        Distribution = 5
    }
}
