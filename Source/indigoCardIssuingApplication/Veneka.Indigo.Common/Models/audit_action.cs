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
    
    public partial class audit_action
    {
        public audit_action()
        {
            this.audit_action_language = new HashSet<audit_action_language>();
            this.audit_control = new HashSet<audit_control>();
        }
    
        public int audit_action_id { get; set; }
        public string audit_action_name { get; set; }
    
        public virtual ICollection<audit_action_language> audit_action_language { get; set; }
        public virtual ICollection<audit_control> audit_control { get; set; }
    }
}
