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
    
    public partial class branch_card_status_current
    {
        public long branch_card_status_id { get; set; }
        public long card_id { get; set; }
        public int card_priority_id { get; set; }
        public int product_id { get; set; }
        public int card_issue_method_id { get; set; }
        public int branch_id { get; set; }
        public int branch_card_statuses_id { get; set; }
        public System.DateTime status_date { get; set; }
        public long user_id { get; set; }
        public Nullable<long> operator_user_id { get; set; }
        public Nullable<int> branch_card_code_id { get; set; }
        public string comments { get; set; }
        public Nullable<int> Expr1 { get; set; }
        public Nullable<int> branch_card_code_type_id { get; set; }
        public string branch_card_code_name { get; set; }
        public Nullable<bool> branch_card_code_enabled { get; set; }
        public Nullable<bool> spoil_only { get; set; }
        public Nullable<bool> is_exception { get; set; }
    }
}
