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
    
    public partial class load_card_statuses
    {
        public load_card_statuses()
        {
            this.load_batch_cards = new HashSet<load_batch_cards>();
            this.load_card_statuses_language = new HashSet<load_card_statuses_language>();
        }
    
        public int load_card_status_id { get; set; }
        public string load_card_status { get; set; }
    
        public virtual ICollection<load_batch_cards> load_batch_cards { get; set; }
        public virtual ICollection<load_card_statuses_language> load_card_statuses_language { get; set; }
    }
}
