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
    
    public partial class sp_get_current_product_fee_Result
    {
        public int fee_scheme_id { get; set; }
        public int fee_detail_id { get; set; }
        public string fee_detail_name { get; set; }
        public System.DateTime effective_from { get; set; }
        public bool fee_waiver_YN { get; set; }
        public bool fee_editable_YN { get; set; }
        public bool deleted_yn { get; set; }
        public Nullable<System.DateTime> effective_to { get; set; }
        public int fee_detail_id1 { get; set; }
        public int currency_id { get; set; }
        public int card_issue_reason_id { get; set; }
        public decimal fee_charge { get; set; }
        public System.DateTime date_created { get; set; }
    }
}
