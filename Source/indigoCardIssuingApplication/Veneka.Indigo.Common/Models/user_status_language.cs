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
    
    public partial class user_status_language
    {
        public int user_status_id { get; set; }
        public int language_id { get; set; }
        public string language_text { get; set; }
    
        public virtual language language { get; set; }
        public virtual user_status user_status { get; set; }
    }
}
