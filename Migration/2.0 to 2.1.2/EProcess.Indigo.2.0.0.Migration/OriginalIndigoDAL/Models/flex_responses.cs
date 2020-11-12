namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class flex_responses
    {
        public flex_responses()
        {
            flex_response_values = new HashSet<flex_response_values>();
        }

        [Key]
        public int flex_response_id { get; set; }

        [Required]
        [StringLength(100)]
        public string flex_response_name { get; set; }

        public virtual ICollection<flex_response_values> flex_response_values { get; set; }
    }
}
