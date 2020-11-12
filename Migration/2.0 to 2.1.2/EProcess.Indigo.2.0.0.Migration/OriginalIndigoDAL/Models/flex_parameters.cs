namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class flex_parameters
    {
        [Key]
        public int flex_parameter_id { get; set; }

        [Required]
        [StringLength(10)]
        public string source_code { get; set; }

        public long request_id { get; set; }

        [Required]
        [StringLength(100)]
        public string request_token { get; set; }

        [Required]
        [StringLength(20)]
        public string request_type { get; set; }

        [Required]
        [StringLength(100)]
        public string source_channel_id { get; set; }
    }
}
