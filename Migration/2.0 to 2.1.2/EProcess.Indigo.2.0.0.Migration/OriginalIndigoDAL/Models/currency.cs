namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("currency")]
    public partial class currency
    {
        public currency()
        {
            issuer_product = new HashSet<issuer_product>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int currency_id { get; set; }

        [Required]
        [StringLength(3)]
        public string currency_code { get; set; }

        public virtual ICollection<issuer_product> issuer_product { get; set; }
    }
}
