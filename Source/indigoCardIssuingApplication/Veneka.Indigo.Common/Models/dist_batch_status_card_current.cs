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
    
    public partial class dist_batch_status_card_current
    {
        public long card_id { get; set; }
        public int product_id { get; set; }
        public int branch_id { get; set; }
        public byte[] card_number { get; set; }
        public int card_sequence { get; set; }
        public byte[] card_index { get; set; }
        public int card_issue_method_id { get; set; }
        public int card_priority_id { get; set; }
        public string card_request_reference { get; set; }
        public byte[] card_production_date { get; set; }
        public byte[] card_expiry_date { get; set; }
        public byte[] card_activation_date { get; set; }
        public byte[] pvv { get; set; }
        public Nullable<int> sub_product_id { get; set; }
        public long dist_batch_id { get; set; }
        public long Expr1 { get; set; }
        public int dist_card_status_id { get; set; }
        public long dist_batch_status_id { get; set; }
        public long Expr2 { get; set; }
        public int dist_batch_statuses_id { get; set; }
        public long user_id { get; set; }
        public System.DateTime status_date { get; set; }
        public string status_notes { get; set; }
    }
}
