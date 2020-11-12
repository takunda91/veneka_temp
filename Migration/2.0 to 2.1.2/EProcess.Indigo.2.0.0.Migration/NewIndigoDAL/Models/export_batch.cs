namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class export_batch
    {
        public export_batch()
        {
            cards = new HashSet<cards>();
            export_batch_status = new HashSet<export_batch_status>();
        }

        [Key]
        public long export_batch_id { get; set; }

        public int issuer_id { get; set; }

        [Required]
        [StringLength(100)]
        public string batch_reference { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime date_created { get; set; }

        public int no_cards { get; set; }

        public virtual ICollection<cards> cards { get; set; }

        public virtual issuer issuer { get; set; }

        public virtual ICollection<export_batch_status> export_batch_status { get; set; }
    }
}
