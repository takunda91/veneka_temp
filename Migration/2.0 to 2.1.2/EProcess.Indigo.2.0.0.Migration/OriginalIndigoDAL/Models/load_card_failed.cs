namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class load_card_failed
    {
        [Key]
        public long failed_card_id { get; set; }

        [Required]
        [StringLength(19)]
        public string card_number { get; set; }

        [Required]
        [StringLength(13)]
        public string card_sequence { get; set; }

        [Required]
        [StringLength(25)]
        public string load_batch_reference { get; set; }

        [Required]
        [StringLength(25)]
        public string card_status { get; set; }

        public long load_batch_id { get; set; }

        public virtual load_batch load_batch { get; set; }
    }
}
