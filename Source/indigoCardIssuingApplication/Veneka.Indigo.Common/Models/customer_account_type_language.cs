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
    
    public partial class customer_account_type_language
    {
        public int account_type_id { get; set; }
        public int language_id { get; set; }
        public string language_text { get; set; }
    
        public virtual customer_account_type customer_account_type { get; set; }
        public virtual language language { get; set; }
    }
}
