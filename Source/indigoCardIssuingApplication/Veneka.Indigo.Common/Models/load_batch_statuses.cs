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
    
    public partial class load_batch_statuses
    {
        public load_batch_statuses()
        {
            this.load_batch = new HashSet<load_batch>();
            this.load_batch_status = new HashSet<load_batch_status>();
            this.load_batch_statuses_language = new HashSet<load_batch_statuses_language>();
        }
    
        public int load_batch_statuses_id { get; set; }
        public string load_batch_status_name { get; set; }
    
        public virtual ICollection<load_batch> load_batch { get; set; }
        public virtual ICollection<load_batch_status> load_batch_status { get; set; }
        public virtual ICollection<load_batch_statuses_language> load_batch_statuses_language { get; set; }
    }
}
