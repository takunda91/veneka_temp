namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class dist_batch_statuses_language
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int dist_batch_statuses_id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int language_id { get; set; }

        [Required]
        [StringLength(100)]
        public string language_text { get; set; }

        public virtual dist_batch_statuses dist_batch_statuses { get; set; }

        public virtual languages languages { get; set; }
    }
}