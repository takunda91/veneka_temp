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
    
    public partial class file_load
    {
        public file_load()
        {
            this.file_history = new HashSet<file_history>();
        }
    
        public int file_load_id { get; set; }
        public System.DateTime file_load_start { get; set; }
        public Nullable<System.DateTime> file_load_end { get; set; }
        public int user_id { get; set; }
        public int files_to_process { get; set; }
    
        public virtual ICollection<file_history> file_history { get; set; }
    }
}
