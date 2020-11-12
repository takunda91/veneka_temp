namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class pin_batch
    {
        [Key]
        [StringLength(50)]
        public string batch_reference { get; set; }

        public DateTime loaded_date { get; set; }

        public int card_count { get; set; }

        public int issuer_ID { get; set; }

        [StringLength(100)]
        public string manager_comment { get; set; }

        [Required]
        [StringLength(25)]
        public string batch_status { get; set; }

        [StringLength(100)]
        public string operator_comment { get; set; }

        [StringLength(20)]
        public string branch_code { get; set; }
    }
}
