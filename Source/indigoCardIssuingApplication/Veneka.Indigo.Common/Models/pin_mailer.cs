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
    
    public partial class pin_mailer
    {
        public string pin_mailer_reference { get; set; }
        public string batch_reference { get; set; }
        public string pin_mailer_status { get; set; }
        public string card_number { get; set; }
        public string pvv_offset { get; set; }
        public string encrypted_pin { get; set; }
        public string customer_name { get; set; }
        public string customer_address { get; set; }
        public Nullable<System.DateTime> printed_date { get; set; }
        public Nullable<System.DateTime> reprinted_date { get; set; }
        public Nullable<int> reprint_request_YN { get; set; }
    }
}
