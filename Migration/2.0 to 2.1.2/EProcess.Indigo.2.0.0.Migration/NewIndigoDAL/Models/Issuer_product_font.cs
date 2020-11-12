namespace NewIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Issuer_product_font
    {
        public Issuer_product_font()
        {
            issuer_product = new HashSet<issuer_product>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int font_id { get; set; }

        [Required]
        [StringLength(50)]
        public string font_name { get; set; }

        [StringLength(200)]
        public string resource_path { get; set; }

        public bool? DeletedYN { get; set; }

        public virtual ICollection<issuer_product> issuer_product { get; set; }

        public virtual Issuer_product_font Issuer_product_font1 { get; set; }

        public virtual Issuer_product_font Issuer_product_font2 { get; set; }
    }
}
