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
    
    public partial class integration_fields
    {
        public integration_fields()
        {
            this.integration_responses = new HashSet<integration_responses>();
        }
    
        public int integration_id { get; set; }
        public int integration_object_id { get; set; }
        public int integration_field_id { get; set; }
        public string integration_field_name { get; set; }
        public bool accept_all_responses { get; set; }
        public byte[] integration_field_default_value { get; set; }
    
        public virtual integration_object integration_object { get; set; }
        public virtual ICollection<integration_responses> integration_responses { get; set; }
    }
}
