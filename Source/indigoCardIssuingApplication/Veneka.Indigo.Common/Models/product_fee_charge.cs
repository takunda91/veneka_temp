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
    
    public partial class product_fee_charge
    {
        public int fee_detail_id { get; set; }
        public int currency_id { get; set; }
        public int card_issue_reason_id { get; set; }
        public decimal fee_charge { get; set; }
        public System.DateTime date_created { get; set; }
    
        public virtual currency currency { get; set; }
        public virtual product_fee_detail product_fee_detail { get; set; }
    }
}