namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class flex_response_values
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int flex_response_value_id { get; set; }

        public int flex_response_id { get; set; }

        [Required]
        [StringLength(100)]
        public string flex_response_value { get; set; }

        public bool valid_response { get; set; }

        public virtual flex_responses flex_responses { get; set; }
    }
}
