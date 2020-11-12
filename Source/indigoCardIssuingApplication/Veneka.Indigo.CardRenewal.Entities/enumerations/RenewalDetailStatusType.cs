using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Renewal.Entities
{
    public enum RenewalDetailStatusType
    {
        [Description("Loaded")]
        Loaded = 0,

        [Description("Approved")]
        Approved = 1,

        [Description("Found in CBS")]
        Found = 2,

        [Description("Not Found in CBS")]
        NotFound = 3,

        [Description("Rejected")]
        Rejected = 4,

        [Description("Batched")]
        Batched = 5,

        [Description("Card Problem")]
        CardProblem = 6,

        [Description("Distributed")]
        Distributed = 7,
    }
}
