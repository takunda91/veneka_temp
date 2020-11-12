namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class issuer_statuses
    {
        public issuer_statuses()
        {
            issuer = new HashSet<issuer>();
            issuer_statuses_language = new HashSet<issuer_statuses_language>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int issuer_status_id { get; set; }

        [Required]
        [StringLength(50)]
        public string issuer_status_name { get; set; }

        public virtual ICollection<issuer> issuer { get; set; }

        public virtual ICollection<issuer_statuses_language> issuer_statuses_language { get; set; }
    }
}
