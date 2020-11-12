namespace NewIndigoDAL.Models
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
            product_fee_charge = new HashSet<product_fee_charge>();
            issuer_product = new HashSet<issuer_product>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int currency_id { get; set; }

        [Required]
        [StringLength(3)]
        public string currency_code { get; set; }

        [Required]
        [StringLength(3)]
        public string iso_4217_numeric_code { get; set; }

        public int? iso_4217_minor_unit { get; set; }

        [Required]
        [StringLength(100)]
        public string currency_desc { get; set; }

        public bool active_YN { get; set; }

        public virtual ICollection<product_fee_charge> product_fee_charge { get; set; }

        public virtual ICollection<issuer_product> issuer_product { get; set; }
    }
}
