using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.Renewal.Entities
{
    public enum RenewalStatusType
    {
        [Description("Loaded")]
        Loaded = 0,

        [Description("Load Confirmed")]
        LoadConfirmed = 1, 
             
        [Description("File Approved")] 
        Approved = 2,
        
        [Description("Cards Verified")] 
        Verified = 3,
        
        [Description("Created")]
        BatchCreated = 4,

        [Description("File Rejected")]
        Rejected = 5
    }
}
