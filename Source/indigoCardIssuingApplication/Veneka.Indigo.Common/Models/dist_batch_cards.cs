//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Veneka.Indigo.Common.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class dist_batch_cards
    {
        public long dist_batch_id { get; set; }
        public long card_id { get; set; }
        public int dist_card_status_id { get; set; }
    
        public virtual card card { get; set; }
        public virtual dist_batch dist_batch { get; set; }
        public virtual dist_card_statuses dist_card_statuses { get; set; }
    }
}