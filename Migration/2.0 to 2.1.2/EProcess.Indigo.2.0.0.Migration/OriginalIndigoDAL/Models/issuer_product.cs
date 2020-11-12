namespace OriginalIndigoDAL.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class issuer_product
    {
        public issuer_product()
        {
            cards = new HashSet<cards>();
            currency = new HashSet<currency>();
        }

        [Key]
        public int product_id { get; set; }

        [Required]
        [StringLength(50)]
        public string product_code { get; set; }

        [Required]
        [StringLength(100)]
        public string product_name { get; set; }

        [Required]
        [StringLength(15)]
        public string product_bin_code { get; set; }

        public int issuer_id { get; set; }

        public decimal? name_on_card_top { get; set; }

        public decimal? name_on_card_left { get; set; }

        public int? Name_on_card_font_size { get; set; }

        public int? font_id { get; set; }

        public bool? DeletedYN { get; set; }

        public virtual ICollection<cards> cards { get; set; }

        public virtual issuer issuer { get; set; }

        public virtual Issuer_product_font Issuer_product_font { get; set; }

        public virtual ICollection<currency> currency { get; set; }
    }
}
