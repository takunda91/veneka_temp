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
    
    public partial class audit_control
    {
        public long audit_id { get; set; }
        public int audit_action_id { get; set; }
        public long user_id { get; set; }
        public System.DateTime audit_date { get; set; }
        public string workstation_address { get; set; }
        public string action_description { get; set; }
        public Nullable<int> issuer_id { get; set; }
        public string data_changed { get; set; }
        public string data_before { get; set; }
        public string data_after { get; set; }
    
        public virtual audit_action audit_action { get; set; }
        public virtual user user { get; set; }
    }
}
